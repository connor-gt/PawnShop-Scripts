using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Provides utility methods related to <see cref="Item"/>s.
/// </summary>
public static class ItemServices
{
    /// <summary>
    /// Instantiates this item's world prefab as a child of the specified <see cref="Transform"/> and assigns
    /// this runtime item to the resulting <see cref="ItemInstance"/>.
    /// </summary>
    /// <param name="item">The item that the instantiated <see cref="ItemInstance"/> is created from.</param>
    /// <param name="parent">The parent transform that the instantiated prefab should be attached to.</param>
    /// <returns>The newly instantiated <see cref="ItemInstance"/>.</returns>
    public static ItemInstance InstantiateWorldPrefab(Item item, Transform parent)
    {
        GameObject instantiatedItemObj = GameObject.Instantiate(item.Data.WorldPrefab, parent);
        ItemInstance itemInstance = instantiatedItemObj.GetComponent<ItemInstance>();
        itemInstance.SetItem(item);
        return itemInstance;
    }

    /// <summary>
    /// Calculates the combined estimated value of a collection of items.
    /// </summary>
    /// <param name="items">The items whose estimated values should be combined.</param>
    /// <returns>The sum of all estimated item values.</returns>
    public static int GetTotalEstimatedValue(IReadOnlyList<Item> items)
    {
        int estimatedValue = 0;

        for (int i = 0; i < items.Count; i++)
        {
            estimatedValue += items[i].Pricing.EstimatedValue;
        }

        return estimatedValue;
    }

    /// <summary>
    /// Calculates the combined listing price of a collection of items.
    /// </summary>
    /// <param name="items">The collection of <see cref="Item"/> instances to calculate the total price for.
    /// </param>
    /// <returns>The sum of all item listing prices.</returns>
    public static int GetTotalListingPrice(IReadOnlyList<Item> items)
    {
        int totalValue = 0;

        for (int i = 0; i < items.Count; i++)
        {
            totalValue += items[i].Pricing.ListingPrice;
        }

        return totalValue;
    }

    /// <summary>
    /// Calculates and assigns to each item the purchase price for each item based on its proportion of the total
    /// estimated value of all of the items in the list.
    /// <para>
    /// The total purchase price is distributed proportionally among the items so that items with a higher
    /// estimated value receive a larger share of the purchase price. This provides each item with an individual
    /// purchase price when multiple items are purchased as part of a single transaction.
    /// </para>
    /// </summary>
    /// <param name="items">The items included in the transaction.</param>
    /// <param name="totalPurchasePrice">The total amount paid for all items in the transaction.</param>
    /// <returns>A dictionary containing each item and its proportionally allocated purchase price. If the input
    /// item list is empty, null is returned.</returns>
    public static Dictionary<Item, int> AssignItemPurchasePrices(IReadOnlyList<Item> items, int totalPurchasePrice)
    {
        if (items.Count == 0)
        {
            return null;
        }

        Dictionary<Item, int> itemsWithPurchasePrices = new();
        int totalEstValue = GetTotalEstimatedValue(items);

        for (int i = 0; i < items.Count; i++)
        {
            float proportionPercentage = 0f;
            if (totalEstValue > 0)
            {
                proportionPercentage = (float)items[i].Pricing.EstimatedValue / totalEstValue;
            }

            float purchasePrice = totalPurchasePrice * proportionPercentage;
            int purchasePriceRounded = (int)Math.Round(purchasePrice, MidpointRounding.AwayFromZero);
            itemsWithPurchasePrices.Add(items[i], purchasePriceRounded);
        }

        // Makes sure to account for possible discrepancies with rounding (e.g., if the totalPurchasePrice = $24
        // but the sum of the calculated purchase prices for the items ends up being $23, add $1 to the largest item)
        int difference = totalPurchasePrice - itemsWithPurchasePrices.Values.Sum();
        if (difference != 0)
        {
            // Find the item with the highest price and add/subtract the difference to that item
            Item maxItem = itemsWithPurchasePrices.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            itemsWithPurchasePrices[maxItem] += difference;
        }

        foreach (var kvp in itemsWithPurchasePrices)
        {
            kvp.Key.Pricing.PlayerPurchasePrice = kvp.Value;
        }

        return itemsWithPurchasePrices;
    }
    
    /// <inheritdoc cref="AssignItemPurchasePrices(IReadOnlyList{Item}, int)"/>
    public static Dictionary<Item, int> AssignItemPurchasePrices(IReadOnlyList<ItemInstance> instances,
        int totalPurchasePrice)
    {
        List<Item> items = instances.Select(i => i.Item).ToList();
        return AssignItemPurchasePrices(items, totalPurchasePrice);
    }

    /// <summary>
    /// Assigns a default listing price to each item based on its purchase price and the specified profit margin.
    /// </summary>
    /// <param name="items">The items whose listing prices should be assigned.</param>
    /// <param name="margin">The desired profit margin as a percentage (e.g., 50% or 85.5%).</param>
    public static void AssignDefaultListingPrices(List<Item> items, float margin)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int defaultListingPrice = CalculateDefaultListingPrice(items[i], margin);
            items[i].Pricing.ListingPrice = defaultListingPrice;
        }
    }

    /// <inheritdoc cref="AssignDefaultListingPrices(List{Item}, float)"/>
    public static void AssignDefaultListingPrices(List<ItemInstance> instances, float margin)
    {
        List<Item> items = instances.Select(i => i.Item).ToList();
        AssignDefaultListingPrices(items, margin);
    }

    /// <summary>
    /// Calculates the default listing price for an item based on its purchase price and the specified profit margin.
    /// <para> 
    /// The resulting price is rounded up to the nearest whole number.
    /// </para>
    /// </summary>
    /// <param name="item">The item whose default listing price should be calculated.</param>
    /// <param name="margin">The desired profit margin as a percentage (e.g., 50% or 85.5%).</param>
    /// <returns>The calculated default listing price.</returns>
    public static int CalculateDefaultListingPrice(Item item, float margin)
    {
        return Mathf.CeilToInt(item.Pricing.PlayerPurchasePrice * ((margin / 100f) + 1f));
    }

    /// <summary>
    /// Creates generic information content using this item's name and description.
    /// </summary>
    /// <param name="item">The item to create information for.</param>
    /// <returns>Content containing the item's name and description.</returns>
    public static ItemInfoContent CreateGenericInfo(Item item)
    {
        return new ItemInfoContent(item.Data.Name, item.Data.Description);
    }

    /// <summary>
    /// Creates information content for an item being offered to the player for purchase.
    /// <para>
    /// The content includes the item's description, condition, and estimated value. 
    /// </para>
    /// </summary>
    /// <param name="item">The item to create information for.</param>
    /// <returns>Content containing information relevant to purchasing the item.</returns>
    public static ItemInfoContent CreateSellerInfo(Item item)
    {
        string itemName = item.Data.Name;
        string itemDescription = item.Data.Description;
        string[] itemInfo =
        {
            $"{item.Pricing.Condition.GetFlavorText()}",
            " ",
            $"Estimated Value: ${item.Pricing.EstimatedValue}"
        };

        return new ItemInfoContent(itemName, itemDescription, itemInfo);
    }

    /// <summary>
    /// Creates information content for an item currently listed for sale by the player.
    /// </summary>
    /// <param name="item">The item to create information for.</param>
    /// <returns>Content containing information relevant to a listed item.</returns>
    public static ItemInfoContent CreateListedItemInfo(Item item)
    {
        string itemName = item.Data.Name;
        string itemDescription = item.Data.Description;
        string[] itemInfo =
        {
            $"Condition: {item.Pricing.Condition}",
            $"Estimated Value: ${item.Pricing.EstimatedValue}",
            $"Bought For: ${item.Pricing.PlayerPurchasePrice}",
            $"Listed Price: ${item.Pricing.ListingPrice}",
            $"Profit Margin: {item.Pricing.Margin}%"
        };

        return new ItemInfoContent(itemName, itemDescription, itemInfo);
    }

    /// <summary>
    /// Creates detailed information for sellable items.
    /// </summary>
    /// <param name="item">The item to create information for.</param>
    /// <returns>Content containing information relevant to a sellable item.</returns>
    public static ItemInfoContent CreateDetailedSellableItemInfo(Item item)
    {
        string itemName = item.Data.Name;
        string itemDescription = item.Data.Description;
        string[] itemInfo =
        {
            $"Condition: {item.Pricing.Condition}",
            $"{item.Pricing.Condition.GetFlavorText()}",
            " ",
            $"Estimated Value: ${item.Pricing.EstimatedValue}",
            " ",
            $"Bought For: ${item.Pricing.PlayerPurchasePrice}",
            $"Listed Price: ${item.Pricing.ListingPrice}",
            " ",
            $"Profit Margin: {item.Pricing.Margin}%"
        };

        return new ItemInfoContent(itemName, itemDescription, itemInfo);
    }
}
