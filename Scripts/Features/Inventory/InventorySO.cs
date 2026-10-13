using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A peristent, asset-based <see cref="global::Inventory"/> that can be used to store <see cref="ItemDataSO"/> items.
/// </summary>
[CreateAssetMenu(fileName = "NewInventory", menuName = "Scriptable Objects/New Inventory")]
public class InventorySO : ScriptableObject
{
    [field: SerializeField]
    public Inventory Inventory { get; private set; } = new();

    /// <summary>
    /// Gets the list of items in this inventory.
    /// </summary>
    public List<Item> Items => Inventory.Items;

    /// <summary>
    /// Gets or sets the maximum number of items this inventory can hold.
    /// </summary>
    public int MaxItems
    {
        get => Inventory.MaxItems;
        set => Inventory.MaxItems = value;
    }

    /// <summary>
    /// Gets whether or not the inventory is full.
    /// </summary>
    public bool IsFull => Inventory.IsFull;
}
