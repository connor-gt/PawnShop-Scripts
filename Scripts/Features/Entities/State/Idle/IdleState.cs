using UnityEngine;

/// <summary>
/// Provides a base idle state that optionally rotates the entity towards a specified target that child idle
/// states can use.
/// </summary>
public abstract class IdleState : State
{
    protected Transform lookTarget;
    protected float rotationSpeed;

    public IdleState(Entity entity) : base(entity) { }

    /// <summary>
    /// Set a <see cref="Transform"/> target to look at while idling. This is reset every time the state exits
    /// so this must be called before the state is entered every time the <see cref="Entity"/> that's idling
    /// should be rotated towards a target.
    /// </summary>
    /// <param name="lookTarget">The target to look at.</param>
    /// <param name="rotationSpeed">The rotation speed.</param>
    public void SetLookTarget(Transform lookTarget, float rotationSpeed)
    {
        this.lookTarget = lookTarget;
        this.rotationSpeed = rotationSpeed;
    }

    public override void Update()
    {
        if (lookTarget != null)
        {
            entity.transform.RotateTowardsTarget(lookTarget, rotationSpeed);
        }
    }
    
    public override void Exit()
    {
        lookTarget = null;
        base.Exit();
    }
}
