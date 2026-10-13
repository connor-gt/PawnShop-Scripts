using System.Linq;

public class GenericSellerBrain : Brain, IEntityInteractionHandler, IExitRequester
{
    private readonly GenericSellerBrainSO brainData;

    private readonly SellerService sellerService;

    private readonly InventoryModule inventoryModule;
    private readonly InteractionModule interactionModule;

    private readonly RandomTimedIdleState randomTimedIdleState;
    private readonly MoveToShelfItemState moveToShelfItemState;
    private readonly CounterInteractionState sellItemsState;
    private readonly SellItemsInteraction sellItemsInteraction;
    private readonly LeaveStoreState leaveStoreState;

    private readonly ShelfController shelfController;

    public GenericSellerBrain(Entity entity, GenericSellerBrainSO brainData, SellerProfileModule sellerProfileModule,
        Counter counter, ShelfController shelfController, EntityController entityController) : base(entity)
    {
        this.brainData = brainData;
        this.shelfController = shelfController;

        inventoryModule = entity.GetComponent<InventoryModule>();
        interactionModule = entity.GetComponent<InteractionModule>();

        sellerService = new SellerService(inventoryModule, sellerProfileModule.Inventory);
        sellerService.GenerateInventory();

        randomTimedIdleState = new RandomTimedIdleState(
            entity,
            brainData.RandomIdleTime.Min,
            brainData.RandomIdleTime.Max
        );
        moveToShelfItemState = new MoveToShelfItemState(entity);
        sellItemsInteraction = new SellItemsInteraction(
            brainData.NegotiationDialogue,
            counter,
            entity,
            sellerProfileModule
        );
        sellItemsState = new CounterInteractionState(sellItemsInteraction, brainData.RotateSpeed);
        leaveStoreState = new LeaveStoreState(entity, entityController);


        sellItemsState.OnEnded += OnSellItemsEnded;
    }

    public override void StartThinking()
    {
        // Begin the selling interaction. Shelf fallback behavior is still being developed.

        TransitionTo(sellItemsState);
    }

    public string GetHoverMessage()
    {
        if (entity.StateMachine.ActiveState is IEntityInteractionHandler stateInteraction)
        {
            return stateInteraction.GetHoverMessage();
        }

        return "";
    }

    public void InteractWithEntity()
    {
        if (entity.StateMachine.ActiveState is IEntityInteractionHandler stateInteraction)
        {
            stateInteraction.InteractWithEntity();
        }
    }

    public void WrapUpAndLeave()
    {
        if (!IsInState(sellItemsState))
        {
            LeaveStore();
        }
    }

    private void OnSellItemsEnded(StateResult result)
    {
        if (result.ResultType == StateResultType.Completed)
        {
            LeaveStore();
        }
        else
        {
            FindAndMoveToEmptyShelfLocation();
        }
    }

    // Empty shelf locations are used to not take up interaction points for buyers who are actually looking
    // for items
    private void FindAndMoveToEmptyShelfLocation()
    {
        Shelf randomShelf = shelfController.GetRandomShelf();
    }

    private void LeaveStore()
    {
        TransitionTo(leaveStoreState);
    }
}
