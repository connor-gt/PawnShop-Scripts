using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Completes when the player finishes a conversation with a specific NPC.
/// </summary>
public class TalkToNPCTask : PlayerTask
{
    [Title("NPC")]
    [Tooltip("The dialogue module for the NPC the player needs to talk to.")]
    [Required]
    [SerializeField]
    private DialogueModule dialogueModule;

    private void OnEnable()
    {
        dialogueModule.OnConversationEnded += OnConversationEnded;
    }

    private void OnDisable()
    {
        dialogueModule.OnConversationEnded -= OnConversationEnded;
    }

    private void OnConversationEnded(DialogueSO dialogue)
    {
        TryComplete();
    }
}
