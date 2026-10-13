using System;
using UnityEngine;

/// <summary>
/// Represents the player's available responses after a specific dialogue line, including the available
/// choices and what dialogue line each choice continues to.
/// </summary>
[Serializable]
public class DialogueChoiceLink
{
    // Empty constructor required for Unity serialization when having a constructor with parameters
    public DialogueChoiceLink() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogueChoiceLink"/> class with the specified dialogue
    /// choice and next dialogue index.
    /// </summary>
    /// <param name="choice">The dialogue choice associated with this link.</param>
    /// <param name="nextDialogueIndex">The index of the next dialogue line that this choice leads to.</param>
    public DialogueChoiceLink(DialogueChoiceSO choice, bool advanceDialogue, int nextDialogueIndex)
    {
        Choice = choice;
        ShouldAdvanceDialogue = advanceDialogue;
        NextDialogueIndex = nextDialogueIndex;
    }

    /// <summary>
    /// Gets the dialogue choice associated with this link.
    /// </summary>
    [field: Tooltip("A response choice the player can pick associated with this line.")]
    [field: SerializeField]
    public DialogueChoiceSO Choice { get; private set; }

    /// <summary>
    /// Gets whether or not the dialogue should advance to another line after this choice is selected (e.g., 
    /// for negotiation dialogue).
    /// </summary>
    [field: Tooltip("Should this line advance the dialogue?")]
    [field: SerializeField]
    public bool ShouldAdvanceDialogue { get; private set; } = true;

    /// <summary>
    /// Gets the index of the next dialogue line that this choice leads to.
    /// </summary>
    [field: Tooltip("The next dialogue line's index to go to after this line completes.")]
    [field: SerializeField]
    public int NextDialogueIndex { get; private set; }
}
