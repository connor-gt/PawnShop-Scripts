public enum NegotiationResult
{
    /// <summary>
    /// The NPC accepts the player's offer.
    /// </summary>
    Accept,

    /// <summary>
    /// The NPC denies the player's offer but is willing to continue the negotiation.
    /// </summary>
    DenyPlayerOffer,

    /// <summary>
    /// The NPC outright rejects the player's offer.
    /// </summary>
    Reject
}
