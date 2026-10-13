public class IntroductionNPCBrain : Brain, IEntityInteractionHandler
{
    private readonly CounterInteractionState introductionState;
    private readonly TimedIdleState retryState;
    private readonly LeaveStoreState leaveStoreState;

    public IntroductionNPCBrain(Entity entity, IntroductionNPCBrainSO brainData, Counter sellCounter,
        EntityController entityController) : base(entity)
    {
        IntroductionCounterInteraction introductionInteraction = new IntroductionCounterInteraction(
            brainData.IntroductionDialogue,
            sellCounter,
            entity
        );

        introductionState = new CounterInteractionState(introductionInteraction, brainData.RotateSpeed);
        retryState = new TimedIdleState(entity, brainData.RetryDelay);
        leaveStoreState = new LeaveStoreState(entity, entityController);

        introductionState.OnEnded += OnIntroductionEnded;
        retryState.OnEnded += OnRetryEnded;
    }

    public override void StartThinking()
    {
        TransitionTo(introductionState);
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

    private void OnIntroductionEnded(StateResult result)
    {
        if (result.ResultType == StateResultType.Completed)
        {
            TransitionTo(leaveStoreState);
        }
        else
        {
            // Keep trying until the guide can reach the counter so the task remains possible. In normal gameplay
            // this shouldn't matter though since this NPC will be here day 1 opening when no other NPCs are active
            TransitionTo(retryState);
        }
    }

    private void OnRetryEnded(StateResult result)
    {
        TransitionTo(introductionState);
    }
}
