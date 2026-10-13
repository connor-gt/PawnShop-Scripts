using UnityEngine;

public class SimpleShoppingBrain : Brain, IEntityInteractionHandler, IShopper, IExitRequester
{
    public ShelfItem CurrentShelfItem { get; set; }

    private readonly SimpleShoppingBrainSO brainData;
    private readonly ShelfController shelfController;

    private readonly BuyerService buyerService;
    private readonly ShopperService shopperService;

    private readonly TimedIdleState timedIdleState;
    private readonly RotateTowardsTargetState rotateTowardsTargetState;
    private readonly GrabItemState grabItemState;
    private readonly MoveToPositionState moveToPositionState;
    private readonly MoveToRandomPositionState moveToRandomPositionState;
    private readonly CheckoutInteraction checkoutInteraction;
    private readonly CounterInteractionState checkoutState;
    private readonly LeaveStoreState leaveStoreState;

    private readonly InventoryModule inventoryModule;
    private readonly InteractionModule interactionModule;
    private readonly DialogueModule dialogueModule;

    public SimpleShoppingBrain(Entity entity, SimpleShoppingBrainSO brainData, BuyerProfileModule buyerProfileModule,
        ShelfController shelfController, EntityController entityController, Counter purchaseCounter) : base(entity)
    {
        this.brainData = brainData;
        this.shelfController = shelfController;

        inventoryModule = entity.GetComponent<InventoryModule>();
        interactionModule = entity.GetComponent<InteractionModule>();
        dialogueModule = entity.GetComponent<DialogueModule>();

        buyerService = new BuyerService(buyerProfileModule);
        shopperService = new ShopperService(entity, this);

        timedIdleState = new TimedIdleState(entity, brainData.IdleTime);
        grabItemState = new GrabItemState(entity);
        moveToPositionState = new MoveToPositionState(entity);
        moveToRandomPositionState = new MoveToRandomPositionState(entity, brainData.WanderRange);
        checkoutInteraction = new CheckoutInteraction(purchaseCounter, entity);
        checkoutState = new CounterInteractionState(checkoutInteraction, brainData.RotateSpeed);
        leaveStoreState = new LeaveStoreState(entity, entityController);
        rotateTowardsTargetState = new RotateTowardsTargetState(
            entity,
            brainData.RotateSpeed,
            brainData.DelayTimeAfterRotating
        );

        timedIdleState.OnEnded += OnIdleEnded;
        rotateTowardsTargetState.OnEnded += OnRotateEnded;
        grabItemState.OnEnded += OnGrabItemEnded;
        moveToPositionState.OnEnded += OnMoveToPositionEnded;
        moveToRandomPositionState.OnEnded += OnMoveToRandomPositionEnded;
        checkoutState.OnEnded += OnCheckoutEnded;
    }

    public override void StartThinking()
    {
        TransitionTo(timedIdleState);
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

    private void OnIdleEnded(StateResult result)
    {
        if (inventoryModule.IsFull || shelfController.GetTotalItemCount() < 1)
        {
            TransitionTo(checkoutState);
            return;
        }
        
        FindNewItem();
    }

    private void OnMoveToPositionEnded(StateResult result)
    {
        if (CurrentShelfItem?.Holdable != null)
        {
            rotateTowardsTargetState.SetLookTarget(CurrentShelfItem.Holdable.transform);
        }

        TransitionTo(rotateTowardsTargetState);
    }

    private void OnRotateEnded(StateResult result)
    {
        if (CurrentShelfItem?.Holdable != null && buyerService.ShouldBuy(CurrentShelfItem?.Holdable))
        {
            grabItemState.SetTarget(CurrentShelfItem.Holdable);
            TransitionTo(grabItemState);
        }
        else
        {
            CurrentShelfItem?.InteractionPoint.Release();
            FindNewItem();
        }
    }

    private void OnGrabItemEnded(StateResult result)
    {
        TransitionTo(timedIdleState);
        shopperService.ScheduleShelfItemRelease(timedIdleState.IdleTime);
    }

    private void OnMoveToRandomPositionEnded(StateResult result)
    {
        TransitionTo(timedIdleState);
    }

    private void OnCheckoutEnded(StateResult result)
    {
        if (result.ResultType == StateResultType.Completed)
        {
            TransitionTo(leaveStoreState);
        }
        else
        {
            TransitionTo(moveToRandomPositionState);
        }
    }

    public void WrapUpAndLeave()
    {
        if (inventoryModule.Count == 0)
        {
            TransitionTo(leaveStoreState);
        }
        else if (!IsInState(checkoutState))
        {
            TransitionTo(checkoutState);
        }
    }

    private void FindNewItem()
    {
        CurrentShelfItem = shelfController.GetRandomShelfItem(requiresAvailability: true);

        if (CurrentShelfItem != null && CurrentShelfItem.InteractionPoint.TryReserve(entity))
        {
            moveToPositionState.SetTarget(CurrentShelfItem.InteractionPoint.transform);
            TransitionTo(moveToPositionState);
        }
        else
        {
            TransitionTo(moveToRandomPositionState);
        }
    }
}
