using UnityEngine;

public class CounterInteractionState : State, IEntityInteractionHandler
{
    /// <summary>
    /// Gets the amount of time the <see cref="Entity"/> has been waiting in the queue.
    /// </summary>
    public float QueueWaitTime { get; private set; }

    /// <summary>
    /// Gets the amount of time the <see cref="Entity"/> has been waiting at the front of the counter.
    /// </summary>
    public float CounterWaitTime { get; private set; }

    public enum CounterPhase
    {
        /// <summary>
        /// The <see cref="Entity"/> is waiting in line to get to the front.
        /// </summary>
        Queueing,

        /// <summary>
        /// The <see cref="Entity"/> has reached the front of the counter.
        /// </summary>
        ReachedFront,

        /// <summary>
        /// The <see cref="Entity"/> is currently at the front counter. It is during this time that the
        /// injected <see cref="CounterInteraction"/>'s logic is running.
        /// </summary>
        AtFrontCounter
    }

    /// <summary>
    /// Gets the current phase of the queue/counter interaction process.
    /// </summary>
    public CounterPhase Phase { get; private set; }

    private readonly NavigationModule navigationModule;

    private readonly CounterInteraction counterInteraction;
    private readonly Counter counter;
    private InteractionPoint currentAssignedPoint;

    /// <summary>
    /// Gets the rotation speed that the <see cref="Entity"/> has while standing at the front counter or
    /// waiting in line.
    /// </summary>
    private readonly float rotationSpeed;

    /// <summary>
    /// Creates a new state that guides an entity through a counter queue and hosts the specified
    /// <see cref="CounterInteraction"/> once the entity reaches the front of the counter.
    /// </summary>
    /// <param name="counterInteraction">The interaction to host after the entity reaches the front of the
    /// queue. This also determines which <see cref="Counter"/> the entity will queue at.</param>
    /// <param name="rotationSpeed">The speed at which the entity rotates to face the <see cref="Counter"/>.
    /// </param>
    public CounterInteractionState(CounterInteraction counterInteraction, float rotationSpeed)
        : base(counterInteraction.Entity)
    {
        navigationModule = entity.GetComponent<NavigationModule>();

        counter = counterInteraction.Counter;
        this.counterInteraction = counterInteraction;
        this.rotationSpeed = rotationSpeed;
    }

    public override void Enter()
    {
        base.Enter();

        Phase = CounterPhase.Queueing;
        QueueWaitTime = 0f;
        CounterWaitTime = 0f;
        counterInteraction.OnFinished += OnCounterInteractionExit;

        if (counter.TryJoinQueue(entity, out InteractionPoint assignedPoint))
        {
            navigationModule.SetDestination(assignedPoint.transform);
            currentAssignedPoint = assignedPoint;
        }
        else
        {
            NotifyEnded(StateResult.InternalCancel(StateCancelReason.TargetUnavailable));
        }
    }

    public override void Update()
    {
        switch (Phase)
        {
            case CounterPhase.Queueing:
                Queueing();
                break;
            case CounterPhase.ReachedFront:
                Phase = CounterPhase.AtFrontCounter;
                ReachedFront();
                break;
            case CounterPhase.AtFrontCounter:
                AtFrontCounter();
                break;
        }
    }

    private void Queueing()
    {
        QueueWaitTime += Time.deltaTime;
        bool isAtFront = counter.IsAtFront(entity);
        if (isAtFront)
        {
            Phase = CounterPhase.ReachedFront;
        }
        else if (!isAtFront && counter.TryAdvanceQueue(entity, out InteractionPoint nextPoint))
        {
            navigationModule.SetDestination(nextPoint.transform);
            currentAssignedPoint = nextPoint;
        }
        
        // Make sure the entity is facing the counter's direction when standing still
        if (navigationModule.HasReachedDestination())
        {
            entity.transform.RotateTowardsTarget(counter.transform, rotationSpeed);
        }
    }

    private void ReachedFront()
    {
        counterInteraction.Begin();
    }

    private void AtFrontCounter()
    {
        CounterWaitTime += Time.deltaTime;
        entity.transform.RotateTowardsTarget(counter.transform, rotationSpeed);
        counterInteraction.LogicUpdate();
    }

    public string GetHoverMessage()
    {
        return counterInteraction.IsActive ? counterInteraction.GetHoverMessage() : "";
    }

    public void InteractWithEntity()
    {
        if (counterInteraction.IsActive)
        {
            counterInteraction.InteractWithEntity();
        }
    }

    private void OnCounterInteractionExit()
    {
        NotifyEnded(StateResult.Completed);
    }

    public override void Exit()
    {
        counterInteraction.OnFinished -= OnCounterInteractionExit;

        if (currentAssignedPoint != null)
        {
            currentAssignedPoint.Release();
            currentAssignedPoint = null;
        }

        base.Exit();
    }
}
