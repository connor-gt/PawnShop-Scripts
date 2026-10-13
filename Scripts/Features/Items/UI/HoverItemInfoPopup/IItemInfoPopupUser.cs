using System;

/// <summary>
/// Provides access to an item's information popup content and the item instance it represents.
/// </summary>
public interface IItemInfoPopupUser
{
    /// <summary>
    /// Gets the item instance associated with this item information popup.
    /// </summary>
    ItemInstance ItemInstance { get; }

    /// <summary>
    /// Sets the function used to generate information displayed by this item information popup.
    /// <para>
    /// For example, <see cref="ItemServices.CreateGenericInfo(Item)"/> can be provided to display an item's
    /// default information, while <see cref="ItemServices.CreateListedItemInfo(Item)"/> can be provided to
    /// include information specific to an item listed for sale.
    /// </para>
    /// </summary>
    /// <param name="contentProvider">The function used to generate popup content from the associated item.</param>
    void SetContentProvider(Func<Item, ItemInfoContent> contentProvider);

    /// <summary>
    /// Refreshes the information displayed by this item information popup using its current content provider.
    /// </summary>
    void Refresh();
}
