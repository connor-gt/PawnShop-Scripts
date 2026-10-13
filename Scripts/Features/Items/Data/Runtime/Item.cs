using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Represents a <b>runtime</b> instance of an item and stores all per-instance state, such as pricing information,
/// while referencing its shared <see cref="ItemDataSO"/> definition.
/// </summary>
[Serializable]
public class Item
{
    /// <summary>
    /// Gets this item's shared data definition.
    /// </summary>
    [field: Title("Item Data")]
    [field: SerializeField]
    public ItemDataSO Data { get; private set; }

    /// <summary>
    /// Gets this item's runtime pricing information.
    /// <para>
    /// This will be null if the item is not sellable.
    /// </para>
    /// </summary>
    [field: Title("Item Pricing")]
    [field: SerializeField]
    public ItemPricing Pricing { get; private set; } = null;

    /// <summary>
    /// Gets whether or not this item is sellable.
    /// </summary>
    public bool IsSellable => Data.IsSellable;

    /// <summary>
    /// Creates a new <see cref="Item"/> instance with the specified data and optional pricing information.
    /// </summary>
    /// <param name="data">The data for the new item.</param>
    /// <param name="pricing">The pricing information for the new item, or null if the item is not sellable.</param>
    public Item(ItemDataSO data, ItemPricing pricing = null)
    {
        Data = data;

        if (Data.IsSellable)
        {
            Pricing = pricing;
        }
    }
    
    /// <summary>
    /// Creates a copy of the specified <see cref="Item"/>.
    /// </summary>
    /// <param name="other">The item whose data should be copied.</param>
    public Item(Item other)
    {
        Data = other.Data;

        if (other.IsSellable)
        {
            Pricing = other.Pricing;
        }
    }

    /// <summary>
    /// Instantiates this item's world prefab as a child of the specified <see cref="Transform"/> and assigns
    /// this runtime item to the resulting <see cref="ItemInstance"/>.
    /// </summary>
    /// <param name="parent">The parent transform that the instantiated prefab should be attached to.</param>
    /// <returns>The newly instantiated <see cref="ItemInstance"/>.</returns>
    public ItemInstance InstantiateWorldPrefab(Transform parent)
    {
        return ItemServices.InstantiateWorldPrefab(this, parent);
    }
}
