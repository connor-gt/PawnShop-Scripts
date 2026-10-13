using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    /// <summary>
    /// Gets the list of items in this inventory.
    /// </summary>
    [field: SerializeField]
    public List<Item> Items { get; private set; } = new();

    /// <summary>
    /// Gets or sets the maximum number of items this inventory can hold.
    /// </summary>
    [field: SerializeField]
    [field: Min(0)]
    public int MaxItems { get; set; } = 5;

    /// <summary>
    /// Gets whether or not the inventory is full.
    /// </summary>
    public bool IsFull => Items.Count >= MaxItems;
}
