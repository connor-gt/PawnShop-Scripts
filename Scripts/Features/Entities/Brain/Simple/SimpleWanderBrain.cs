public class SimpleWanderBrain : Brain
{
    private readonly RandomTimedIdleState randomTimedIdleState;
    private readonly MoveToRandomPositionState moveToRandomPositionState;

    public SimpleWanderBrain(Entity entity, SimpleWanderBrainSO brainData) : base(entity)
    {
        randomTimedIdleState = new RandomTimedIdleState(
            entity,
            brainData.IdleTimes.Min,
            brainData.IdleTimes.Max
        );

        moveToRandomPositionState = new MoveToRandomPositionState(entity, brainData.WanderRange);

        randomTimedIdleState.OnEnded += OnIdleEnded;
        moveToRandomPositionState.OnEnded += OnMoveEnded;
    }

    public override void StartThinking()
    {
        TransitionTo(randomTimedIdleState);
    }

    private void OnIdleEnded(StateResult result)
    {
        TransitionTo(moveToRandomPositionState);
    }

    private void OnMoveEnded(StateResult result)
    {
        TransitionTo(randomTimedIdleState);
    }
}
