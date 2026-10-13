using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Configuration asset for stocking shelves with items.
/// </summary>
[CreateAssetMenu(fileName = "NewShelfStockData", menuName = "Scriptable Objects/New Shelf Stock Data")]
public class ShelfStockDataSO : ScriptableObject
{   
    /// <summary>
    /// Gets the item data that can be randomly selected when stocking shelves.
    /// </summary>
    [field: Title("Items to Stock Shelves")]
    [field: SerializeField]
    public ItemDataSO[] Items { get; private set; }

    /// <summary>
    /// Gets the configuration used to determine the condition of spawned items.
    /// </summary>
    [field: Title("Item Condition Configuration")]
    [field: SerializeField]
    public ItemConditionConfigSO ConditionConfig { get; private set; }

    /// <summary>
    /// Gets te minimum and maximum multipliers applied to an item's estimated value when determining its listed
    /// price.
    /// </summary>
    [field: Title("Item Listed Price Configuration")]
    [field: Tooltip("The minimum and maximum multipliers applied to an item's estimated value when determining"
        + " its listed price.")]
    [field: SerializeField]
    public Vector2 ListedPriceMultiplierRange { get; private set; } = new(0.75f, 0.95f);

    /// <summary>
    /// The number of items to place on the shelves.
    /// </summary>
    [field: Title("Number of Items")]
    [field: Tooltip("The total number of items to place on the shelves.")]
    [field: SerializeField]
    [field: Min(0)]
    public int ItemsToSpawn { get; private set; } = 20;
}
