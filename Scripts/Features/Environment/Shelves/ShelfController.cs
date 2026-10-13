using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShelfController : MonoBehaviour
{
    private readonly List<Shelf> shelves = new();

    /// <summary>
    /// Register a <see cref="Shelf"/> with the <see cref="ShelfController"/> so it can be accessed by other
    /// systems.
    /// </summary>
    /// <param name="shelf">The shelf to register.</param>
    public void RegisterShelf(Shelf shelf)
    {
        if (!shelves.Contains(shelf))
        {
            shelves.Add(shelf);
        }
    }

    /// <summary>
    /// Get a random <see cref="Shelf"/> object.
    /// </summary>
    /// <returns>A randomly selected <see cref="Shelf"/></returns>
    public Shelf GetRandomShelf()
    {
        return shelves[Random.Range(0, shelves.Count)];
    }

    /// <summary>
    /// Gets a random <see cref="Shelf"/> that either has or does not have any items.
    /// </summary>
    /// <param name="hasItems">Whether the returned <see cref="Shelf"/> must have at least one item.</param>
    /// <returns>A randomly selected <see cref="Shelf"/> that matches the specified item state, or null if there
    /// are no shelves that match the <see cref="hasItems"/> state.</returns>
    public Shelf GetRandomShelf(bool hasItems)
    {
        List<Shelf> filteredShelves = GetShelves(hasItems);

        if (filteredShelves.Count == 0)
        {
            return null;
        }

        return filteredShelves[Random.Range(0, filteredShelves.Count)];
    }

    /// <summary>
    /// Gets a snapshot list of all registered shelves.
    /// </summary>
    /// <returns>A list containing all registered <see cref="Shelf"/> objects.</returns>
    public List<Shelf> GetShelves()
    {
        return shelves.ToList();
    }

    /// <summary>
    /// Gets a snapshot of all registered shelves that either have or do not have any items.
    /// </summary>
    /// <param name="hasItems">Whether to return shelves that have at least one item. If false, only empty
    /// shelves are returned.</param>
    /// <returns>A list of shelves matching the specified item state.</returns>
    public List<Shelf> GetShelves(bool hasItems)
    {
        List<Shelf> filteredShelves = new();
        
        for (int i = 0; i < shelves.Count; i++)
        {
            if (hasItems && shelves[i].ItemCount > 0)
            {
                filteredShelves.Add(shelves[i]);
            }
            else if (!hasItems && shelves[i].ItemCount == 0)
            {
                filteredShelves.Add(shelves[i]);
            }
        }

        return filteredShelves;
    }

    /// <summary>
    /// Gets a random shelf item, optionally requiring its <see cref="InteractionPoint"/> to be available.
    /// </summary>
    /// <param name="requiresAvailability">Whether the returned <see cref="ShelfItem"/> must have an available
    /// interaction point.</param>
    /// <returns>A randomly selected <see cref="ShelfItem"/> that meets the specified requirements, or null if
    /// no suitable item is found.</returns>
    public ShelfItem GetRandomShelfItem(bool requiresAvailability)
    {
        shelves.Shuffle();

        if (!requiresAvailability)
        {
            return shelves[0].GetRandomItem();
        }

        for (int i = 0; i < shelves.Count; i++)
        {
            List<ShelfItem> shelfItems = shelves[i].GetCurrentItems();
            shelfItems.Shuffle();

            for (int j = 0; j < shelfItems.Count; j++)
            {
                if (!shelfItems[j].InteractionPoint.IsOccupied)
                {
                    return shelfItems[j];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the total number of items currently placed across all shelves.
    /// </summary>
    /// <returns>The total number of items across all shelves.</returns>
    public int GetTotalItemCount()
    {
        int totalCount = 0;

        List<Shelf> nonEmptyShelves = GetShelves(hasItems: true);
        for (int i = 0; i < nonEmptyShelves.Count; i++)
        {
            totalCount += nonEmptyShelves[i].ItemCount;
        }

        return totalCount;
    }
}
