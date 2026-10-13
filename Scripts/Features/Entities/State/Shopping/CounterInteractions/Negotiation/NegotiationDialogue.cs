using System;
using Sirenix.OdinInspector;
using UnityEngine;

/*
    NOTE: This is probably not the best way to handle negotiation dialogue since this is pretty static and not
    very flexible, but given the scope of this project and the fact this is only for negotiations I'm fine with it.
*/

/// <summary>
/// Provides the dialogue and player response choices used during an NPC negotiation.
/// </summary>
[Serializable]
public class NegotiationDialogue : INegotiationDialogueProvider
{
    /// <summary>
    /// The dialogue used to begin the negotiation and present the NPC's initial offer.
    /// </summary>
    [field: Title("Dialogue Lines")]
    [field: Tooltip("Dialogue used to begin the negotiation and present the NPC's initial offer.")]
    [field: SerializeField]
    public DialogueSO Opening { get; private set; }

    /// <summary>
    /// The dialogue used when the player accepts the NPC's initial offer without negotiating.
    /// </summary>
    [field: Tooltip("Dialogue used when the player accepts the NPC's initial offer without negotiating.")]
    [field: SerializeField]
    public DialogueSO PlayerAcceptedInitialOffer { get; private set; }

    /// <summary>
    /// The dialogue used when the NPC rejects the player's offer during negotiation.
    /// </summary>
    [field: Tooltip("Dialogue used when the NPC rejects the player's offer during negotiation.")]
    [field: SerializeField]
    public DialogueSO DenyPlayerOffer { get; private set; }

    /// <summary>
    /// The dialogue used when the NPC reacts negatively to an unusually low offer from the player.
    /// </summary>
    [field: Tooltip("Dialogue used when the NPC reacts negatively to an unusually low player offer.")]
    [field: SerializeField]
    public DialogueSO DenyPlayerOfferInsulted { get; private set; }

    /// <summary>
    /// The dialogue used when the player submits an offer lower than an offer the NPC previously rejected.
    /// </summary>
    [field: Tooltip("Dialogue used when the player submits an offer lower than one the NPC previously rejected.")]
    [field: SerializeField]
    public DialogueSO PlayerOfferBelowPreviousOffer { get; private set; }

    /// <summary>
    /// The dialogue used when the NPC accepts the player's offer and the negotiation ends successfully.
    /// </summary>
    [field: Tooltip("Dialogue used when the NPC accepts the player's offer and the negotiation ends successfully.")]
    [field: SerializeField]
    public DialogueSO SuccessfulClose { get; private set; }

    /// <summary>
    /// The dialogue used when the NPC ends the negotiation.
    /// </summary>
    [field: Tooltip("Dialogue used when the NPC ends the negotiation.")]
    [field: SerializeField]
    public DialogueSO ClosingFailed { get; private set; }

    /// <summary>
    /// The dialogue used when the negotiation ends unsuccessfully due to the NPC becoming dissatisfied/insulted
    /// by the player's offers.
    /// </summary>
    [field: Tooltip("Dialogue used when the NPC becomes dissatisfied/insulted and ends the negotiation.")]
    [field: SerializeField]
    public DialogueSO ClosingFailedInsulted { get; private set; }

    /// <summary>
    /// The player response choices available during the negotiation.
    /// </summary>
    [field: Title("Player Response Choices")]
    [field: Tooltip("The player response choices available during the negotiation.")]
    [field: SerializeField]
    public NegotiationChoicesSO Choices { get; private set; }

    /// <summary>
    /// Gets the dialogue associated with the specified negotiation dialogue category.
    /// </summary>
    /// <param name="dialogueType">The category of dialogue to retrieve.</param>
    /// <returns>The dialogue asset associated with the specified category, or null if no dialogue is associated
    /// with the category.</returns>
    public DialogueSO GetNegotiationDialogue(NegotiationDialogueCategory dialogueType)
    {
        switch (dialogueType)
        {
            case NegotiationDialogueCategory.Opening:
                return Opening;

            case NegotiationDialogueCategory.PlayerAcceptedInitialOffer:
                return PlayerAcceptedInitialOffer;

            case NegotiationDialogueCategory.DenyPlayerOffer:
                return DenyPlayerOffer;

            case NegotiationDialogueCategory.DenyPlayerOfferInsulted:
                return DenyPlayerOfferInsulted;

            case NegotiationDialogueCategory.PlayerOfferBelowPreviousOffer:
                return PlayerOfferBelowPreviousOffer;

            case NegotiationDialogueCategory.ClosingSuccessful:
                return SuccessfulClose;

            case NegotiationDialogueCategory.ClosingFailed:
                return ClosingFailed;

            case NegotiationDialogueCategory.ClosingFailedInsulted:
                return ClosingFailedInsulted;

            default:
                return null;
        }
    }
}
