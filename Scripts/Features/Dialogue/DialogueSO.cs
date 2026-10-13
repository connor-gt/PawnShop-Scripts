using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

/// <summary>
/// Represents the different ways a <see cref="DialogueSO"/> can be presented to the player.
/// </summary>
public enum DialogueType
{
    /// <summary>
    /// Dialogue that is presented as a complete conversation, potentially containing multiple lines and player
    /// responses.
    /// </summary>
    Conversational,

    /// <summary>
    /// A single dialogue line selected from the available lines without requiring the player to progress through
    /// a conversation.
    /// </summary>
    OneLiner
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Scriptable Objects/Dialogue/New Dialogue")]
public class DialogueSO : ScriptableObject
{
    /// <summary>
    /// Gets the way this dialogue is intended to be presented to the player.
    /// </summary>
    [field: Title("Dialogue Type")]
    [field: Tooltip("Should this dialogue be played as a complete conversation or as a single one-liner?")]
    [field: SerializeField]
    public DialogueType DialogueType { get; private set; } = DialogueType.Conversational;

    /// <summary>
    /// Gets whether this dialogue is intended to be played as a complete conversation.
    /// </summary>
    public bool IsConversational => DialogueType == DialogueType.Conversational;

    /// <summary>
    /// Gets the dialogue lines and their corresponding responses.
    /// </summary>
    [field: Title("Dialogue Lines")]
    [field: ListDrawerSettings(ShowFoldout = false, ShowIndexLabels = true, ShowPaging = false)]
    [field: RequiredListLength(1, null)]
    [field: SerializeField]
    public DialogueLine[] DialogueLines { get; private set; }

    /// <summary>
    /// Gets the count of <see cref="DialogueLine"/>s this asset has.
    /// </summary>
    public int DialogueLineCount => DialogueLines.Length;

    /// <summary>
    /// Get the dialogue's line at a particular index.
    /// </summary>
    /// <param name="index">The index to get the line.</param>
    /// <returns>A string as the dialogue index.</returns>
    public string GetLine(int index)
    {
        return DialogueLines[index].Line;
    }

    /// <summary>
    /// Get the dialogue's <see cref="DialogueResponse"/> at a particular index.
    /// </summary>
    /// <param name="index">The index to get the response.</param>
    /// <returns>A <see cref="DialogueResponse"/> instance.</returns>
    public DialogueResponse GetResponse(int index)
    {
        return DialogueLines[index].Response;
    }

    /// <summary>
    /// Get a random line index of dialogue.
    /// </summary>
    /// <returns>A random line index of dialogue.</returns>
    /// <exception cref="System.Exception">Thrown if no lines were set in the inspector.</exception>
    public int GetRandomIndex()
    {
        if (DialogueLineCount == 0)
        {
            throw new System.Exception($"No dialogue lines were set for {name}!");
        }

        return Random.Range(0, DialogueLineCount);
    }
}

#if UNITY_EDITOR
/// <summary>
/// Custom editor for <see cref="DialogueSO"/> that provides additional tools for quickly modifying dialogue
/// assets in the Unity inspector.
/// </summary>
[CustomEditor(typeof(DialogueSO))]
public class DialogueSOEditor : OdinEditor
{
    /// <summary>
    /// The negotiation choices asset used when automatically populating dialogue responses.
    /// </summary>
    private NegotiationChoicesSO negotiationChoicesSO;

    public override void OnInspectorGUI()
    {
        DialogueSO dialogue = (DialogueSO)target;

        DrawDefaultInspector();
        GUILayout.Space(10);

        GUILayout.Label("Dialogue Tools", EditorStyles.boldLabel);
        GUILayout.Space(10);

        negotiationChoicesSO = (NegotiationChoicesSO)EditorGUILayout.ObjectField(
            "Negotiation Choices SO",
            negotiationChoicesSO,
            typeof(NegotiationChoicesSO),
            false
        );
        GUILayout.Space(10);

        if (GUILayout.Button("Apply Negotiation Choices"))
        {
            ApplyNegotiationChoices(dialogue);
        }
    }

    /// <summary>
    /// Applies the standard negotiation choices to every dialogue line in the target dialogue asset.
    /// <para>
    /// This is a convenience editor tool used to quickly populate negotiation dialogue responses without
    /// manually assigning each choice link.
    /// </para>
    /// </summary>
    /// <param name="dialogue">The dialogue asset to modify.</param>
    private void ApplyNegotiationChoices(DialogueSO dialogue)
    {
        if (negotiationChoicesSO == null)
        {
            return;
        }

        // So Ctrl + Z works to undo the changes made to the dialogue asset
        Undo.RecordObject(dialogue, "Apply Negotiation Choices");

        for (int i = 0; i < dialogue.DialogueLines.Length; i++)
        {
            dialogue.DialogueLines[i].Response.ChoiceLinks = new DialogueChoiceLink[]
            {
                new DialogueChoiceLink(negotiationChoicesSO.Accept, false, i),
                new DialogueChoiceLink(negotiationChoicesSO.Decline, false, i),
                new DialogueChoiceLink(negotiationChoicesSO.Lower, false, i),
                new DialogueChoiceLink(negotiationChoicesSO.Raise, false, i),
                new DialogueChoiceLink(negotiationChoicesSO.Submit, false, i)
            };
        }

        Debug.Log($"Applied negotiation choices to {dialogue.name}");
        EditorUtility.SetDirty(dialogue);
    }
}
#endif
