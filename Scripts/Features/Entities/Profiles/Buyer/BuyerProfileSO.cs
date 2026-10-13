using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuyerProfile", menuName = "Scriptable Objects/Profiles/New Buyer Profile")]
public class BuyerProfileSO : ScriptableObject
{
    /// <summary>
    /// Gets the estimated value ratio at which this buyer considers an item to be reasonably priced and will
    /// purchase it without needing to consider their impulse buy chance.
    /// <para>
    /// The ratio is calculated by dividing the item's listing price by its estimated value. For example, a value
    /// of 0.7 means the buyer considers an item priced at 70% or less of its estimated value to be reasonably
    /// priced and will always purchase it.
    /// </para>
    /// <para>
    /// If the item's value ratio exceeds this value, the buyer may still purchase the item based on
    /// <see cref="ImpulseBuyChance"/>. The chance of an impulse purchase decreases as the item's value ratio
    /// approaches <see cref="MaxValueRatio"/>.
    /// </para>
    /// </summary>
    [field: Title("Buyer Settings")]
    [field: Tooltip("The maximum ratio of an item's estimated value that this buyer considers reasonably priced. "
        + "For example, 0.7 means the buyer will always purchase an item priced at 70% or less of its estimated "
        + "value. Items priced above this ratio may still be purchased based on the buyer's impulse buy chance.")]
    [field: SerializeField]
    [field: Range(0f, 2f)]
    public float DesiredValueRatio{ get; private set; } = 0.7f;

    /// <summary>
    /// Gets the estimated value ratio at which this buyer's impulse-buy chance reaches zero.
    /// <para>
    /// When an item's value ratio is between <see cref="DesiredValueRatio"/> and this value, the buyer has a
    /// chance to purchase the item despite it being priced above their desired ratio. This chance decreases
    /// linearly as the item's value ratio approaches this value.
    /// </para>
    /// <para>
    /// For example, if <see cref="DesiredValueRatio"/> is 0.7 and this value is 1.0, the buyer's
    /// <see cref="ImpulseBuyChance"/> is used at a value ratio of 0.7 and gradually decreases to 0% at a value
    /// ratio of 1.0. Because of this, values above this ratio will never be purchased.
    /// </para>
    /// </summary>
    [field: Tooltip("The value ratio at which this buyer's impulse-buy chance reaches 0%. The impulse buy chance "
        + "decreases linearly between the desired value ratio and this value. For example, 1.0 means the buyer "
        + "will have no chance to impulse buy an item priced at 100% or more of its estimated value.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float MaxValueRatio { get; private set; } = 1f;

    /// <summary>
    /// Gets the initial chance that this buyer will purchase an item through impulse buying when its value ratio
    /// is at or just above <see cref="DesiredValueRatio"/>.
    /// <para>
    /// This chance is gradually reduced as the item's value ratio approaches <see cref="MaxValueRatio"/>.
    /// The reduction is linear, meaning the configured chance is used at the desired value ratio and reaches
    /// 0% at the maximum value ratio.
    /// </para>
    /// <para>
    /// For example, an impulse buy chance of 0.5 gives the buyer a 50% chance to purchase an item that is only
    /// slightly above their desired value ratio. If the item's ratio is closer to <see cref="MaxValueRatio"/>,
    /// then the impulse buy chance is closer to 0%.
    /// </para>
    /// </summary>
    [field: Tooltip("The chance that this buyer will purchase an item through impulse buying when its price "
        + "exceeds their desired value ratio. The chance decreases as the item's value ratio approaches Max Value"
        + " Ratio, reaching 0% at that point. Set to 0 for no impulse buying.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float ImpulseBuyChance { get; private set; } = 0.5f;

    /// <summary>
    /// Gets the absolute maximum price this buyer is willing to pay for an item.
    /// <para>
    /// This value acts as a hard spending limit and is checked independently of the item's estimated value.
    /// Even if an item is priced within the buyer's desired or maximum value ratio, the buyer will not purchase
    /// it if its listing price exceeds this amount.
    /// </para>
    /// </summary>
    [field: Tooltip("The absolute maximum price this buyer is willing to pay for any item. This acts as a hard "
        + "spending limit regardless of the item's estimated value or value ratio.")]
    [field: SerializeField]
    [field: Min(0)]
    public int MaxPrice { get; private set; } = 500;

    /// <summary>
    /// Gets the lowest item condition this buyer is willing to accept.
    /// <para>
    /// Items below this condition will never be purchased regardless of their listing price or value ratio.
    /// </para>
    /// </summary>
    [field: Tooltip("Items below this condition will never be purchased regardless of their listing price or value"
        + " ratio.")]
    [field: SerializeField]
    public ItemCondition LowestAcceptableCondition { get; private set; } = ItemCondition.Broken;

    /// <summary>
    /// Gets the maximum items this buyer will browse.
    /// <para>
    /// If the buyer looks at this many items and still doesn't have a full inventory, they should either checkout
    /// with what they currently have or leave the store entirely if their inventory is empty.
    /// </para>
    /// </summary>
    [field: Tooltip("Once a buyer looks at (doesn't matter if they add it to their inventory or not) at this "
        + "many items, instead of continuing to shop, they will either checkout or leave the store.")]
    [field: SerializeField]
    [field: Min(0)]
    public int MaxItemsToBrowse { get; private set; } = 10;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Makes sure the maximum acceptable value ratio never goes below or equals desired value ratio. This
        // makes sure that the math performed in BuyerService.ShouldBuy() is good and that the potential for
        // division by 0 also doesn't happen
        MaxValueRatio = Mathf.Max(MaxValueRatio, DesiredValueRatio + 0.01f);
    }
#endif
}
