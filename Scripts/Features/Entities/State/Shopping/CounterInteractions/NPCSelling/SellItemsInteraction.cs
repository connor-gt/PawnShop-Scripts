using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Represents the current phase of the item-selling interaction.
/// </summary>
public enum SellingPhase
{
    /// <summary>
    /// The interaction cannot currently present an offer because one or more eligibility requirements are not met.
    /// </summary>
    CannotPresentYet,

    /// <summary>
    /// The interaction is ready to present an offer when the player interacts with the entity.
    /// </summary>
    ReadyToPresent,

    /// <summary>
    /// The interaction is currently presenting the seller's offer and preparing the negotiation.
    /// </summary>
    PresentOffer,

    /// <summary>
    /// The initial offer has been presented and the interaction is waiting for the player's response.
    /// </summary>
    AwaitingPlayerResponse
}

/// <summary>
/// Provides the shared negotiation flow for counter-based interactions where an entity sells items to the player.
/// </summary>
public class SellItemsInteraction : CounterInteraction
{
    /// <summary>
    /// Gets the current phase of the selling interaction.
    /// </summary>
    public SellingPhase Phase { get; private set; }

    private NegotiationSession negotiationSession;
    private readonly NegotiationService negotiationService;
    private readonly INegotiationDialogueProvider negotiationDialogueProvider;

    /// <summary>
    /// Gets the temporary input binding used to control the negotiation.
    /// </summary>
    private readonly TemporaryInputBinding negotiationInput;

    // So the player can hold down the lower/raise keybindings and have them continuously lower/raise
    private readonly HoldRepeat lowerInputHoldRepeat;
    private readonly HoldRepeat raiseInputHoldRepeat;

    /// <summary>
    /// The amount of time the player must hold an offer adjustment input before it begins repeating.
    /// </summary>
    private const float INITIAL_HOLD_DELAY = 0.5f;

    /// <summary>
    /// The amount of time between repeated offer adjustments while an adjustment input is held.
    /// </summary>
    private const float HOLD_REPEAT_DELAY = 0.04f;

    /// <summary>
    /// The hover message displayed when the counter must be cleared before the offer can be presented.
    /// </summary>
    private const string CLEAR_COUNTER_MSG = "Clear Counter To See Offer";

    /// <summary>
    /// The hover message displayed when the negotiation offer is ready to be presented.
    /// </summary>
    private const string SEE_OFFER_MSG = "See Offer";

    /// <summary>
    /// The hover message displayed when a paused negotiation can be resumed.
    /// </summary>
    private const string RESUME_MSG = "Resume Negotiations";

    /// <summary>
    /// The minimum offer-to-asking-price ratio considered acceptable before the NPC considers the player's offer
    /// insulting.
    /// </summary>
    private const float INSULTED_OFFER_THRESHOLD = 0.6f;

    protected override string GoToServiceAreaMsg => "Go Behind Counter To See Offer";

    /// <summary>
    /// Initializes a new instance of the <see cref="SellItemsInteraction"/>.
    /// </summary>
    /// <param name="negotiationDialogueProvider">Provides the dialogue and input configuration used during the
    /// negotiation.</param>
    /// <param name="counter">The counter hosting the interaction.</param>
    /// <param name="entity">The entity selling items to the player.</param>
    /// <param name="sellerProfileModule">The module that contains the seller's profile that determines how the
    /// entity evaluates negotiation offers.</param>
    public SellItemsInteraction(INegotiationDialogueProvider negotiationDialogueProvider, Counter counter,
        Entity entity, SellerProfileModule sellerProfileModule) : base(counter, entity)
    {
        this.negotiationDialogueProvider = negotiationDialogueProvider;

        negotiationService = new NegotiationService(sellerProfileModule);

        negotiationInput = new NegotiationInputFactory(negotiationDialogueProvider).CreateNegotiationInputBinding(
            OnPlayerAccepts,
            OnPlayerDeclines,
            OnPlayerLowersOffer,
            OnPlayerRaisesOffer,
            OnPlayerSubmitsOffer
        );

        lowerInputHoldRepeat = new HoldRepeat(INITIAL_HOLD_DELAY, HOLD_REPEAT_DELAY);
        raiseInputHoldRepeat = new HoldRepeat(INITIAL_HOLD_DELAY, HOLD_REPEAT_DELAY);
    }

    public override void Begin()
    {
        base.Begin();
        negotiationSession = new NegotiationSession(inventoryModule.GetItems(), negotiationService);

        if (!CanPresentOffer())
        {
            Phase = SellingPhase.CannotPresentYet;
        }
        else
        {
            ReadyToPresentOffer();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        switch (Phase)
        {
            case SellingPhase.CannotPresentYet:
                CheckForEligibility();
                break;

            case SellingPhase.ReadyToPresent:
                // Do nothing whilst waiting for the player standing at the front of the line
                break;

            case SellingPhase.PresentOffer:
                Phase = SellingPhase.AwaitingPlayerResponse;
                Entity.StartCoroutine(PresentOffer());
                break;

            case SellingPhase.AwaitingPlayerResponse:
                ProcessOfferAdjustmentInput();
                break;
        }
    }

    public override void InteractWithEntity()
    {
        base.InteractWithEntity();

        switch (Phase)
        {
            case SellingPhase.ReadyToPresent:
                Phase = SellingPhase.PresentOffer;
                break;
        }

        if (dialogueModule.IsPaused)
        {
            dialogueModule.ResumeConversation();
        }
    }

    /// <summary>
    /// Determines whether the selling interaction is currently eligible to present an offer.
    /// </summary>
    /// <returns>True if the player is in the counter's service area and the counter is empty; false if otherwise.
    /// </returns>
    private bool CanPresentOffer()
    {
        return Counter.IsPlayerInServiceArea && Counter.ItemCount == 0;
    }

    /// <summary>
    /// Checks whether the interaction has become eligible to present an offer and updates the hover message when
    /// an eligibility requirement is not met.
    /// </summary>
    private void CheckForEligibility()
    {
        if (CanPresentOffer())
        {
            ReadyToPresentOffer();
        }
        else if (!Counter.IsPlayerInServiceArea)
        {
            if (!HoverMessage.Equals(GoToServiceAreaMsg))
            {
                HoverMessage = GoToServiceAreaMsg;
                interactionModule.RequestRefresh();
            }
        }
        else if (Counter.ItemCount > 0)
        {
            if (!HoverMessage.Equals(CLEAR_COUNTER_MSG))
            {
                HoverMessage = CLEAR_COUNTER_MSG;
                interactionModule.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Transitions the interaction into the ready state and enables the dialogue events required to manage the
    /// negotiation lifecycle.
    /// </summary>
    private void ReadyToPresentOffer()
    {
        Phase = SellingPhase.ReadyToPresent;
        HoverMessage = interactionModule.BuildPrompt(SEE_OFFER_MSG);
        interactionModule.RequestRefresh();

        dialogueModule.OnConversationPaused += OnConversationPaused;
        dialogueModule.OnConversationResumed += OnConversationResumed;
        dialogueModule.OnConversationEnded += OnConversationEnded;
    }

    /// <summary>
    /// Presents the seller's items at the counter, prepares their interaction information, and plays the opening
    /// negotiation dialogue before enabling negotiation input.
    /// <para>
    /// This is a coroutine because it waits for the opening dialogue to finish before showing and enabling the
    /// negotiation context/input.
    /// </para>
    /// </summary>
    private IEnumerator PresentOffer()
    {
        HoverMessage = "";

        DialogueSO openingDialogue = negotiationDialogueProvider.GetNegotiationDialogue(
            NegotiationDialogueCategory.Opening
        );

        yield return Entity.StartCoroutine(PlayDialogue(openingDialogue));

        IReadOnlyList<Item> itemsInInventory = inventoryModule.Empty();
        Counter.PlaceItems(itemsInInventory);
        Counter.LockItemsFromPlayer(true);

        List<ItemInstance> itemsOnCounter = Counter.GetItemInstances();
        for (int i = 0; i < itemsOnCounter.Count; i++)
        {
            // Only show the item's name as the hover message during the negotiation session
            if (itemsOnCounter[i].TryGetComponent(out IHoverMessageUser hoverMessageUser))
            {
                hoverMessageUser.HoverMessage = $"{itemsOnCounter[i].Data.Name}";
            }

            // Set the item's popup information
            if (itemsOnCounter[i].TryGetComponent(out IItemInfoPopupUser itemInfoPopupUser))
            {
                itemInfoPopupUser.SetContentProvider(ItemServices.CreateSellerInfo);
            }
        }

        // Makes sure input is only enabled if the player has not left the interaction whilst the dialogue was playing
        if (!dialogueModule.IsPaused)
        {
            RefreshNegotiationContext();
            negotiationInput.Enable();
        }
    }

    /// <summary>
    /// Plays the specified dialogue according to its configured <see cref="DialogueType"/> and waits for it to finish.
    /// <para>
    /// Conversational dialogue is played as a complete conversation, while one-liner dialogue is played by selecting
    /// a random line from the dialogue.
    /// </para>
    /// </summary>
    /// <param name="dialogue">The dialogue to play.</param>
    private IEnumerator PlayDialogue(DialogueSO dialogue)
    {
        dialogueModule.StartConversation(dialogue);
        while (dialogueModule.IsDialogueInProgress())
        {
            yield return null;
        }
    }

    /// <summary>
    /// Completes the transaction by purchasing the seller's items at the player's accepted offer price and preparing
    /// the items for sale in the shop.
    /// <para>
    /// This should be called when a deal has been reached between the NPC and the player, where the player purchases
    /// the NPCs items.
    /// </para>
    /// </summary>
    private void CloseDeal()
    {
        int totalPurchasePrice = negotiationSession.PlayerOffer;
        Counter.LockItemsFromPlayer(false);
        Counter.Register.SubtractCash(totalPurchasePrice);

        // TODO: remove magic numbers, but these are here for quick testing currently
        float minMargin = 70f;
        float maxMargin = 80f;

        List<ItemInstance> itemsOnCounter = Counter.GetItemInstances();
        ItemServices.AssignItemPurchasePrices(itemsOnCounter, totalPurchasePrice);
        ItemServices.AssignDefaultListingPrices(itemsOnCounter, Random.Range(minMargin, maxMargin));

        ClearCustomHoverMessages(itemsOnCounter);

        negotiationInput.Disable();
        HideNegotiationContext();
    }

    /// <summary>
    /// Handles this NPC denying the player's submitted offer whilst keeping the negotiation session active.
    /// </summary>
    private void DenyPlayerOffer()
    {
        // NOTE: the coroutine PlayDialogue() is not used in this method since mid-negotiation dialogue currently
        // only supports random one-liners
        int playerOffer = negotiationSession.PlayerOffer;
        if (playerOffer < negotiationSession.HighestDeniedPlayerOffer)
        {
            dialogueModule.StartConversation(
                negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.PlayerOfferBelowPreviousOffer
                )
            );

            return;
        }

        int npcAskingPrice = negotiationSession.NPCAskingPrice;
        float ratio = (float)playerOffer / npcAskingPrice;

        if (ratio < INSULTED_OFFER_THRESHOLD)
        {
            dialogueModule.StartConversation(
                negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.DenyPlayerOfferInsulted
                )
            );
        }
        else
        {
            dialogueModule.StartConversation(
                negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.DenyPlayerOffer
                )
            );
        }
    }

    /// <summary>
    /// Ends the negotiation without completing the transaction and returns the seller's items to their inventory.
    /// <para>
    /// This should be called when the NPC "walks away" from the negotiations, taking their items with them.
    /// </para>
    /// </summary>
    private void WalkAway()
    {
        List<Item> itemsOnCounter = Counter.RetrieveItems();
        inventoryModule.Add(itemsOnCounter);

        negotiationInput.Disable();
        HideNegotiationContext();
    }

    /// <summary>
    /// Clears any custom hover messages from the specified items so their default hover messages can be displayed.
    /// </summary>
    /// <param name="items">The item instances whose hover messages should be reset.</param>
    private void ClearCustomHoverMessages(List<ItemInstance> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            // Stop overriding the hover message so the generic hover message is used instead (letting the player
            // know they can now pick the items up from the counter)
            if (items[i].TryGetComponent(out IHoverMessageUser hoverMessageUser))
            {
                hoverMessageUser.HoverMessage = null;
            }
        }
    }

    /// <summary>
    /// Handles the player accepting the current offer.
    /// </summary>
    private void OnPlayerAccepts(InputAction.CallbackContext ctx)
    {
        CloseDeal();
        Entity.StartCoroutine(
            PlayDialogue(
                negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.PlayerAcceptedInitialOffer
                )
            )
        );
    }

    /// <summary>
    /// Handles the player declining the current offer.
    /// </summary>
    private void OnPlayerDeclines(InputAction.CallbackContext ctx)
    {
        WalkAway();
        Entity.StartCoroutine(
            PlayDialogue(
                negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.ClosingFailed
                )
            )
        );
    }

    /// <summary>
    /// Handles the player lowering their current offer.
    /// </summary>
    private void OnPlayerLowersOffer(InputAction.CallbackContext ctx)
    {
        negotiationSession.LowerPlayerOffer(1);
        RefreshNegotiationContext();
#if UNITY_EDITOR
        Debug.Log($"Offer ratio = {negotiationService.CalculateOfferValueRatio(negotiationSession)}");
#endif
    }

    /// <summary>
    /// Handles the player raising their current offer.
    /// </summary>
    private void OnPlayerRaisesOffer(InputAction.CallbackContext ctx)
    {
        negotiationSession.RaisePlayerOffer(1);
        RefreshNegotiationContext();
#if UNITY_EDITOR
        Debug.Log($"Offer ratio = {negotiationService.CalculateOfferValueRatio(negotiationSession)}");
#endif
    }

    /// <summary>
    /// Evaluates the player's submitted offer and handles the resulting negotiation outcome.
    /// </summary>
    private void OnPlayerSubmitsOffer(InputAction.CallbackContext ctx)
    {
        NegotiationResult result = negotiationSession.SubmitPlayerOffer();

        switch (result)
        {
            case NegotiationResult.Accept:
                CloseDeal();
                Entity.StartCoroutine(
                    PlayDialogue(
                        negotiationDialogueProvider.GetNegotiationDialogue(
                            NegotiationDialogueCategory.ClosingSuccessful
                        )
                    )
                );
                break;

            case NegotiationResult.DenyPlayerOffer:
                DenyPlayerOffer();
                break;
                
            case NegotiationResult.Reject:
                WalkAway();
                Entity.StartCoroutine(
                    PlayDialogue(
                        negotiationDialogueProvider.GetNegotiationDialogue(
                            NegotiationDialogueCategory.ClosingFailedInsulted
                        )
                    )
                );
                break;
        }
    }

    /// <summary>
    /// Handles the negotiation dialogue being paused, either preparing the interaction to resume or finishing it
    /// when the negotiation has reached a terminal dialogue state.
    /// </summary>
    private void OnConversationPaused()
    {
        if (ShouldFinishAfterDialogueEnds(dialogueModule.CurrentDialogue))
        {
            HoverMessage = "";
            Finish();
        }
        else
        {
            negotiationInput.Disable();
            HoverMessage = interactionModule.BuildPrompt(RESUME_MSG);
        }
    }

    /// <summary>
    /// Resumes negotiation input and restores the current negotiation context when dialogue resumes.
    /// </summary>
    private void OnConversationResumed()
    {
        negotiationInput.Enable();
        HoverMessage = "";
        RefreshNegotiationContext();
    }

    /// <summary>
    /// Handles the negotiation dialogue ending and finishes the interaction when a terminal negotiation dialogue
    /// has completed.
    /// </summary>
    /// <param name="dialogue">The dialogue that ended.</param>
    private void OnConversationEnded(DialogueSO dialogue)
    {
        if (ShouldFinishAfterDialogueEnds(dialogue))
        {
            Finish();
        }
    }

    /// <summary>
    /// Determines whether the specified dialogue represents the end of the negotiation.
    /// </summary>
    /// <param name="dialogue">The dialogue to evaluate.</param>
    /// <returns>True if the dialogue represents a successful or failed negotiation ending, false if otherwise.
    /// </returns>
    private bool ShouldFinishAfterDialogueEnds(DialogueSO dialogue)
    {
        return dialogue == negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.PlayerAcceptedInitialOffer)
            || dialogue == negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.ClosingSuccessful)
            || dialogue == negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.ClosingFailed)
            || dialogue == negotiationDialogueProvider.GetNegotiationDialogue(
                    NegotiationDialogueCategory.ClosingFailedInsulted);
    }

    /// <summary>
    /// Refreshes the dialogue context to display the current negotiation offers.
    /// </summary>
    private void RefreshNegotiationContext()
    {
        string context = $"NPC wants: ${negotiationSession.NPCAskingPrice}\n";
        context += $"Your offer: ${negotiationSession.PlayerOffer}";
        dialogueModule.SetExtraContext(show: true, context);
    }

    /// <summary>
    /// Hides the negotiation dialogue context.
    /// </summary>
    private void HideNegotiationContext()
    {
        dialogueModule.SetExtraContext(false, "");
    }

    /// <summary>
    /// Processes held inputs that adjust the player's current offer.
    /// <para>
    /// This method checks the state of both inputs each update and uses <see cref="HoldRepeat"/> to determine when
    /// another adjustment should occur, allowing the player to quickly change their offer without requiring a
    /// separate button press for every dollar.
    /// </para>
    /// </summary>
    private void ProcessOfferAdjustmentInput()
    {
        if (lowerInputHoldRepeat.ShouldTick(IsLowerOfferInputHeld()))
        {
            OnPlayerLowersOffer(default);
        }

        if (raiseInputHoldRepeat.ShouldTick(IsRaiseOfferInputHeld()))
        {
            OnPlayerRaisesOffer(default);
        }
    }

    /// <summary>
    /// Determines whether the input assigned to lowering the player's offer is currently being held.
    /// </summary>
    /// <returns>True if the lower-offer input is being held, false if otherwise.</returns>
    private bool IsLowerOfferInputHeld()
    {
        return negotiationInput.IsInputBeingHeld(negotiationDialogueProvider.Choices.Lower.InputAction);
    }

    /// <summary>
    /// Determines whether the input assigned to raising the player's offer is currently being held.
    /// </summary>
    /// <returns>True if the raise-offer input is being held, false if otherwise.</returns>
    private bool IsRaiseOfferInputHeld()
    {
        return negotiationInput.IsInputBeingHeld(negotiationDialogueProvider.Choices.Raise.InputAction);
    }
}
