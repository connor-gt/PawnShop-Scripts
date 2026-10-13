using Unity.VisualScripting;
using UnityEngine;

public class SimpleSellItemsBrain : Brain, IEntityInteractionHandler
{
    private readonly SimpleSellItemsBrainSO brainData;

    private readonly TimedIdleState timedIdleState;
    private readonly SellItemsInteraction sellItemsInteraction;
    private readonly CounterInteractionState sellItemsState;
    private readonly MoveToRandomPositionState moveToRandomPositionState;
    private readonly LeaveStoreState leaveStoreState;

    private readonly InventoryModule inventoryModule;
    private readonly InteractionModule interactionModule;
    private readonly DialogueModule dialogueModule;

    private readonly SellerService sellerService;

    public SimpleSellItemsBrain(Entity entity, SimpleSellItemsBrainSO brainData, SellerProfileModule sellerProfileModule,
        SellerInventorySO sellerInventory, EntityController entityController, Counter sellCounter) : base(entity)
    {
        this.brainData = brainData;

        inventoryModule = entity.GetComponent<InventoryModule>();
        interactionModule = entity.GetComponent<InteractionModule>();
        dialogueModule = entity.GetComponent<DialogueModule>();

        // NOTE: this service should ALWAYS be created and call GenerateInventory() before the sell items interaction, 
        // because the sell items interaction immediately uses the entity's inventory to generate the NPC's asking price

        // TODO: check if this still needs to be ordered like this after moving the negotiation session creation outside
        // of the constructor
        sellerService = new SellerService(inventoryModule, sellerInventory);
        sellerService.GenerateInventory();

        timedIdleState = new TimedIdleState(entity, brainData.IdleTime);
        sellItemsInteraction = new SellItemsInteraction(
            brainData.NegotiationDialogue,
            sellCounter,
            entity,
            sellerProfileModule
        );
        sellItemsState = new CounterInteractionState(sellItemsInteraction, brainData.RotateSpeed);
        moveToRandomPositionState = new MoveToRandomPositionState(entity, brainData.WanderRange);
        leaveStoreState = new LeaveStoreState(entity, entityController);

        sellItemsState.OnEnded += OnSellItemsEnded;
        moveToRandomPositionState.OnEnded += OnMoveToRandomPositionEnded;
        timedIdleState.OnEnded += OnTimedIdleEnded;
    }

    public override void StartThinking()
    {
        TransitionTo(sellItemsState);
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

    private void OnSellItemsEnded(StateResult result)
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

    private void OnMoveToRandomPositionEnded(StateResult result)
    {
        TransitionTo(timedIdleState);
    }

    private void OnTimedIdleEnded(StateResult result)
    {
        TransitionTo(sellItemsState);
    }
}
