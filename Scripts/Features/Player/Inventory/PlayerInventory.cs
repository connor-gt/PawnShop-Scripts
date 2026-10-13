using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    /// <summary>
    /// This event is triggered when a change occurs within the player's inventory (e.g., adding/removing
    /// items and scrolling between items).
    /// </summary>
    public event Action OnUpdated;

    [field: Title("Inventory Reference")]
    [field: SerializeField]
    public Inventory Inventory { get; private set; }
    
    /// <summary>
    /// Gets the current focused item in the inventory. If the inventory is empty, this will return null.
    /// </summary>
    public Item CurrentItem
    {
        get
        {
            if (Inventory.Items.Count == 0)
            {
                return null;
            }

            return Inventory.Items[CurrentIndex];
        }
    }

    /// <summary>
    /// Gets whether or not there is a focused item in the inventory.
    /// </summary>
    public bool HasCurrentItem => CurrentItem != null;

    /// <summary>
    /// Gets the index of the current item in the inventory.
    /// </summary>
    public int CurrentIndex { get; private set; } = 0;

    /// <summary>
    /// Gets the number of items in the inventory.
    /// </summary>
    public int Count => Inventory.Items.Count;

    /// <summary>
    /// Gets the maximum number of items the player can have in the inventory.
    /// </summary>
    public int MaxItems => Inventory.MaxItems;

    /// <summary>
    /// Adds an item to the inventory. If the inventory is full, the item will not be added.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The added item, or null if the inventory is full.</returns>
    public Item Add(Item item)
    {
        if (Inventory.IsFull)
        {
            return null;
        }
        else if (Count == 0)
        {
            CurrentIndex = 0;
        }

        Inventory.Items.Add(item);
        OnUpdated?.Invoke();
        return item;
    }

    /// <summary>
    /// Removes and returns the currently selected item. Adjusts the <see cref="CurrentIndex"/> to remain
    /// valid after removal.
    /// </summary>
    /// <returns>The removed item, or null if the inventory is empty.</returns>
    public Item RemoveCurrentItem()
    {
        if (Count == 0)
        {
            return null;
        }

        Item item = Inventory.Items[CurrentIndex];
        Inventory.Items.RemoveAt(CurrentIndex);

        if (CurrentIndex >= Count)
        {
            CurrentIndex = Count - 1;
        }
        
        OnUpdated?.Invoke();
        return item;
    }

    /// <summary>
    /// Moves the selection to the next item in the inventory, wrapping around when reaching the end.
    /// </summary>
    /// <returns>The next item in the inventory, or null if the inventory is completely empty.</returns>
    public Item Next()
    {
        if (Count == 0)
        {
            return null;
        }

        CurrentIndex = (CurrentIndex + 1) % Count;
        OnUpdated?.Invoke();
        return Inventory.Items[CurrentIndex];
    }

    /// <summary>
    /// Returns the item that would be selected next without changing the current index.
    /// </summary>
    /// <returns>The next item in the inventory, or null if the inventory is completely empty.</returns>
    public Item PeekNext()
    {
        if (Count == 0)
        {
            return null;
        }

        return Inventory.Items[(CurrentIndex + 1) % Count];
    }

    /// <summary>
    /// Moves the selection to the previous item in the inventory, wrapping around when reaching the beginning.
    /// </summary>
    /// <returns>The previous item in the inventory, or null if the inventory is completely empty.</returns>
    public Item Previous()
    {
        if (Count == 0)
        {
            return null;
        }

        CurrentIndex = (CurrentIndex - 1 + Count) % Count;
        OnUpdated?.Invoke();
        return Inventory.Items[CurrentIndex];
    }

    /// <summary>
    /// Returns the item that would be selected previously without changing the current index.
    /// </summary>
    /// <returns>The previous item in the inventory, or null if the inventory is completely empty.</returns>
    public Item PeekPrevious()
    {
        if (Count == 0)
        {
            return null;
        }

        return Inventory.Items[(CurrentIndex - 1 + Count) % Count];
    }
}
