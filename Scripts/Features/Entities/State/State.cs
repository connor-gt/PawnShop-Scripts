using System;
using UnityEngine;

/// <summary>
/// Base class for all entity states. Each state encapsulates specific behavior and can optionally signal the
/// <see cref="State.OnEnded"/> event when they have completed their work.
/// </summary>
public abstract class State
{
    /// <summary>
    /// Invoked every time this state has <see cref="Enter()"/> called.
    /// </summary>
    public event Action OnEntered;
    
    /// <summary>
    /// Invoked exactly once when this state's lifetime has ended. Finite states typically invoke this when
    /// they complete their behavior, while indefinite states (like an unending idle) typically invoke it from
    /// <see cref="Exit()"/> when the <see cref="StateMachine"/> switches to a different state.
    /// </summary>
    public event Action<StateResult> OnEnded;

    /// <summary>
    /// Gets the state's finishing result. This starts off as <see cref="StateResult.Interrupted"/> in
    /// <see cref="Enter()"/> and should be explicitly changed by the inheriting state.
    /// </summary>
    public StateResult Result { get; private set; }

    /// <summary>
    /// The entity that owns and executes this state.
    /// </summary>
    protected Entity entity;

    /// <summary>
    /// Creates a new state bound to the given <see cref="entity"/>.
    /// </summary>
    /// <param name="entity">The entity this state will operate on.</param>
    public State(Entity entity)
    {
        this.entity = entity;
    }

    public virtual void Enter()
    {
        // Interrupted is the starting result because if the state is changed, the result returned should
        // be interrupted unless the state completed its behavior and changed the result to completed
        Result = StateResult.Interrupted;
        OnEntered?.Invoke();
    }

    public virtual void Update() { }
    public virtual void Exit() { }

    /// <summary>
    /// Notifies listeners that the state has completed its behavior.
    /// </summary>
    /// <param name="endingStateResult">The state's result as this method is called.</param>
    protected void NotifyEnded(StateResult endingStateResult)
    {
        Result = endingStateResult;
        OnEnded?.Invoke(Result);
    }
}
