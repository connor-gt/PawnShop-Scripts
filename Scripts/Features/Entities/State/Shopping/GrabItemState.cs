using UnityEngine;

/// <summary>
/// A state that entities can use to grab holdables.
/// <para>
/// <see cref="SetTarget"/> must be called before the entity enters this state.
/// </para>
/// </summary>
public class GrabItemState : State
{
    private Holdable targetHoldable;

    private readonly InventoryModule inventoryModule;

    public GrabItemState(Entity entity) : base(entity)
    {
        inventoryModule = entity.GetComponent<InventoryModule>();
    }

    /// <summary>
    /// Set the target <see cref="Holdable"/> this state will attempt to add to the entity's inventory. This
    /// should be called before transitioning to this state and must be called every time, as the target item
    /// gets reset to null when this state exits.
    /// </summary>
    /// <param name="targetHoldable">The <see cref="Holdable"/> to grab.</param>
    public void SetTarget(Holdable targetHoldable)
    {
        this.targetHoldable = targetHoldable;
    }

    public override void Enter()
    {
        base.Enter();

        StateResult result;

        if (targetHoldable != null && !inventoryModule.IsFull)
        {
            targetHoldable.ConsumeWorldInstance();
            inventoryModule.Add(targetHoldable.Item);
            result = StateResult.Completed;
        }
        else if (targetHoldable == null)
        {
            result = StateResult.InternalCancel(StateCancelReason.MissingTarget);
        }
        else if (inventoryModule.IsFull)
        {
            result = StateResult.InternalCancel(StateCancelReason.InventoryFull);
        }
        else
        {
            result = StateResult.InternalCancel(StateCancelReason.Other);
        }

        NotifyEnded(result);
    }

    public override void Exit()
    {
        targetHoldable = null;
        base.Exit();
    }
}
