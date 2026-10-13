using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Determines how a seller's inventory should be generated.
/// </summary>
public enum SellerInventoryMode
{
    /// <summary>
    /// Generates a random selection of items from the configured possible items.
    /// </summary>
    Random,

    /// <summary>
    /// Uses the configured exact items without randomly generating new items.
    /// </summary>
    Exact
}

[CreateAssetMenu(fileName = "NewSellerInventory", menuName = "Scriptable Objects/Profiles/New Seller Inventory")]
public class SellerInventorySO : ScriptableObject
{
    [Title("Generation Mode")]
    [Tooltip("Determines whether this inventory should be randomly generated or use the exact items configured below.")]
    [SerializeField]
    private SellerInventoryMode mode = SellerInventoryMode.Random;
    
    [Title("Randomly Selected From")]
    [Tooltip("The pool of items that can be randomly selected when using Random inventory mode.")]
    [ShowIf(nameof(mode), SellerInventoryMode.Random)]
    [RequiredListLength(1, null)]
    [SerializeField]
    private List<ItemDataSO> possibleItems;

    [Title("Random Item Count")]
    [Tooltip("The minimum number of items to generate when using Random inventory mode.")]
    [ShowIf(nameof(mode), SellerInventoryMode.Random)]
    [SerializeField]
    [Min(1)]
    private int minItemCount = 1;

    [Tooltip("The maximum number of items to generate when using Random inventory mode.")]
    [ShowIf(nameof(mode), SellerInventoryMode.Random)]
    [SerializeField]
    [Min(1)]
    private int maxItemCount = 3;

    [Title("Random Item Condition Configuration")]
    [Tooltip("Determines how conditions are randomly assigned to generated items.")]
    [ShowIf(nameof(mode), SellerInventoryMode.Random)]
    [Required]
    [SerializeField]
    private ItemConditionConfigSO conditionConfig;

    [Title("Use Exact Items")]
    [Tooltip("The exact items to give the seller when using Exact inventory mode.")]
    [ShowIf(nameof(mode), SellerInventoryMode.Exact)]
    [SerializeField]
    private List<Item> exactItems;

    /// <summary>
    /// Creates the seller's inventory according to the configured <see cref="mode"/>.
    /// </summary>
    /// <returns>A new list containing the items generated for the seller's inventory.</returns>
    public List<Item> GetItems()
    {
        switch (mode)
        {
            case SellerInventoryMode.Random:
                return GenerateRandomItems();

            case SellerInventoryMode.Exact:
                // Create copies so changes to the generated inventory don't actually modify this asset
                return exactItems.Select(item => new Item(item)).ToList();
                
            default:
#if UNITY_EDITOR
                Debug.LogError($"Unknown {nameof(SellerInventoryMode)} value '{mode}' on {name}.");
#endif
                // null is returned so this fails loudly instead of silently failing with an empty inventory
                return null;
        }
    }

    /// <summary>
    /// Generates a random selection of sellable items using the configured item pool, item count range, and
    /// condition configuration.
    /// </summary>
    /// <returns>A list containing the randomly generated seller items.</returns>
    private List<Item> GenerateRandomItems()
    {
        // In case the min/max values in the inspector are inverted
        int minCount = Mathf.Min(minItemCount, maxItemCount);
        int maxCount = Mathf.Max(minItemCount, maxItemCount);

        int finalCount = Random.Range(minCount, maxCount + 1); // + 1 because 2nd argument is exclusive
        
        List<ItemDataSO> copiedList = new(possibleItems);
        copiedList.Shuffle();

        // This goes without saying that the list count should always be greater than the max item count number.
        // Since I'm the only developer for this game, I'm fine leaving it like this
        int itemsToRemove = copiedList.Count - finalCount;
        if (itemsToRemove > 0)
        {
            copiedList.RemoveRange(finalCount, itemsToRemove);
        }

        List<Item> randomlyGeneratedItems = ItemFactory.CreateRandomSellables(copiedList, conditionConfig);
        return randomlyGeneratedItems;
    }
}
