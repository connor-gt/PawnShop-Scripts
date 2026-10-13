using UnityEngine;

/// <summary>
/// An idle state that rotates the entity towards a specified target and optionally waits for a delay after
/// facing the target before completing.
/// </summary>
public class RotateTowardsTargetState : IdleState
{
    /// <summary>
    /// Gets the delay time that the entity will wait for after rotating to the target.
    /// </summary>
    public readonly float Delay;
    private float delayTimer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RotateTowardsTargetState"/> class.
    /// </summary>
    /// <param name="entity">The entity controlled by this state.</param>
    /// <param name="rotationSpeed">The speed at which the entity rotates towards the target.</param>
    /// <param name="delay">The amount of time, in seconds, to wait after the entity is facing the target before
    /// completing.</param>
    public RotateTowardsTargetState(Entity entity, float rotationSpeed, float delay) : base(entity)
    {
        this.rotationSpeed = rotationSpeed;
        Delay = delay;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RotateTowardsTargetState"/> class with no delay after
    /// facing the target.
    /// </summary>
    /// <param name="entity">The entity controlled by this state.</param>
    /// <param name="rotationSpeed">The speed at which the entity rotates towards the target.</param>
    public RotateTowardsTargetState(Entity entity, float rotationSpeed) : this(entity, rotationSpeed, 0f) { }

    public override void Enter()
    {
        base.Enter();
        delayTimer = 0f;
    }

    /// <summary>
    /// Set a <see cref="Transform"/> target to look at while idling. This is reset every time the state exits
    /// so this must be called before the state is entered every time the <see cref="Entity"/> that's idling
    /// should be rotated towards a target.
    /// </summary>
    /// <param name="lookTarget">The target to look at.</param>
    public void SetLookTarget(Transform lookTarget)
    {
        this.lookTarget = lookTarget;
    }

    public override void Update()
    {
        base.Update();

        if (lookTarget == null)
        {
            NotifyEnded(StateResult.InternalCancel(StateCancelReason.MissingTarget));
        }
        else if (entity.transform.IsFacingTarget(lookTarget))
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > Delay)
            {
                NotifyEnded(StateResult.Completed);
            }
        }
    }
}
