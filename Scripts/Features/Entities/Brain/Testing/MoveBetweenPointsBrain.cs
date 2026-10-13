using UnityEngine;

public class MoveBetweenPointsBrain : Brain
{
    private readonly RandomTimedIdleState randomTimedIdleState;
    private readonly MoveToPositionState moveToPositionState;

    private readonly MoveBetweenPointsModule locationsModule;

    public MoveBetweenPointsBrain(Entity entity, MoveBetweenPointsBrainSO brainData) : base(entity)
    {
        locationsModule = entity.GetComponent<MoveBetweenPointsModule>();

        randomTimedIdleState = new RandomTimedIdleState(entity, brainData.IdleTimes.Min, brainData.IdleTimes.Max);
        moveToPositionState = new MoveToPositionState(entity);

        randomTimedIdleState.OnEnded += OnIdleEnded;
        moveToPositionState.OnEnded += OnMoveEnded;
    }

    public override void StartThinking()
    {
        TransitionTo(randomTimedIdleState);
    }

    private void OnIdleEnded(StateResult result)
    {
        Transform destination = locationsModule.GetNext();
        moveToPositionState.SetTarget(destination);
        TransitionTo(moveToPositionState);
    }

    private void OnMoveEnded(StateResult result)
    {
        TransitionTo(randomTimedIdleState);
    }
}
