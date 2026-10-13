using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A hover response object that displays hover text when hovered over.
/// </summary>
public class HoverMessageObject : MonoBehaviour, IHoverResponse, IHoverMessageUser
{
    [field: Title("Message Text")]
    [field: SerializeField]
    public string HoverMessage { get; set; } = "Hover text";

    [Title("Message Settings")]
    [Tooltip("If enabled, a keybind can be shown before a message like this: \"[E]: Grab Item\".")]
    [SerializeField]
    private bool includeInputKeybinding = false;

    [Tooltip("The input action reference's string name that should be displayed before the message.")]
    [ShowIf(nameof(includeInputKeybinding))]
    [SerializeField]
    private InputActionReference inputAction = null;

    public void OnFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.UpdateHoverText(GetMessage());
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.ClearHoverText();
    }

    /// <summary>
    /// Builds and returns the final hover text string for this object. If input keybinding display is
    /// enabled and a valid <see cref="InputActionReference"/> is assigned, the method prepends the
    /// action's display binding (e.g., "[E]:") before the base message.
    /// </summary>
    /// <returns>The formatted hover text string to display while this object is focused.</returns>
    public string GetMessage()
    {
        if (includeInputKeybinding && inputAction != null)
        {
            return InputManager.Instance.GetFormattedBindingDisplay(inputAction, HoverMessage);
        }

        return HoverMessage;
    }
}
