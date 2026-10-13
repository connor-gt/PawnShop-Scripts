using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerActions _actions;
    
    /// <summary>
    /// Gets the main input actions asset.
    /// </summary>
    public PlayerActions Actions
    {
        get
        {
            if (_actions == null)
            {
                _actions = new PlayerActions();
                _actions.Enable();
            }

            return _actions;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Enables or disables the specified <see cref="InputActionMap"/> based on the provided flag.
    /// </summary>
    /// <param name="map">The input action map to modify.</param>
    /// <param name="enabled">If true, the map is enabled. If false, the map is disabled.</param>
    public void EnableMap(InputActionMap map, bool enabled)
    {
        if (enabled)
        {
            map.Enable();
        }
        else
        {
            map.Disable();
        }
    }

    /// <summary>
    /// Returns a display string for the given input action using its primary binding.
    /// </summary>
    /// <param name="action">The input action to display.</param>
    /// <returns>The formatted binding display string.</returns>
    public string GetInputActionAsString(InputAction action)
    {
        return GetInputActionAsString(action, 0);
    }

    /// <summary>
    /// Returns a display string for the given input action using the specified display options.
    /// </summary>
    /// <param name="action">The input action to display.</param>
    /// <param name="displayOptions">Formatting options for the binding string.</param>
    /// <returns>The formatted binding display string.</returns>
    public string GetInputActionAsString(InputAction action, InputBinding.DisplayStringOptions displayOptions)
    {
        // TODO: make sure this works with both keyboard and gamepad when I add gamepad functionality.
        return action.bindings[0].ToDisplayString(displayOptions);
    }

    /// <summary>
    /// Builds a formatted input prompt by combining the display string for the specified <see cref="InputAction"/>
    /// with a contextual message.
    /// <para>
    /// For example, a message that uses the interact keybinding: "[E]: Pick up item".
    /// </para>
    /// </summary>
    /// <param name="action">The input action whose primary binding should be displayed (e.g., Interact).</param>
    /// <param name="message">The contextual message to append after the formatted binding (e.g., "Pick up Item").
    /// </param>
    /// <returns>A formatted string in the form "[Binding]: Message".</returns>
    public string GetFormattedBindingDisplay(InputAction action, string message)
    {
        return $"[{GetInputActionAsString(action)}]: {message}";
    }
}
