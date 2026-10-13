using UnityEngine;

/// <summary>
/// A state that moves the entity to a randomly selected position within a specified range.
/// </summary>
public class MoveToRandomPositionState : State
{
    /// <summary>
    /// Gets the maximum distance from the entity's starting position within which a random destination can be
    /// selected.
    /// </summary>
    public float Range { get; }

    private readonly NavigationModule navigationModule;

    public MoveToRandomPositionState(Entity entity, float range) : base(entity)
    {
        Range = range;
        navigationModule = entity.GetComponent<NavigationModule>();
    }

    public override void Enter()
    {
        base.Enter();

        if (navigationModule.GetRandomPointInRange(entity.transform.position, Range, out Vector3 randomPoint))
        {
            navigationModule.SetDestination(randomPoint);
        }
        else
        {
            NotifyEnded(StateResult.InternalCancel(StateCancelReason.PathNotFound));
        }
    }

    public override void Update()
    {
        if (navigationModule.HasReachedDestination())
        {
            NotifyEnded(StateResult.Completed);
        }
    }
}
