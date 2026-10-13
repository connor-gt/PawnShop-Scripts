using UnityEngine;

public class SimpleBrowseBrain : Brain, IShopper
{
    public ShelfItem CurrentShelfItem { get; set; }

    private readonly SimpleBrowseBrainSO brainData;
    private readonly ShelfController shelfController;

    private readonly ShopperService shopperService;

    private readonly MoveToPositionState moveToPositionState;
    private readonly RandomTimedIdleState randomTimedIdleState;

    public SimpleBrowseBrain(Entity entity, SimpleBrowseBrainSO brainData, ShelfController shelfController)
        : base(entity)
    {
        this.brainData = brainData;
        this.shelfController = shelfController;

        shopperService = new ShopperService(entity, this);

        moveToPositionState = new MoveToPositionState(entity);
        randomTimedIdleState = new RandomTimedIdleState(
            entity,
            brainData.IdleTimes.Min,
            brainData.IdleTimes.Max
        );

        randomTimedIdleState.OnEnded += OnIdleEnded;
        moveToPositionState.OnEnded += OnMoveToPositionEnded;
    }

    public override void StartThinking()
    {
        TransitionTo(randomTimedIdleState);
    }

    private void OnIdleEnded(StateResult result)
    {
        CurrentShelfItem = shelfController.GetRandomShelfItem(requiresAvailability: true);
        
        if (CurrentShelfItem != null && CurrentShelfItem.InteractionPoint.TryReserve(entity))
        {
            moveToPositionState.SetTarget(CurrentShelfItem.InteractionPoint.transform);
        }
        else
        {
            moveToPositionState.SetTarget(Vector3.zero);
        }

        TransitionTo(moveToPositionState);
    }

    private void OnMoveToPositionEnded(StateResult result)
    {
        if (CurrentShelfItem?.Holdable != null)
        {
            randomTimedIdleState.SetLookTarget(CurrentShelfItem.Holdable.transform, brainData.RotateSpeed);
        }

        TransitionTo(randomTimedIdleState);
        shopperService.ScheduleShelfItemRelease(randomTimedIdleState.IdleTime);
    }
}
