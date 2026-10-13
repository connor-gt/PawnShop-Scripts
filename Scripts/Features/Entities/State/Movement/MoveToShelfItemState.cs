using UnityEngine;

/// <summary>
/// A state that moves the entity to the interaction point of a specified <see cref="ShelfItem"/>.
/// <para>
/// <see cref="SetTargetShelfItem"/> needs to be called before the entity enters this state.
/// </para>
/// </summary>
public class MoveToShelfItemState : State
{
    private ShelfItem targetShelfItem;

    private readonly NavigationModule navigationModule;

    public MoveToShelfItemState(Entity entity) : base(entity)
    {
        navigationModule = entity.GetComponent<NavigationModule>();
    }

    /// <summary>
    /// Sets the target <see cref="ShelfItem"/> this state will attempt to move to its
    /// <see cref="ShelfItem.InteractionPoint"/>. This should be called before the state is entered.
    /// </summary>
    /// <param name="targetShelfItem">The <see cref="ShelfItem"/> whose interaction point the entity should
    /// move to.</param>
    public void SetTargetShelfItem(ShelfItem targetShelfItem)
    {
        this.targetShelfItem = targetShelfItem;
    }

    public override void Enter()
    {
        base.Enter();

        if (targetShelfItem == null)
        {
            NotifyEnded(StateResult.InternalCancel(StateCancelReason.MissingTarget));
            return;
        }

        navigationModule.SetDestination(targetShelfItem.InteractionPoint.transform);
    }

    public override void Update()
    {
        if (navigationModule.HasReachedDestination())
        {
            NotifyEnded(StateResult.Completed);
        }
        else if (targetShelfItem.Holdable == null)
        {
            NotifyEnded(StateResult.InternalCancel(StateCancelReason.TargetUnavailable));
        }
    }

    public override void Exit()
    {
        targetShelfItem = null;
        base.Exit();
    }
}
