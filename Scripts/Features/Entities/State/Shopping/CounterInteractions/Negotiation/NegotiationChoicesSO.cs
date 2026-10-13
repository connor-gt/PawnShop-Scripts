using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Stores the standard dialogue choices used during item negotiations.
/// <para>
/// I made this asset to avoid manually recreating the same negotiation responses for every negotiation dialogue asset.
/// Editor tools (such as the one found in <see cref="DialogueSO"/>) can use this asset to automatically populate dialogue
/// responses, ensuring that all negotiation dialogues share a consistent set of choices.
/// </para>
/// </summary>
[CreateAssetMenu(fileName = "NewNegotiationChoices", menuName = "Scriptable Objects/Dialogue/New Negotiation Choices")]
public class NegotiationChoicesSO : ScriptableObject
{
    /// <summary>
    /// The choice used when the player accepts the NPC's offer.
    /// </summary>
    [field: Title("Negotiation Choices")]
    [field: Tooltip("The choice used when the player accepts the NPC's offer.")]
    [field: SerializeField]
    public DialogueChoiceSO Accept { get; private set; }

    /// <summary>
    /// The choice used when the player declines the negotiation all-together.
    /// </summary>
    [field: Tooltip("The choice used when the player declines the negotiation all-together.")]
    [field: SerializeField]
    public DialogueChoiceSO Decline { get; private set; }

    /// <summary>
    /// The choice used when the player lowers their proposed offer.
    /// </summary>
    [field: Tooltip("The choice used when the player lowers their proposed offer.")]
    [field: SerializeField]
    public DialogueChoiceSO Lower { get; private set; }

    /// <summary>
    /// The choice used when the player raises their proposed offer.
    /// </summary>
    [field: Tooltip("The choice used when the player raises their proposed offer.")]
    [field: SerializeField]
    public DialogueChoiceSO Raise { get; private set; }

    /// <summary>
    /// The choice used when the player submits their current offer to the NPC.
    /// </summary>
    [field: Tooltip("The choice used when the player submits their current offer to the NPC.")]
    [field: SerializeField]
    public DialogueChoiceSO Submit { get; private set; }
}
