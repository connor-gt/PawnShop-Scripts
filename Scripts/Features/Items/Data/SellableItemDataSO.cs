using UnityEngine;

[CreateAssetMenu(fileName = "NewSellableItemData", menuName = "Scriptable Objects/Item Data/New Sellable Item Data")]
public class SellableItemDataSO : ScriptableObject
{
    /// <summary>
    /// Gets this sellable item's intrinsic value.
    /// <para>
    /// This represents the item's baseline value before any runtime modifiers (such as condition, rarity, or
    /// special events) are applied.
    /// </para>
    /// </summary>
    [field: Tooltip("This represents the item's baseline value before any runtime modifiers (such as condition,"
        + " rarity, or special events) are applied.")]
    [field: SerializeField]
    public int IntrinsicValue { get; private set; }
}
