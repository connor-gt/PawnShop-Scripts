using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;

public class DialogueUIController : MonoBehaviour
{
    public static DialogueUIController Instance { get; private set; }

    [Title("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Title("Speaker")]
    [Tooltip("The text box used to display the current speaker's name.")]
    [SerializeField]
    private TextMeshProUGUI speakerText;

    [Title("Dialogue")]
    [Tooltip("The text box used to display dialogue.")]
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [Tooltip("The type writer effect used when displaying dialogue.")]
    [SerializeField]
    private TypeWriterEffect typeWriterEffect;

    /// <summary>
    /// Gets whether the type writer effect is currently "typing" dialogue.
    /// </summary>
    public bool IsTyping => typeWriterEffect.InProgress;

    [Title("Dialogue Context")]
    [Tooltip("The text box used to extra dialogue context (e.g. negotation info when haggling with NPCs).")]
    [SerializeField]
    private TextMeshProUGUI contextText;

    [Title("Options")]
    [Tooltip("The parent object that dialogue choice UI elements are instantiated under.")]
    [SerializeField]
    private GameObject choicesContainer;

    [Tooltip("The prefab used to display a dialogue choice.")]
    [SerializeField]
    private GameObject choicePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ShowDialogueUI(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play a dialogue line using the type writer effect.
    /// <para>
    /// <b>Note:</b> The line is played via a coroutine, and if the type writing effect needs to be cut short,
    /// <see cref="SkipTyping()"/> can be called to instantly complete and display the dialogue line.
    /// </para>
    /// </summary>
    /// <param name="dialogueLine">The dialogue line to display.</param>
    public void PlayLine(string dialogueLine)
    {
        typeWriterEffect.BeginEffect(dialogueLine);
    }

    /// <summary>
    /// Immediately finishes the current type writer effect.
    /// </summary>
    public void SkipTyping()
    {
        typeWriterEffect.Finish();
    }

    /// <summary>
    /// Remove all dialogue choices from the dialogue UI.
    /// </summary>
    public void ClearChoices()
    {
        foreach (Transform choice in choicesContainer.transform)
        {
            Destroy(choice.gameObject);
        }
    }

    /// <summary>
    /// Add a dialogue choice to the dialogue UI.
    /// </summary>
    /// <param name="choiceText">The text displayed for the dialogue choice.</param>
    public void AddChoice(string choiceText)
    {
        GameObject choiceObj = Instantiate(choicePrefab, choicesContainer.transform);
        TextMeshProUGUI choiceTextBox = choiceObj.GetComponent<TextMeshProUGUI>();
        choiceTextBox.text = choiceText;
    }

    /// <summary>
    /// Show or hide the dialogue UI.
    /// <para>
    /// This also hides and resets any extra context that was set via <see cref="SetExtraContext"/>, so if the
    /// dialogue box UI is shown again and extra context needs to be display, <see cref="SetExtraContext"/>
    /// needs to be called again.
    /// </para>
    /// </summary>
    /// <param name="show">true to the show dialogue UI, false to hide it.</param>
    public void ShowDialogueUI(bool show)
    {
        if (show)
        {
            canvasGroup.alpha = 1f;
        }
        else
        {
            canvasGroup.alpha = 0f;
            SetExtraContext(false, "");
        }
    }

    /// <summary>
    /// Set the extra context section that's displayed below dialogue.
    /// </summary>
    /// <param name="show">Whether or not to show or hide the extra context.</param>
    /// <param name="text">The text to display.</param>
    public void SetExtraContext(bool show, string text)
    {
        contextText.text = text;
        contextText.gameObject.SetActive(show);
    }
}
