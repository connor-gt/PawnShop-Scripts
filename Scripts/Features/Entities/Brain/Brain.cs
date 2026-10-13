/// <summary>
/// The base class for all runtime <see cref="Brain"/> logic. Brains control an entity's behavior and are
/// updated every frame by the Entity component.
/// </summary>
public abstract class Brain
{
    protected Entity entity;

    /// <summary>
    /// Creates a new <see cref="Brain"/> instance bound to the given entity.
    /// </summary>
    /// <param name="entity">The entity this <see cref="Brain"/> will control.</param>
    public Brain(Entity entity)
    {
        this.entity = entity;
    }

    /// <summary>
    /// Acts as a Start() method for <see cref="Brain"/>.
    /// </summary>
    public abstract void StartThinking();

    /// <summary>
    /// Called every frame to update logic-based behavior.
    /// </summary>
    public virtual void LogicUpdate()
    {
        entity.StateMachine.ActiveState.Update();
    }

    /// <summary>
    /// Called every fixed frame to update physics-based behavior.
    /// </summary>
    public virtual void PhysicsUpdate() { }

    /// <summary>
    /// Enter a new state.
    /// </summary>
    /// <param name="newState">The state to enter.</param>
    protected void TransitionTo(State newState)
    {
        entity.StateMachine.ChangeState(newState);
    }

    /// <summary>
    /// Check whether or not the <see cref="StateMachine.ActiveState"/> is equal to the argument state.
    /// </summary>
    /// <param name="state">The state to check.</param>
    /// <returns>True if the argument state is equal to <see cref="StateMachine.ActiveState"/>, false if
    /// otherwise.</returns>
    protected bool IsInState(State state)
    {
        return entity.StateMachine.ActiveState.Equals(state);
    }
}
