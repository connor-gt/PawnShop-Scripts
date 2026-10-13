/// <summary>
/// Manages a single active <see cref="State"/>.
/// </summary>
public class StateMachine
{
    private State _activeState = null;
    public State ActiveState => _activeState;

    /// <summary>
    /// Initializes the state machine with the given starting state.
    /// </summary>
    /// <param name="startingState">The state to initialize the state machine with.</param>
    public void Initialize(State startingState)
    {
        _activeState = startingState;
        _activeState?.Enter();
    }

    /// <summary>
    /// Changes the active state to a new state.
    /// </summary>
    /// <param name="newState">The state to change to.</param>
    public void ChangeState(State newState)
    {
        _activeState?.Exit();
        _activeState = newState;
        _activeState?.Enter();
    }
}
