using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for creating runtime <see cref="Item"/> instances.
/// </summary>
public static class ItemFactory
{
    /// <summary>
    /// Creates a sellable item with the specified pricing data.
    /// </summary>
    /// <param name="data">The item's shared definition.</param>
    /// <param name="pricing">The runtime pricing information to assign.</param>
    /// <returns>A new sellable item instance.</returns>
    public static Item CreateSellable(ItemDataSO data, ItemPricing pricing)
    {
        return new Item(data, pricing);
    }

    /// <summary>
    /// Creates a sellable item with randomly generated pricing information based on the supplied condition
    /// configuration.
    /// <para>
    /// The item's estimated value is calculated by applying the generated condition's multiplier to the item's
    /// intrinsic value.
    /// </para>
    /// </summary>
    /// <param name="data">The shared item definition to create.</param>
    /// <param name="conditionConfig">The configuration used to generate the item's condition.</param>
    /// <returns>A new sellable item with randomly generated pricing information.</returns>
    public static Item CreateRandomSellable(ItemDataSO data, ItemConditionConfigSO conditionConfig)
    {
        int intrinsicValue = data.SellableData.IntrinsicValue;

        ItemCondition randomCondition = ItemConditionUtils.GetRandomCondition(conditionConfig);
        float conditionMultiplier = randomCondition.GetMultiplier(conditionConfig);
        int estimatedValue = (int)(intrinsicValue * conditionMultiplier);

        ItemPricing pricing = new(randomCondition, estimatedValue);
        return CreateSellable(data, pricing);
    }

    public static List<Item> CreateRandomSellables(List<ItemDataSO> dataList, ItemConditionConfigSO conditionConfig)
    {
        List<Item> randomItems = new();

        for (int i = 0; i < dataList.Count; i++)
        {
            Item randomItem = CreateRandomSellable(dataList[i], conditionConfig);
            randomItems.Add(randomItem);
        }

        return randomItems;
    }
}
