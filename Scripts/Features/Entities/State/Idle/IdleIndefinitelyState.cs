/// <summary>
/// An entity that enters this state will remain in this state (doing nothing) until an external system changes
/// the entity's state.
/// </summary>
public class IdleIndefinitelyState : IdleState
{
    public IdleIndefinitelyState(Entity entity) : base(entity) { }

    public override void Exit()
    {
        base.Exit();
        NotifyEnded(StateResult.Interrupted);
    }
}
