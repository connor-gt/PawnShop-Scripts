using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShelfStocker : MonoBehaviour
{
    [Title("Stock Configuration")]
    [Tooltip("Configuration used to determine which items are stocked and how they are configured.")]
    [SerializeField]
    private ShelfStockDataSO stockingData;

    private const float SPAWN_DELAY = 0.2f;

    private void Start()
    {
        // Spawn delay is used to make sure all shelves are registered before accessing shelf controller
        this.Invoke(() => FillShelves(PawnShop.Instance.ShelfController.GetShelves(), stockingData), SPAWN_DELAY);
    }

    /// <summary>
    /// Populates the specified shelves with randomly selected sellable items using the provided stocking data.
    /// </summary>
    /// <param name="shelves">The shelves to populate with items.</param>
    /// <param name="stockingData">The configuration used to determine which items are spawned and how they are
    /// configured.</param>
    public void FillShelves(List<Shelf> shelves, ShelfStockDataSO stockingData)
    {
        // A list of placeable locations is used so that shelves with more placeable locations have a higher
        // weight to have items spawned there (for example, the corner shelf). This makes sure smaller shelves
        // don't end up with a much higher distribution
        List<PlaceableLocation> placeableLocations = new();
        for (int i = 0; i < shelves.Count; i++)
        {
            placeableLocations.AddRange(shelves[i].GetPlaceableLocations().Where(location => location.IsEmpty));
        }

        placeableLocations.Shuffle();

        for (int i = 0; i < stockingData.ItemsToSpawn; i++)
        {
            Item item = ItemFactory.CreateRandomSellable(
                stockingData.Items[Random.Range(0, stockingData.Items.Length)], stockingData.ConditionConfig
            );

            Vector2 multiplierRange = stockingData.ListedPriceMultiplierRange;
            float listedPriceMultiplier = Random.Range(multiplierRange.x, multiplierRange.y);
            item.Pricing.ListingPrice = (int)(item.Pricing.EstimatedValue * listedPriceMultiplier);

            placeableLocations[i].Place(item);
        }
    }
}
