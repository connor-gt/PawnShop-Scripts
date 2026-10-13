using UnityEngine;

public class Scanner : MonoBehaviour, IHoverContextReceiver
{
    /// <summary>
    /// Gets the item currently being hovered over by the player, or null if no item is currently being hovered
    /// over by the player OR if the <see cref="Hoverable"/> that's being hovered over does not have an
    /// <see cref="ItemInstance"/> component.
    /// </summary>
    public ItemInstance CurrentHoveredItem { get; private set; } = null;

    private IHoverHandler hoverHandler;

    public void Inject(IHoverHandler hoverHandler)
    {
        this.hoverHandler = hoverHandler;

        this.hoverHandler.OnFocus += OnHoverableFocused;
        this.hoverHandler.OffFocus += OnHoverableUnfocused;

        OnHoverableFocused(this.hoverHandler.CurrentHoverable);
    }

    private void OnDisable()
    {
        if (hoverHandler != null)
        {
            hoverHandler.OnFocus -= OnHoverableFocused;
            hoverHandler.OffFocus -= OnHoverableUnfocused;
        }
    }

    /// <summary>
    /// Updates the current hovered item when the player focuses a hoverable object containing an
    /// <see cref="ItemInstance"/>.
    /// </summary>
    /// <param name="hoverable">The <see cref="Hoverable"/> object the player has focused.</param>
    private void OnHoverableFocused(Hoverable hoverable)
    {
        if (hoverable != null && hoverable.TryGetComponent(out ItemInstance itemInstance))
        {
            CurrentHoveredItem = itemInstance;
        }
    }

    /// <summary>
    /// Clears the current hovered item when the player stops focusing a hoverable object.
    /// </summary>
    /// <param name="hoverable">The <see cref="Hoverable"/> object the player has stopped focusing.</param>
    private void OnHoverableUnfocused(Hoverable hoverable)
    {
        CurrentHoveredItem = null;
    }

    /// <summary>
    /// Attempts to update the listing price of the currently hovered sellable item.
    /// </summary>
    /// <param name="amount">The amount by which to change the item's listing price.</param>
    /// <returns>True if the current hovered item is sellable and its listing price was updated, false if
    /// otherwise.</returns>
    public bool TryAdjustItemPrice(int amount)
    {
        if (IsCurrentHoveredItemSellable())
        {
            CurrentHoveredItem.Item.Pricing.ListingPrice += amount;
            if (CurrentHoveredItem.TryGetComponent(out IItemInfoPopupUser infoPopupUser))
            {
                infoPopupUser.Refresh();
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the currently hovered item exists and is sellable.
    /// </summary>
    /// <returns>True if the <see cref="CurrentHoveredItem"/> is not null and is sellable, false if otherwise.
    /// </returns>
    public bool IsCurrentHoveredItemSellable()
    {
        return CurrentHoveredItem != null && CurrentHoveredItem.Item.IsSellable;
    }
}
