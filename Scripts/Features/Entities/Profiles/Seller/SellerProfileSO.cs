using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSellerProfile", menuName = "Scriptable Objects/Profiles/New Seller Profile")]
public class SellerProfileSO : ScriptableObject
{
    /// <summary>
    /// Gets the range of estimated item value ratios used to generate this seller's initial asking price.
    /// <para>
    /// For example, a value of (0.5, 0.7) means the seller's opening offer will be randomly chosen between 50%
    /// and 70% of the item's estimated value.
    /// </para>
    /// </summary>
    [field: Title("Starting Offer Percentage Range")]
    [field: Tooltip("The range of the item's estimated value that this seller will initially ask for. Example: 0.5"
        + " - 0.7 means 50% - 70% of the items' total estimated value. If the total est. value is $150, then a "
        + "valid starting range using 0.5 - 0.7 would be anywhere from $75 - $105.")]
    [field: SerializeField]
    public Vector2 StartingOfferValueRatioRange { get; private set; } = new(0.5f, 0.7f);

    /// <summary>
    /// Gets the value ratio of a list of items at which this seller will immediately accept a player's offer.
    /// <para>
    /// For example, a value of 0.5 means offers equal to or greater than 50% of the items' estimated value will be
    /// accepted without further negotiation.
    /// </para>
    /// <para>
    /// Player offers between this value and <see cref="MinimumAcceptableValueRatio"/> should trigger additional negotiation.
    /// </para>
    /// </summary>
    [field: Title("Acceptance Offer Percentages")]
    [field: Tooltip("The minimum ratio of the item's estimated value required for the seller to immediately accept an" 
        + " offer. Example: 0.5 means the seller accepts offers worth 50% or more of the items' estimated value.")]
    [field: SerializeField]
    [field: Range(0f, 1f)]
    public float AcceptanceValueRatio { get; private set; } = 0.5f;

    /// <summary>
    /// Gets how likely this seller is to accept offers slightly below their desired value ratio.
    /// </summary>
    [field: Tooltip("The chance that this seller accepts offers below their desired value ratio. This chance decreases"
        + " as the player's offer becomes further from the desired ratio. Set to 0 for no extra chance.")]
    [field: SerializeField]
    [field: Range(0f, 1f)]
    public float BelowDesiredAcceptanceChance { get; private set; } = 0.15f;

    /// <summary>
    /// Gets the lowest estimated value ratio this seller is willing to accept.
    /// <para>
    /// Offers below this threshold will cause the seller to reject the offer entirely rather than continue negotiating.
    /// </para>
    /// <para>
    /// For example, a value of 0.2 means the seller will never accept less than 20% of the item's estimated value.
    /// </para>
    /// </summary>
    [field: Tooltip("The absolute lowest ratio of the item's estimated value this seller is willing to accept. "
        + "Offers below this value ratio will be rejected.")]
    [field: SerializeField]
    [field: Range(0f, 1f)]
    public float MinimumAcceptableValueRatio { get; private set; } = 0.2f;

    /// <summary>
    /// Gets whether or not this seller has a limit on the number of counter-offers they will tolerate.
    /// <para>
    /// If true, see <see cref="MaxCounterOffers"/>.
    /// </para>
    /// </summary>
    [field: Title("Patience Settings")]
    [field: Tooltip("If enabled, MaxCounterOffers will be used in negotiations.")]
    [field: SerializeField]
    public bool LimitCounterOffers { get; private set; } = true;

    /// <summary>
    /// Gets the maximum number of counter-offers from the player this seller is willing to participate in before
    /// ending the negotiation.
    /// <para>
    /// For example, if this is 4 and the player and NPC go back and forth 4 times without settling, the NPC should
    /// walk away from the offer.
    /// </para>
    /// </summary>
    [field: Tooltip("The number of counter-offers this seller will tolerate before giving up.")]
    [field: ShowIf(nameof(LimitCounterOffers))]
    [field: SerializeField]
    [field: Min(0)]
    public int MaxCounterOffers { get; private set; } = 4;
}
