using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single negotiation between the player and an NPC.
/// <para>
/// Tracks the initial asking price along with the current offers made by both the player and the NPC throughout
/// the negotiation.
/// </para>
/// </summary>
public class NegotiationSession
{
    /// <summary>
    /// Gets the list of items that are used for this session.
    /// </summary>
    public IReadOnlyList<Item> Items { get; private set; }

    /// <summary>
    /// Gets the total estimated value of the list of items.
    /// <para>
    /// For example, if there are 3 items with estimated values of $50, $60, and $70, the sum of all 3 estimated
    /// values is returned, so $180.
    /// </para>
    /// </summary>
    public int EstimatedValue => ItemServices.GetTotalEstimatedValue(Items);

    /// <summary>
    /// Gets the price the NPC wants for the items.
    /// </summary>
    public readonly int NPCAskingPrice;

    /// <summary>
    /// Gets the player's current offer.
    /// </summary>
    public int PlayerOffer { get; private set; }

    /// <summary>
    /// Gets the highest player offer that the NPC has denied while continuing the negotiation.
    /// </summary>
    public int HighestDeniedPlayerOffer { get; private set; }

    /// <summary>
    /// Gets how many times the player has submitted an offer for the NPC to evaluate.
    /// </summary>
    public int OffersSubmitted { get; private set; }

    private readonly NegotiationService negotiationService;

    /// <summary>
    /// Creates a new negotiation session with the specified items, negotiation service, and NPC's desired price.
    /// </summary>
    /// <param name="items">The list of items that are involved in this session.</param>
    /// <param name="negotiationService">The service that determines how the NPC evaluates the player's offers.
    /// </param>
    /// <param name="npcAskingPrice">The price the NPC wants for the items.</param>
    public NegotiationSession(IReadOnlyList<Item> items, NegotiationService negotiationService, int npcAskingPrice)
    {
        this.negotiationService = negotiationService;
        Items = items;

        OffersSubmitted = 0;
        NPCAskingPrice = npcAskingPrice;
        PlayerOffer = npcAskingPrice;
    }

    /// <summary>
    /// Creates a new negotiation session with the specified items and negotiation service.
    /// <para> 
    /// The NPC's desired price is calculated using the service and items by calling
    /// <see cref="NegotiationService.CalculateStartingOffer(IReadOnlyList{Item})"/>.
    /// </para>
    /// </summary>
    /// <param name="items">The list of items that are involved in this session.</param>
    /// <param name="negotiationService">The service that determines how the NPC evaluates the player's offers.
    /// </param>
    public NegotiationSession(IReadOnlyList<Item> items, NegotiationService negotiationService)
        : this(items, negotiationService, negotiationService.CalculateStartingOffer(items)) { }

    /// <inheritdoc cref="NegotiationSession(IReadOnlyList{Item}, NegotiationService)" />
    public NegotiationSession(List<Item> items, NegotiationService negotiationService)
        : this((IReadOnlyList<Item>)items, negotiationService) { }

    /// <summary>
    /// Raises the player's current offer by the specified amount.
    /// </summary>
    /// <param name="amount">The amount to increase the offer by.</param>
    public void RaisePlayerOffer(int amount)
    {
        PlayerOffer += amount;
    }

    /// <summary>
    /// Lowers the player's current offer by the specified amount.
    /// </summary>
    /// <param name="amount">The amount to decrease the offer by.</param>
    public void LowerPlayerOffer(int amount)
    {
        PlayerOffer -= amount;
    }

    /// <summary>
    /// Submits the player's current offer for the NPC to evaluate.
    /// </summary>
    /// <returns>The result of the NPC's evaluation of the player's current offer.</returns>
    public NegotiationResult SubmitPlayerOffer()
    {
        OffersSubmitted++;

        NegotiationResult result = negotiationService.EvaluateSession(this);
        if (result == NegotiationResult.DenyPlayerOffer)
        {
            HighestDeniedPlayerOffer = Mathf.Max(HighestDeniedPlayerOffer, PlayerOffer);
        }

        return result;
    }
}
