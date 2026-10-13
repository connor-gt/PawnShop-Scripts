/// <summary>
/// Provides negotiation-specific dialogue to <see cref="Entity"/>s that sell items to the player.
/// </summary>
public interface INegotiationDialogueProvider
{
    /// <summary>
    /// Gets the dialogue asset associated with the specified stage of the negotiation.
    /// </summary>
    /// <param name="dialogueType">The type of negotiation dialogue to retrieve.</param>
    /// <returns>The dialogue asset that should be played for the requested negotiation.</returns>
    DialogueSO GetNegotiationDialogue(NegotiationDialogueCategory dialogueType);

    /// <summary>
    /// Gets the set of choices available to the player during negotiations.
    /// </summary>
    NegotiationChoicesSO Choices { get; }
}
