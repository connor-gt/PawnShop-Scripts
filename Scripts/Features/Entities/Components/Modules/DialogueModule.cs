using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A module that is responsible for handling the dialogue of this entity.
/// </summary>
public class DialogueModule : MonoBehaviour
{
    /// <summary>
    /// Invoked when the conversation was paused via <see cref="PauseConversation"/> (e.g., when the player
    /// wanders too far away).
    /// </summary>
    public event Action OnConversationPaused;

    /// <summary>
    /// Invoked when the conversation resumes from a paused state via <see cref="ResumeConversation"/>.
    /// </summary>
    public event Action OnConversationResumed;

    /// <summary>
    /// Invoked when the conversation has ended via <see cref="EndConversation"/>.
    /// </summary>
    public event Action<DialogueSO> OnConversationEnded;

    /// <summary>
    /// Gets the max distance the player can move away before the conversation is automatically paused.
    /// </summary>
    [field: Title("Distance Settings")]
    [field: Tooltip("The max distance the player can move away before the conversation is automatically ended.")]
    [field: SuffixLabel("units")]
    [field: SerializeField]
    [field: Min(0f)]
    public float ConversationRange { get; private set; } = 3.15f;

    [Tooltip("The amount of time between checks to see whether the player is still within conversation range.")]
    [SuffixLabel("seconds")]
    [SerializeField]
    [Min(0f)]
    private float distanceCheckInterval = 0.25f;

    /// <summary>
    /// Monitors the distance between this entity and the player while a conversation is active.
    /// </summary>
    private Coroutine monitorConversationRoutine = null;
    private WaitForSeconds waitForSeconds;

    /// <summary>
    /// Gets the dialogue that is currently being played.
    /// </summary>
    [ShowInInspector, ReadOnly]
    public DialogueSO CurrentDialogue { get; private set; }

    /// <summary>
    /// Gets the index of the currently displayed dialogue line.
    /// </summary>
    [ShowInInspector, ReadOnly]
    public int DialogueIndex { get; private set; } = 0;

    /// <summary>
    /// Gets whether a conversation is currently in progress.
    /// </summary>
    public bool InConversation => CurrentDialogue != null;

    /// <summary>
    /// Gets whether or not the current conversation is paused (is always false if there is no active
    /// conversation).
    /// </summary>
    [ShowInInspector, ReadOnly]
    public bool IsPaused { get; private set; }

    /// <summary>
    /// Gets whether the current dialogue line is still being "typed out" by the type writer effect.
    /// </summary>
    public bool IsTyping => DialogueUI.IsTyping;

    private Coroutine playLineRoutine = null;
    
    // Lazy initialization because the gameplay UI scene containing DialogueUIController may not have loaded yet
    private DialogueUIController _dialogueUI = null;
    private DialogueUIController DialogueUI
    {
        get
        {
            if (_dialogueUI == null && DialogueUIController.Instance != null)
            {
                _dialogueUI = DialogueUIController.Instance;
            }

            return _dialogueUI;
        }
    }

    /// <summary>
    /// Stores the input callbacks for the currently available dialogue choices so they can be unsubscribed
    /// when the dialogue advances or ends.
    /// </summary>
    private readonly Dictionary<InputAction, Action<InputAction.CallbackContext>> choiceActions = new();

    /// <summary>
    /// Gets whether the current continue input should be ignored. This prevents the same input used to
    /// advance dialogue from immediately skipping the newly displayed dialogue line
    /// <para>
    /// e.g. when the dialogue response uses the spacebar, pressing it would trigger the response AND the
    /// <see cref="OnContinuePerformed"/> method.
    /// </para>
    /// </summary>
    private bool suppressContinueInput;

    /// <summary>
    /// The amount of time continue input is ignored after displaying a new dialogue line.
    /// </summary>
    private const float SUPPRESS_CONTINUE_INPUT_DUR = 0.15f;

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(distanceCheckInterval);
    }

    private void OnEnable()
    {
        InputManager.Instance.Actions.Base.Continue.performed += OnContinuePerformed;
    }

    private void OnDisable()
    {
        InputManager.Instance.Actions.Base.Continue.performed -= OnContinuePerformed;
    }

    /// <summary>
    /// Starts displaying the specified dialogue, selecting the starting line according to its configured
    /// <see cref="DialogueType"/>.
    /// </summary>
    /// <param name="dialogue">The dialogue conversation to display.</param>
    public void StartConversation(DialogueSO dialogue)
    {
        int startingIndex = dialogue.IsConversational ? 0 : dialogue.GetRandomIndex();
        StartConversation(dialogue, startingIndex);
    }

    /// <summary>
    /// Starts displaying the specified dialogue from a specific line index.
    /// </summary>
    /// <param name="dialogue">The dialogue conversation to display.</param>
    /// <param name="startingIndex">The dialogue line index to begin displaying from.</param>
    private void StartConversation(DialogueSO dialogue, int startingIndex)
    {
        IsPaused = false;

        // Replace a conversation if one already exists
        if (playLineRoutine != null)
        {
            StopCoroutine(playLineRoutine);
            playLineRoutine = null;
        }

        DialogueUI.ShowDialogueUI(true);
        CurrentDialogue = dialogue;
        DialogueIndex = startingIndex;

        if (monitorConversationRoutine == null)
        {
            monitorConversationRoutine = StartCoroutine(MonitorConversationRoutine());
        }

        PlayLine(DialogueIndex);
    }

    /// <summary>
    /// End the current dialogue conversation and clear any remaining dialogue choices.
    /// </summary>
    public void EndConversation()
    {
        if (playLineRoutine != null)
        {
            StopCoroutine(playLineRoutine);
            playLineRoutine = null;
        }

        if (monitorConversationRoutine != null)
        {
            StopCoroutine(monitorConversationRoutine);
            monitorConversationRoutine = null;
        }

        DialogueUI.ShowDialogueUI(false);
        ClearDialogueChoices();

        DialogueSO dialogue = CurrentDialogue;
        CurrentDialogue = null;
        OnConversationEnded?.Invoke(dialogue);
    }

    /// <summary>
    /// Determines whether the currently active dialogue is still in progress.
    /// <para>
    /// One-liner dialogue is considered complete once its current line finishes typing. Conversational dialogue
    /// remains in progress while its current response indicates that the conversation should advance to another line.
    /// </para>
    /// </summary>
    /// <returns>True if the current dialogue is still being presented or should continue to another line, false if
    /// otherwise.</returns>
    public bool IsDialogueInProgress()
    {
        if (!InConversation)
        {
            return false;
        }
        
        if (IsTyping)
        {
            return true;
        }

        // This is a pretty hacky way to check if the current dialogue is conversational or a one-liner, but this
        // works because it's expected that ALL dialogue lines have at least one response (which is why I'm using
        // ChoiceLinks[0]), and the only reason why ShouldAdvanceDialogue would be false is because the line's options
        // are using the negotiation options, which are the only dialogue lines that don't advance conversations
        DialogueResponse response = CurrentDialogue.GetResponse(DialogueIndex);
        return response.ChoiceLinks[0].ShouldAdvanceDialogue;
    }

    /// <summary>
    /// Play the specified dialogue line.
    /// </summary>
    /// <param name="dialogueIndex">The dialogue line index to play.</param>
    private void PlayLine(int dialogueIndex)
    {
        if (playLineRoutine != null)
        {
            StopCoroutine(playLineRoutine);
        }

        playLineRoutine = StartCoroutine(
            PlayLineRoutine(CurrentDialogue.GetLine(dialogueIndex), CurrentDialogue.GetResponse(dialogueIndex))
        );
    }

    /// <summary>
    /// Play a dialogue line and wait for the typewriter effect to finish before displaying the available
    /// dialogue choices.
    /// </summary>
    /// <param name="line">The dialogue line to display.</param>
    /// <param name="response">The dialogue branch associated with the line.</param>
    private IEnumerator PlayLineRoutine(string line, DialogueResponse response)
    {
        ClearDialogueChoices();

        // Prevents the same continue input used to advance dialogue from immediately skipping the next line
        suppressContinueInput = true;
        this.Invoke(() => suppressContinueInput = false, SUPPRESS_CONTINUE_INPUT_DUR);

        DialogueUI.PlayLine(line);
        while (DialogueUI.IsTyping)
        {
            yield return null;
        }

        CreateChoicesFromResponse(response);
        playLineRoutine = null;
    }

    /// <summary>
    /// Add a dialogue choice and subscribe to its associated input action.
    /// </summary>
    /// <param name="choiceText">The text displayed for the dialogue choice.</param>
    /// <param name="inputAction">The input action used to select the dialogue choice.</param>
    /// <param name="onChosen">The callback invoked when the dialogue choice is selected.</param>
    private void AddDialogueChoice(string choiceText, InputAction inputAction, Action<InputAction.CallbackContext> onChosen)
    {
        DialogueUI.AddChoice(choiceText);

        if (choiceActions.TryAdd(inputAction, onChosen))
        {
            inputAction.performed += onChosen;
        }
    }

    /// <summary>
    /// Remove all dialogue choices from the dialogue UI and unsubscribe their associated input actions.
    /// </summary>
    private void ClearDialogueChoices()
    {
        foreach (var choiceActionKVP in choiceActions)
        {
            choiceActionKVP.Key.performed -= choiceActionKVP.Value;
        }

        DialogueUI.ClearChoices();
        choiceActions.Clear();
    }

    /// <summary>
    /// Create dialogue choices from a <see cref="DialogueResponse"/>.
    /// </summary>
    /// <param name="response">The <see cref="DialogueResponse"/> to create choices from.</param>
    private void CreateChoicesFromResponse(DialogueResponse response)
    {
        foreach (var choiceLink in response.ChoiceLinks)
        {
            string choiceText = $"[{InputManager.Instance.GetInputActionAsString(choiceLink.Choice.InputAction)}]: ";
            choiceText += choiceLink.Choice.Label;

            AddDialogueChoice(
                choiceText,
                choiceLink.Choice.InputAction,
                (InputAction.CallbackContext ctx) => ChooseOption(choiceLink)
            );
        }
    }

    /// <summary>
    /// Handle the player selecting a dialogue choice.
    /// </summary>
    /// <param name="choice">The selected choice, including whether to advance dialogue and which line to show next.</param>
    private void ChooseOption(DialogueChoiceLink choice)
    {
        if (!choice.ShouldAdvanceDialogue)
        {
            return;
        }

        if (choice.NextDialogueIndex == -1)
        {
            EndConversation();
            return;
        }

        DialogueIndex = choice.NextDialogueIndex;
        PlayLine(DialogueIndex);
    }

    private void OnContinuePerformed(InputAction.CallbackContext ctx)
    {
        if (suppressContinueInput)
        {
            return;
        }

        if (InConversation && DialogueUI.IsTyping)
        {
            DialogueUI.SkipTyping();
        }
    }

    /// <summary>
    /// Monitors the distance between the player and this entity while a conversation is active. The conversation
    /// is automatically paused if the player moves outside of the allowed conversation range.
    /// </summary>
    private IEnumerator MonitorConversationRoutine()
    {
        Transform playerTransform = PawnShop.Instance.Player.transform;

        while (InConversation && transform.WithinDistance(playerTransform, ConversationRange))
        {
            yield return waitForSeconds;
        }

        PauseConversation();
        monitorConversationRoutine = null;
    }

    /// <summary>
    /// Temporarily hides the active conversation without ending it.
    /// <para>
    /// This preserves the current dialogue state, allowing the conversation to be resumed later using
    /// <see cref="ResumeConversation"/>.
    /// </para>
    /// </summary>
    /// <returns>true if a conversation was active and successfully paused, false if otherwise.</returns>
    public bool PauseConversation()
    {
        if (InConversation)
        {
            IsPaused = true;
            DialogueUI.ShowDialogueUI(false);
            OnConversationPaused?.Invoke();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resumes a previously paused conversation and restores the dialogue UI.
    /// <para>
    /// The conversation resumes from the current <see cref="DialogueIndex"/> rather than restarting from
    /// the beginning.
    /// </para>
    /// </summary>
    /// <returns>true if an active conversation existed and was successfully resumed, false if otherwise.</returns>
    public bool ResumeConversation()
    {
        if (InConversation)
        {
            StartConversation(CurrentDialogue, DialogueIndex);
            OnConversationResumed?.Invoke();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the currently active dialogue matches the specified dialogue asset.
    /// </summary>
    /// <param name="other">The dialogue asset to compare against the currently active dialogue.</param>
    /// <returns>True if the specified dialogue asset matches the <see cref="CurrentDialogue"/>, false if otherwise.
    /// </returns>
    public bool CurrentDialogueEquals(DialogueSO other)
    {
        return CurrentDialogue != null && CurrentDialogue == other;
    }

    /// <summary>
    /// Set the extra context section that's displayed below dialogue.
    /// </summary>
    /// <param name="show">Whether or not to show or hide the extra context.</param>
    /// <param name="text">The text to display.</param>
    public void SetExtraContext(bool show, string text)
    {
        DialogueUI.SetExtraContext(show, text);
    }
}
