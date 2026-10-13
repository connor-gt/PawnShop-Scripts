using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Scriptable object that holds data for items that can be interacted with in the world in any meaningful way
/// (e.g. if the item is hoverable, interactable, holdable, etc. or a combination of any of the above or more).
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "Scriptable Objects/Item Data/New Item Data")]
public class ItemDataSO : ScriptableObject
{
    [field: Title("Unique ID")]
    [field: Tooltip("A unique identifier used to save and load this item. Once this item is used in a save "
        + "file, this ID should not be changed.")]
    [field: SerializeField]
    public string Id { get; private set; } = "unique_id";

    [field: Title("Item Name")]
    [field: SerializeField]
    public string Name { get; private set; } = "Name";

    /// <summary>
    /// Gets the world prefab of this item.
    /// </summary>
    [field: Title("Prefab")]
    [field: SerializeField]
    public GameObject WorldPrefab { get; private set; } = null;

    [field: Title("Item Description")]
    [field: SerializeField]
    public string Description { get; private set; } = "Generic description.";

    /// <summary>
    /// Gets the holdable data of this item. If this is null, the item is not holdable.
    /// </summary>
    [field: Title("Holdable Data (Optional)")]
    [field: Tooltip("Optional data that's used on holdable items that the player grabs. If this item is not "
        + "holdable, leave null.")]
    [field: SerializeField]
    public HoldableItemDataSO HoldableData { get; private set; } = null;

    /// <summary>
    /// Gets whether or not this item is holdable.
    /// </summary>
    public bool IsHoldable => HoldableData != null;

    /// <summary>
    /// Gets the sellable data of this item. If this is null, the item is not sellable.
    /// </summary>
    [field: Title("Sellable Data (Optional)")]
    [field: Tooltip("Optional data that's used on sellable items. If this item is not sellable, leave null.")]
    [field: SerializeField]
    public SellableItemDataSO SellableData { get; private set; } = null;

    /// <summary>
    /// Gets whether or not this item is sellable.
    /// </summary>
    public bool IsSellable => SellableData != null;
}
