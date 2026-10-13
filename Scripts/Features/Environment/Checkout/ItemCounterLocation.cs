using UnityEngine;

/// <summary>
/// Represents a single location on a counter where an <see cref="Item"/> can be temporarily placed.
/// </summary>
public class ItemCounterLocation : MonoBehaviour
{
    /// <summary>
    /// Gets the current <see cref="Item"/> at this location.
    /// </summary>
    public ItemInstance CurrentItemInstance { get; private set; }

    /// <summary>
    /// Gets whether or not this <see cref="ItemCounterLocation"/> is currently occupied by an <see cref="Item"/>.
    /// </summary>
    public bool IsOccupied => CurrentItemInstance != null;

    /// <summary>
    /// Attempts to place an <see cref="Item"/> at this location.
    /// </summary>
    /// <param name="item">The item to place.</param>
    /// <returns>True if the item was successfully placed, false if otherwise (meaning the location is already
    /// occupied).</returns>
    public bool TryPlaceItem(Item item)
    {
        return TryPlaceItem(item, out _);
    }

    /// <summary>
    /// Attempts to place an <see cref="Item"/> at this location and returns the instantiated world object (or
    /// null if the placement failed).
    /// </summary>
    /// <param name="item">The item to place.</param>
    /// <param name="instantiatedObj">The instantiated world object representing the placed item, or null if
    /// the placement failed.</param>
    /// <returns>True if the item was successfully placed, false if otherwise (meaning the location is already
    /// occupied).</returns>
    public bool TryPlaceItem(Item item, out GameObject instantiatedObj)
    {
        if (IsOccupied)
        {
            instantiatedObj = null;
            return false;
        }

        ItemInstance instantiatedItem = item.InstantiateWorldPrefab(transform);
        CurrentItemInstance = instantiatedItem.GetComponent<ItemInstance>();
        instantiatedObj = instantiatedItem.gameObject;
        return true;
    }

    /// <summary>
    /// Removes and destroys the current world item at this location.
    /// </summary>
    /// <returns>The removed <see cref="Item"/>, or if this location is not occupied.</returns>
    public Item RemoveCurrentItem()
    {
        if (!IsOccupied)
        {
            return null;
        }

        Item item = CurrentItemInstance.Item;
        Destroy(CurrentItemInstance.gameObject);
        CurrentItemInstance = null;
        return item;
    }
}
