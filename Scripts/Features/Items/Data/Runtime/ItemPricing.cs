using System;
using UnityEngine;

/// <summary>
/// Stores all runtime pricing information associated with a sellable <see cref="Item"/>, including its
/// generated value, condition, and any player-controlled pricing data.
/// </summary>
[Serializable]
public class ItemPricing
{
    /// <summary>
    /// Gets or sets this <see cref="Item"/>'s condition.
    /// </summary>
    [field: SerializeField]
    public ItemCondition Condition { get; set; }

    /// <summary>
    /// Gets or sets this <see cref="Item"/>'s estimated value.
    /// <para>
    /// This value is typically generated from the item's intrinsic value and modified by factors such as
    /// condition or rarity.
    /// </para>
    /// </summary>
    [field: SerializeField]
    public int EstimatedValue { get; set; }

    /// <summary>
    /// Gets or sets the amount that the player originally paid to acquire this <see cref="Item"/>.
    /// </summary>
    [field: SerializeField]
    public int PlayerPurchasePrice { get; set; }

    /// <summary>
    /// Gets or sets the price that the player is currently listing this <see cref="Item"/> for.
    /// </summary>
    [field: SerializeField]
    public int ListingPrice { get; set; }

    /// <summary>
    /// Gets the profit that the player would make if this item sold at its current listing price.
    /// </summary>
    public int Profit => ListingPrice - PlayerPurchasePrice;

    /// <summary>
    /// Gets the profit margin of this item based on the player's purchase price (rounded to 1 decimal place).
    /// <para>
    /// A value of <c>100.0</c> represents a 100% return on investment, while <c>25.5</c> represents a 25.5% return.
    /// </para>
    /// <para>
    /// If the player's purchase price is <c>0</c>, the margin is defaulted to 100%.
    /// </para>
    /// </summary>
    public float Margin => 
        PlayerPurchasePrice == 0 ? 100 : (float)Math.Round((float)Profit / PlayerPurchasePrice * 100, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPricing"/> class with the specified condition, estimated
    /// value, player purchase price, and listing price.
    /// </summary>
    /// <param name="condition">The condition of the item.</param>
    /// <param name="estimatedValue">The estimated value of the item.</param>
    /// <param name="playerPurchasePrice">The price at which the player purchased the item.</param>
    /// <param name="listingPrice">The price at which the player is listing the item.</param>
    public ItemPricing(ItemCondition condition, int estimatedValue, int playerPurchasePrice, int listingPrice)
    {
        Condition = condition;
        EstimatedValue = estimatedValue;
        PlayerPurchasePrice = playerPurchasePrice;
        ListingPrice = listingPrice;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPricing"/> class with the specified condition and
    /// estimated value.
    /// </summary>
    /// <param name="condition">The condition of the item.</param>
    /// <param name="estimatedValue">The estimated value of the item.</param>
    public ItemPricing(ItemCondition condition, int estimatedValue) : this(condition, estimatedValue, 0, 0) { }
}
