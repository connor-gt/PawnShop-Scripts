using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A module that contains an <see cref="Inventory"/> object.
/// </summary>
public class InventoryModule : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;

    /// <summary>
    /// Check if the <see cref="inventory"/> is currently full.
    /// </summary>
    public bool IsFull => inventory.IsFull;

    /// <summary>
    /// Gets the current count of the number of <see cref="Item"/>s in the inventory.
    /// </summary>
    public int Count => inventory.Items.Count;

    /// <summary>
    /// Add an <see cref="Item"/> to this inventory.
    /// </summary>
    /// <param name="item">The <see cref="Item"/> to add.</param>
    /// <returns>The <see cref="Item"/> that was added. If the inventory is full, null is returned.</returns>
    public Item Add(Item item)
    {
        if (IsFull)
        {
            return null;
        }

        inventory.Items.Add(item);
        return item;
    }

    /// <summary>
    /// Add a list of <see cref="Item"/>s to this inventory. If the list is full, already contains an
    /// <see cref="Item"/> instance, or runs out of space during the addition, some <see cref="Item"/>s may
    /// not be added.
    /// </summary>
    /// <param name="items">The list of items to add.</param>
    /// <returns>The count of items that were successfully added to this inventory.</returns>
    public int Add(IReadOnlyList<Item> items)
    {
        int numItemsAdded = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (!IsFull && !inventory.Items.Contains(items[i]))
            {
                inventory.Items.Add(items[i]);
                numItemsAdded++;
            }
        }

        return numItemsAdded;
    }

    /// <summary>
    /// Gets a copy of the <see cref="Item"/>s that are currently in this inventory.
    /// </summary>
    /// <returns>A copied list of <see cref="Item"/>s.</returns>
    public List<Item> GetItems()
    {
        return inventory.Items.ToList();
    }

    /// <summary>
    /// Empty this inventory and get the <see cref="Item"/>s that were in it (if any).
    /// </summary>
    /// <returns>A list of the <see cref="Item"/> this inventory had before being emptied.</returns>
    public List<Item> Empty()
    {
        List<Item> items = new(inventory.Items);
        inventory.Items.Clear();
        return items;
    }
}
