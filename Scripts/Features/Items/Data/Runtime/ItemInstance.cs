using UnityEngine;

/// <summary>
/// Represents a specific <see cref="Item"/> in the world. This component links a world object to its
/// runtime item data, allowing systems (hover, interact, hold, etc.) to access both the item's shared
/// definition and any per-instance state (e.g. purchase price, listing price, condition, etc.).
/// </summary>
public class ItemInstance : MonoBehaviour
{
    /// <summary>
    /// Gets the runtime <see cref="Item"/> represented by this world object.
    /// </summary>
    [field: SerializeField]
    public Item Item { get; private set; }

    /// <summary>
    /// Gets this item's shared data (e.g. name, world prefab, holdable data, etc.).
    /// </summary>
    public ItemDataSO Data => Item.Data;

    /// <summary>
    /// Set this runtime <see cref="ItemInstance"/>'s item data to a new item.
    /// <para>
    /// Since <see cref="ItemInstance"/> is tied to game objects and get reset when destroyed/instantiated,
    /// this should be called whenever a new item game object is created from an item's world prefab data.
    /// </para>
    /// </summary>
    /// <param name="item">The item data to use for this instance.</param>
    public void SetItem(Item item)
    {
        Item = item;
    }
}
