using UnityEngine.InputSystem;

/// <summary>
/// Defines an action that can be performed while an item is being held.
/// </summary>
public interface IHoldAction
{
    /// <summary>
    /// Gets the asset containing this action's input and display configuration.
    /// </summary>
    HoldActionInputSO HoldActionInput { get; }

    /// <summary>
    /// Gets the temporary input binding used to perform this action.
    /// </summary>
    TemporaryInputBinding InputBinding { get; }
}
