/// <summary>
/// An idle state that remains active for a specified duration before completing.
/// </summary>
public class TimedIdleState : IdleState
{
    /// <summary>
    /// Gets or sets the idle time that this state will be idling for.
    /// </summary>
    public float IdleTime { get; set; }

    public TimedIdleState(Entity entity, float idleTime) : base(entity)
    {
        IdleTime = idleTime;
    }

    public override void Enter()
    {
        base.Enter();
        entity.Invoke(() => NotifyEnded(StateResult.Completed), IdleTime);
    }
}
