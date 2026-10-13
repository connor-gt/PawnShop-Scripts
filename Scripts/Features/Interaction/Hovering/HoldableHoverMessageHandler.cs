using UnityEngine;

/// <summary>
/// Provides the default hover message for a <see cref="Holdable"/> item.
/// </summary>
[RequireComponent(typeof(ItemInstance), typeof(Holdable))]
public class HoldableHoverMessageHandler : MonoBehaviour, IHoverResponse, IHoverMessageUser
{
    public string HoverMessage { get; set; } = "";

    private ItemInstance itemInstance;
    private Holdable holdable;

    private void Awake()
    {
        itemInstance = GetComponent<ItemInstance>();
        holdable = GetComponent<Holdable>();
    }

    public void OnFocus(IHoverHandler hoverHandler)
    {
        // If an external system has already provided a hover message, use it
        if (!string.IsNullOrEmpty(HoverMessage))
        {
            hoverHandler.HoverMessage.UpdateHoverText(HoverMessage);
            return;
        }

        // No external override was provided, so fall back to the default logic which represents the basic
        // interaction behavior for holdable items
        if (hoverHandler.PlayerInventoryState.Inventory.IsFull)
        {
            HoverMessage = "You cannot hold any more items!";
        }
        else if (holdable.enabled)
        {
            HoverMessage = InputManager.Instance.GetFormattedBindingDisplay(
                InputManager.Instance.Actions.Base.Interact, $"Pick up {itemInstance.Item.Data.Name}"
            );
        }

        hoverHandler.HoverMessage.UpdateHoverText(HoverMessage);
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.ClearHoverText();
    }
}
