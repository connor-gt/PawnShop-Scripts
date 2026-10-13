using System;
using UnityEngine.InputSystem;

/// <summary>
/// Represents a temporary set of input bindings that can be enabled or disabled as a group.
/// </summary>
public class TemporaryInputBinding
{
    private readonly InputAction[] inputs;
    private readonly Action<InputAction.CallbackContext>[] actions;

    /// <summary>
    /// Gets whether or not the inputs have been subscribed to the actions and are enabled.
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Creates a new TemporaryInputBinding using the provided input action and its associated callback. The input
    /// is not enabled by default, so to enable it <see cref="Enable"/> must be called.
    /// </summary>
    /// <param name="input">The input action to bind.</param>
    /// <param name="action">The callback to invoke when the action is performed.</param>
    public TemporaryInputBinding(InputAction input, Action<InputAction.CallbackContext> action)
        : this(new InputAction[] { input }, new Action<InputAction.CallbackContext>[] { action }) { }

    /// <summary>
    /// Creates a new TemporaryInputBinding using the provided input actions and their associated callbacks. Both
    /// arrays must be the same length, with each action corresponding to the callback at the same index. The inputs
    /// are not enabled by default, so to enable them <see cref="Enable"/> must be called.
    /// </summary>
    /// <param name="inputs">The input actions to bind.</param>
    /// <param name="actions">The callbacks to invoke when each action is performed.</param>
    /// <exception cref="ArgumentException">Thrown when the inputs and actions arrays do not have matching lengths.
    /// </exception>
    public TemporaryInputBinding(InputAction[] inputs, Action<InputAction.CallbackContext>[] actions)
    {
        if (inputs.Length != actions.Length)
        {
            throw new ArgumentException($"TemporaryInputBinding: inputs and actions arguments must be the same length.");
        }

        this.inputs = inputs;
        this.actions = actions;
    }

    /// <summary>
    /// Enables all temporary bindings by subscribing each callback to the corresponding input action's performed event.
    /// </summary>
    public void Enable()
    {
        if (IsEnabled)
        {
            return;
        }

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].performed += actions[i];
        }

        IsEnabled = true;
    }

    /// <summary>
    /// Disables all temporary bindings by unsubscribing each callback from the corresponding input action's performed
    /// event.
    /// </summary>
    public void Disable()
    {
        if (!IsEnabled)
        {
            return;
        }

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i].performed -= actions[i];
        }

        IsEnabled = false;
    }

    /// <summary>
    /// Gets whether the specified input action is currently being held down.
    /// <para>
    /// This method will always return false if this binding is disabled or if the specified input action is not
    /// part of this binding.
    /// </para>
    /// </summary>
    /// <param name="input">The input action to check.</param>
    /// <returns>True if the input is currently pressed, false if otherwise.</returns>
    public bool IsInputBeingHeld(InputAction input)
    {
        if (!IsEnabled)
        {
            return false;
        }

        int index = GetIndexOfInputAction(input);
        return index != -1 && inputs[index].IsPressed();
    }

    /// <summary>
    /// Gets the index of the specified input action within this binding.
    /// </summary>
    /// <param name="input">The input action to search for.</param>
    /// <returns>The index of the input action or <c>-1</c> if it does not exist within this binding.</returns>
    private int GetIndexOfInputAction(InputAction input)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == input)
            {
                return i;
            }
        }

        return -1;
    }
}
