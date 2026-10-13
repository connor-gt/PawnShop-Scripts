using System;
using UnityEngine;

[RequireComponent(typeof(ItemInstance))]
public class ItemInfoPopupHandler : MonoBehaviour, IHoverResponse, IItemInfoPopupUser
{
    public ItemInstance ItemInstance { get; private set; }

    /// <summary>
    /// Delegate function that generates the information content displayed by the item's information popup.
    /// </summary>
    private Func<Item, ItemInfoContent> contentProvider;

    private void Awake()
    {
        ItemInstance = GetComponent<ItemInstance>();

        // If no delegate is provided by an external system (e.g., a shelf wanting to display listed item info
        // for this item instance), just generate the popup info with generic information
        contentProvider = ItemServices.CreateGenericInfo;
    }

    public void OnFocus(IHoverHandler hoverHandler)
    {
        // Get the game object's highest y-position based on its mesh
        Vector2 verticalBounds = GameObjectUtils.GetMinMaxBound(gameObject, Axis.Y);
        Vector3 topPosition = new(transform.position.x, verticalBounds.y, transform.position.z);

        // Place the popup slightly above this object's mesh highest y-position
        PawnShop.Instance.ItemInfoPopup.PlaceAbove(topPosition);
        PawnShop.Instance.ItemInfoPopup.Show(contentProvider(ItemInstance.Item));
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        PawnShop.Instance.ItemInfoPopup.Hide();
    }

    public void SetContentProvider(Func<Item, ItemInfoContent> provider)
    {
        contentProvider = provider;
    }

    public void Refresh()
    {
        PawnShop.Instance.ItemInfoPopup.SetContent(contentProvider(ItemInstance.Item));
    }
}
