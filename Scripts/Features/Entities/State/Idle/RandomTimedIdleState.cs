using UnityEngine;

/// <summary>
/// An idle state that remains active for a randomly selected duration within a specified range.
/// </summary>
public class RandomTimedIdleState : IdleState
{
    public readonly float MinIdleTime;
    public readonly float MaxIdleTime;

    /// <summary>
    /// Gets the random idle time that this state will be idling for.
    /// </summary>
    public float IdleTime { get; private set; }

    public RandomTimedIdleState(Entity entity, float minIdleTime, float maxIdleTime) : base(entity)
    {
        if (minIdleTime > maxIdleTime)
        {
            (minIdleTime, maxIdleTime) = (maxIdleTime, minIdleTime);
        }

        MinIdleTime = minIdleTime;
        MaxIdleTime = maxIdleTime;
    }

    public override void Enter()
    {
        base.Enter();
        
        IdleTime = Random.Range(MinIdleTime, MaxIdleTime);
        entity.Invoke(() => NotifyEnded(StateResult.Completed), IdleTime);
    }
}
