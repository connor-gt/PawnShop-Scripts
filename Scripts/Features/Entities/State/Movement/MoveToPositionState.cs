using UnityEngine;

/// <summary>
/// A state that moves the entity to a specified world position.
/// <para>
/// <see cref="SetTarget"/> needs to be called before the entity enters this state.
/// </para>
/// </summary>
public class MoveToPositionState : State
{
    private Vector3? targetPosition = null;

    private readonly NavigationModule navigationModule;

    public MoveToPositionState(Entity entity) : base(entity)
    {
        navigationModule = entity.GetComponent<NavigationModule>();
    }

    /// <summary>
    /// Sets the target position to move to. This must be called before the state is entered.
    /// </summary>
    /// <param name="targetTransform">The transform of the target position.</param>
    public void SetTarget(Transform targetTransform)
    {
        SetTarget(targetTransform.position);
    }

    /// <summary>
    /// Sets the target position to move to. This must be called before the state is entered.
    /// </summary>
    /// <param name="targetPosition">The position to move to.</param>
    public void SetTarget(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public override void Enter()
    {
        base.Enter();

        if (targetPosition == null)
        {
            NotifyEnded(StateResult.InternalCancel(StateCancelReason.MissingTarget));
            return;
        }

        navigationModule.SetDestination(targetPosition.Value);
    }

    public override void Update()
    {
        if (navigationModule.HasReachedDestination())
        {
            NotifyEnded(StateResult.Completed);
        }
    }

    public override void Exit()
    {
        targetPosition = null;
        base.Exit();
    }
}