using UnityEngine;

public class GenericShopperBrain : Brain, IShopper, IExitRequester, IEntityInteractionHandler
{
    public ShelfItem CurrentShelfItem { get; set; }

    private readonly GenericShopperBrainSO brainData;

    private readonly BuyerProfileModule buyerProfileModule;
    private readonly InventoryModule inventoryModule;
    private readonly InteractionModule interactionModule;
    private readonly DialogueModule dialogueModule;

    private readonly BuyerService buyerService;
    private readonly ShopperService shopperService;

    private readonly RandomTimedIdleState randomTimedIdleState; // Used as an intermediary behavior
    private readonly MoveToShelfItemState moveToShelfItemState;
    private readonly MoveToRandomPositionState moveToRandomPositionState; // Used as fallback behavior
    private readonly RotateTowardsTargetState rotateTowardsTargetState;
    private readonly GrabItemState grabItemState;
    private readonly CheckoutInteraction checkoutInteraction;
    private readonly CounterInteractionState checkoutState;
    private readonly LeaveStoreState leaveStoreState;

    private readonly ShelfController shelfController;

    /// <summary>
    /// Gets how many items this shopper has browsed.
    /// </summary>
    public int ItemsBrowsed { get; private set; }

    public GenericShopperBrain(Entity entity, GenericShopperBrainSO brainData, BuyerProfileModule buyerProfileModule,
        ShelfController shelfController, EntityController entityController, Counter purchaseCounter) : base(entity)
    {
        this.brainData = brainData;
        this.buyerProfileModule = buyerProfileModule;

        this.shelfController = shelfController;

        inventoryModule = entity.GetComponent<InventoryModule>();
        interactionModule = entity.GetComponent<InteractionModule>();
        dialogueModule = entity.GetComponent<DialogueModule>();

        buyerService = new BuyerService(buyerProfileModule);
        shopperService = new ShopperService(entity, this);

        randomTimedIdleState = new RandomTimedIdleState(entity, brainData.RandomIdleTime.Min, brainData.RandomIdleTime.Max);
        moveToShelfItemState = new MoveToShelfItemState(entity);
        moveToRandomPositionState = new MoveToRandomPositionState(entity, brainData.WanderRange);
        rotateTowardsTargetState = new RotateTowardsTargetState(entity, brainData.RotateSpeed, brainData.FixedIdleTime);
        grabItemState = new GrabItemState(entity);
        checkoutInteraction = new CheckoutInteraction(purchaseCounter, entity);
        checkoutState = new CounterInteractionState(checkoutInteraction, brainData.RotateSpeed);
        leaveStoreState = new LeaveStoreState(entity, entityController);

        randomTimedIdleState.OnEnded += OnRandomTimedIdleEnded;
        moveToShelfItemState.OnEnded += OnMoveToShelfItemEnded;
        moveToRandomPositionState.OnEnded += OnMoveToRandomPositionEnded;
        rotateTowardsTargetState.OnEnded += OnRotateTowardsTargetEnded;
        grabItemState.OnEnded += OnGrabItemEnded;
        checkoutState.OnEnded += OnCheckoutEnded;

        ItemsBrowsed = 0;
    }

    public override void StartThinking()
    {
        FindNewItemToBrowse();
    }

    private void OnMoveToShelfItemEnded(StateResult result)
    {
        if (result.ResultType == StateResultType.Completed && CurrentShelfItem?.Holdable != null)
        {
            rotateTowardsTargetState.SetLookTarget(CurrentShelfItem.Holdable.transform);
            TransitionTo(rotateTowardsTargetState);
        }
        else
        {
            FallbackBehavior();
        }
    }

    private void OnRotateTowardsTargetEnded(StateResult result)
    {
        ItemsBrowsed++;

        if (result.ResultType == StateResultType.Completed
            && CurrentShelfItem?.Holdable != null
            && buyerService.ShouldBuy(CurrentShelfItem.Holdable))
        {
            grabItemState.SetTarget(CurrentShelfItem.Holdable);
            TransitionTo(grabItemState);
        }
        else
        {
            DecideNextAction();
        }
    }

    private void OnGrabItemEnded(StateResult result)
    {
        // No need for fallback behavior since the grab item state completes within a single frame when it's
        // been transitioned to
        TransitionTo(randomTimedIdleState);
        shopperService.ScheduleShelfItemRelease(randomTimedIdleState.IdleTime);
    }

    private void OnRandomTimedIdleEnded(StateResult result)
    {
        DecideNextAction();
    }

    private void OnMoveToRandomPositionEnded(StateResult result)
    {
        TransitionTo(randomTimedIdleState);
    }

    private void OnCheckoutEnded(StateResult result)
    {
        if (result.ResultType == StateResultType.Completed)
        {
            LeaveStore();
        }
        else
        {
            FallbackBehavior();
        }
    }

    private void DecideNextAction()
    {
        // If this entity was using an interaction point, make sure to release it before moving on
        shopperService.TryReleaseCurrentShelfItem();

        if (inventoryModule.Count > 0
            && (inventoryModule.IsFull || ItemsBrowsed >= buyerProfileModule.Profile.MaxItemsToBrowse))
        {
            Checkout();
            return;
        }

        if (ItemsBrowsed >= buyerProfileModule.Profile.MaxItemsToBrowse && inventoryModule.Count == 0)
        {
            LeaveStore();
            return;
        }

        FindNewItemToBrowse();
    }

    private void FindNewItemToBrowse()
    {
        CurrentShelfItem = shelfController.GetRandomShelfItem(requiresAvailability: true);

        if (CurrentShelfItem != null && CurrentShelfItem.InteractionPoint.TryReserve(entity))
        {
            moveToShelfItemState.SetTargetShelfItem(CurrentShelfItem);
            TransitionTo(moveToShelfItemState);
        }
        else
        {
            FallbackBehavior();
        }
    }

    // Called externally by director controllers
    public void WrapUpAndLeave()
    {
        if (inventoryModule.Count > 0)
        {
            Checkout();
        }
        else
        {
            LeaveStore();
        }
    }

    private void FallbackBehavior()
    {
        shopperService.TryReleaseCurrentShelfItem();
        TransitionTo(moveToRandomPositionState);
    }

    private void Checkout()
    {
        TransitionTo(checkoutState);
    }

    private void LeaveStore()
    {
        TransitionTo(leaveStoreState);
    }

    public string GetHoverMessage()
    {
        if (entity.StateMachine.ActiveState is IEntityInteractionHandler stateInteraction)
        {
            return stateInteraction.GetHoverMessage();
        }

        return interactionModule.BuildPrompt("Talk");
    }

    public void InteractWithEntity()
    {
        if (entity.StateMachine.ActiveState is IEntityInteractionHandler stateInteraction)
        {
            stateInteraction.InteractWithEntity();
            return;
        }

        dialogueModule.StartConversation(brainData.GenericDialogue);
    }

    /*
        BUYER BEHAVIOR MAP

        General behavior:

        Note: buyer profile determines actual purchase behavior, the brain should responsible for higher-level
        decision making.

        - If the inventory is full, go to checkout.
        - If the NPC browses X amount of items in total, either go to checkout or leave (so the NPC doesn't keep
            browsing endlessly if there aren't items it's willing to buy based on its buyer profile) (this could
            be a serialized variable in the SO).
        - If there are less than X items remaining on the pawn shop floor, go to checkout or leave the store
            if the inventory is empty (this could also be a serialized variable in the SO).
        - Mix between finding random shelf items at any distance and nearby shelf items?
        - Can include some random wandering around, but should be used lightly.
        - After this NPC completes checkout, leave the store.

        Should this buyer also be able to move items around the pawn shop? E.g., pick up an item, later decide
        not to buy it, and place it somewhere else? This would have the benefit of making the pawnshop feel a bit
        more lively and if the player ends up constantly placing items on only 2-3 shelves and leaving the others
        empty because it's too much walking around to place items evenly, this could make it so that the shelves
        appear to be more full in general.
    */
}
