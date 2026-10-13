using UnityEngine;

/// <summary>
/// Provides calculations and evaluations for NPC purchase behavior using a specified buyer profile.
/// </summary>
public class BuyerService
{
    private readonly BuyerProfileModule buyerProfileModule;

    /// <summary>
    /// Gets the <see cref="BuyerProfileSO"/> that this service is currently using via <see cref="BuyerProfileModule"/>.
    /// </summary>
    public BuyerProfileSO BuyerProfile => buyerProfileModule.Profile;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuyerService"/> class.
    /// </summary>
    /// <param name="buyerProfileModule">The buyer profile module that contains the buyer profile which determines how
    /// likely the buyer is to purchase something based on its price and other conditions.</param>
    public BuyerService(BuyerProfileModule buyerProfileModule)
    {
        this.buyerProfileModule = buyerProfileModule;
    }

    /// <summary>
    /// Determines whether the NPC should purchase the specified item based on its listing price.
    /// </summary>
    /// <param name="item">The item considered for purchasing.</param>
    /// <returns>True if the item should be bought based on the <see cref="BuyerProfile"/>, false if otherwise.
    /// </returns>
    public bool ShouldBuy(Item item)
    {
        if (!MeetsMinimumRequirements(item))
        {
            return false;
        }

        float valueRatio = CalculateValueRatio(item);
        if (valueRatio <= BuyerProfile.DesiredValueRatio)
        {
            return true;
        }

        // If the value ratio is greater than the desired value ratio, have a smaller chance to impulse buy
        // anyways. The closer the value ratio is to the buyer's desired value ratio, the higher the chance
        float difference = valueRatio - BuyerProfile.DesiredValueRatio;
        float range = BuyerProfile.MaxValueRatio - BuyerProfile.DesiredValueRatio;
        float lerpValue = difference / range;
        float acceptanceChance = Mathf.Lerp(BuyerProfile.ImpulseBuyChance, 0f, lerpValue);
        if (Random.value < acceptanceChance)
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="ShouldBuy(Item)"/>
    public bool ShouldBuy(Holdable holdable)
    {
        if (holdable.TryGetComponent(out ItemInstance itemInstance))
        {
            return ShouldBuy(itemInstance.Item);
        }

        return false;
    }

    /// <summary>
    /// Determines whether an item meets the buyer's minimum requirements for purchase.
    /// <para>
    /// This acts as a hard stop if the item does not meet certain hard requirements, like
    /// <see cref="BuyerProfileSO.MaxPrice"/>.
    /// </para>
    /// </summary>
    /// <param name="item">The item being evaluated.</param>
    /// <returns>True if the item satisfies all of the buyer's minimum purchase requirements, false if otherwise.
    /// </returns>
    public bool MeetsMinimumRequirements(Item item)
    {
        return item.IsSellable
            && item.Pricing.ListingPrice <= BuyerProfile.MaxPrice
            && item.Pricing.Condition >= BuyerProfile.LowestAcceptableCondition;
    }

    /// <summary>
    /// Calculates the ratio between an item's listing price and its estimated value.
    /// <para>
    /// The value ratio represents how expensive an item is relative to its estimated value (e.g., a value ratio
    /// of 0.7 means the item's listing price is 70% of its estimated value).
    /// </para>
    /// <para>
    /// A higher value ratio means a worse deal, so a value ratio of 0.5 for an item is a much better deal than
    /// an item with a listing price that has a value ratio of 2.5.
    /// </para>
    /// </summary>
    /// <param name="item">The item whose listing price and estimated value should be compared.</param>
    /// <returns>The item's listing price as a ratio of its estimated value, or 0 if the item is not sellable.
    /// </returns>
    public float CalculateValueRatio(Item item)
    {
        if (!item.IsSellable)
        {
            return 0f;
        }

        return item.Pricing.ListingPrice / (float)item.Pricing.EstimatedValue;
    }
}
