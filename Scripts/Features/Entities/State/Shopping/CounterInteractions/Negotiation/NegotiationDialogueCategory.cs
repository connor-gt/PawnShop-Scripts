/// <summary>
/// Represents the different categories of dialogue that can occur during a negotiation sequence.
/// </summary>
public enum NegotiationDialogueCategory
{
    /// <summary>
    /// Dialogue that begins the negotiation and presents the NPC's initial offer.
    /// </summary>
    Opening,

    /// <summary>
    /// Dialogue that occurs when the player accepts the NPC's initial offer without negotiating.
    /// </summary>
    PlayerAcceptedInitialOffer,

    /// <summary>
    /// Dialogue that occurs when the NPC rejects the player's offer.
    /// </summary>
    DenyPlayerOffer,

    /// <summary>
    /// Dialogue that occurs when the NPC reacts negatively to an unusually low player offer.
    /// </summary>
    DenyPlayerOfferInsulted,

    /// <summary>
    /// Dialogue that occurs when the player submits an offer lower than an offer the NPC previously rejected.
    /// </summary>
    PlayerOfferBelowPreviousOffer,

    /// <summary>
    /// Dialogue that occurs when the NPC accepts the player's offer and the negotiation ends.
    /// </summary>
    ClosingSuccessful,

    /// <summary>
    /// Dialogue that occurs when the NPC rejects the player's offer alltogether and the negotiation ends.
    /// </summary>
    ClosingFailed,

    /// <summary>
    /// Dialogue that occurs when the NPC becomes particularly dissatisfied with the player's offers and ends
    /// the negotiation.
    /// </summary>
    ClosingFailedInsulted
}
