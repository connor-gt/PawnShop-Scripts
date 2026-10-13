using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [Title("Shelf Slot Locations")]
    [SerializeField]
    private PlaceableLocation[] placeableLocations;

#if UNITY_EDITOR
    [Title("Debug Interaction Mappings")]
    [SerializeField]
    private bool showMappingConnections;

    private readonly InteractionMappingDebugger mappingDebugger = new();
#endif

    [Title("Interaction Mappings")]
    [Tooltip("Use this list to map placeable locations to interaction points, making sure that NPCs don't "
        + "overcrowd or overlap one spot trying to browse the same item (or different items but on different"
        + " levels of the shelves).")]
    [SerializeField]
    private List<InteractionMapping<PlaceableLocation>> locationMappings;

    private readonly List<ShelfItem> currentShelfItems = new();
    private readonly List<InteractionPoint> interactionPoints = new();

    /// <summary>
    /// Gets the current number of items on this shelf.
    /// </summary>
    public int ItemCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < placeableLocations.Length; i++)
            {
                if (!placeableLocations[i].IsEmpty)
                {
                    count++;
                }
            }

            return count;
        }
    }

    private void Start()
    {
        foreach (var locationMapping in locationMappings)
        {
            interactionPoints.Add(locationMapping.Point);
        }

        PawnShop.Instance.ShelfController.RegisterShelf(this);
    }

    private void OnEnable()
    {
        for (int i = 0; i < placeableLocations.Length; i++)
        {
            placeableLocations[i].OnUpdated += OnPlaceableLocationUpdated;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < placeableLocations.Length; i++)
        {
            if (placeableLocations[i] != null)
            {
                placeableLocations[i].OnUpdated -= OnPlaceableLocationUpdated;
            }
        }
    }

    /// <summary>
    /// Get a snapshot list of the current <see cref="ShelfItem"/>s on this shelf. Since the returned list is a
    /// snapshot, it is best to make sure this shelf still has the particular item via <see cref="HasItem"/> if
    /// it's going to be used for anything at a later point after this method has been called.
    /// </summary>
    /// <returns>A list of the current </returns>
    public List<ShelfItem> GetCurrentItems()
    {
        RefreshCurrentShelfItemsList();
        return currentShelfItems.ToList();
    }

    /// <summary>
    /// Get a random <see cref="ShelfItem"/> from this shelf.
    /// </summary>
    /// <returns>A randomly selected <see cref="ShelfItem"/> or null if the shelf is empty.</returns>
    public ShelfItem GetRandomItem()
    {
        RefreshCurrentShelfItemsList();

        if (currentShelfItems.Count == 0)
        {
            return null;
        }

        return currentShelfItems[Random.Range(0, currentShelfItems.Count)];
    }

    /// <summary>
    /// Refreshes the <see cref="currentShelfItems"/> list. This method should be called anytime an action needs
    /// to be performed on the list.
    /// </summary>
    private void RefreshCurrentShelfItemsList()
    {
        currentShelfItems.Clear();

        for (int i = 0; i < placeableLocations.Length; i++)
        {
            if (!placeableLocations[i].IsEmpty)
            {
                Holdable holdable = placeableLocations[i].CurrentHoldable;
                InteractionPoint interactionPoint = GetInteractionPointForItem(holdable);

                ShelfItem shelfItem = new(holdable, interactionPoint);
                currentShelfItems.Add(shelfItem);
            }
        }
    }

    /// <summary>
    /// Check to see if this shelf currently has an item via its <see cref="ItemDataSO"/>.
    /// </summary>
    /// <param name="itemData">The item to check if its on this shelf.</param>
    /// <returns>True if the item is currently on the shelf, false if otherwise.</returns>
    public bool HasItem(ItemDataSO itemData)
    {
        for (int i = 0; i < placeableLocations.Length; i++)
        {
            if (!placeableLocations[i].IsEmpty && placeableLocations[i].CurrentHoldable.Item.Data == itemData)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Retrieves the <see cref="InteractionPoint"/> associated with the specified <see cref="Holdable"/>
    /// instance.
    /// </summary>
    /// <param name="holdable">The specific <see cref="Holdable"/> instance.</param>
    /// <returns>The <see cref="InteractionPoint"/> mapped to the given holdable instance, or <c>null</c> if
    /// the holdable is not present on this shelf.</returns>
    public InteractionPoint GetInteractionPointForItem(Holdable holdable)
    {
        foreach (var locationMapping in locationMappings)
        {
            foreach (var placeableLocation in locationMapping.Interactions)
            {
                if (placeableLocation.CurrentHoldable != null && placeableLocation.CurrentHoldable == holdable)
                {
                    return locationMapping.Point;
                }
            }
        }
        
        return null;
    }

    /// <summary>
    /// Gets the <see cref="InteractionPoint"/> associated with the specified <see cref="ItemDataSO"/> on this
    /// shelf.
    /// <para>
    /// <b>Note:</b> This lookup is based on <see cref="ItemDataSO"/> identity rather than a specific
    /// <see cref="Holdable"/> instance. Because multiple holdable instances may share the same item data, this
    /// method may return different interaction points across calls for the same <see cref="ItemDataSO"/> if
    /// more than one item on this shelf shares the exact same data.
    /// </para>
    /// </summary>
    /// <param name="itemData">The item whose corresponding interaction point should be located.</param>
    /// <returns>The <see cref="InteractionPoint"/> mapped to the item if it is mapped to this shelf, null if
    /// otherwise.</returns>
    public InteractionPoint GetInteractionPointForItem(ItemDataSO itemData)
    {
        List<InteractionMapping<PlaceableLocation>> matchingMappings = new();

        foreach (var locationMapping in locationMappings)
        {
            foreach (var placeableLocation in locationMapping.Interactions)
            {
                if (placeableLocation.CurrentHoldable != null
                    && placeableLocation.CurrentHoldable.Item.Data == itemData)
                {
                    matchingMappings.Add(locationMapping);

                    // Avoid including duplicate mappings if a single mapping contains 2 placeable locations
                    // that have the same item
                    break; 
                }
            }
        }

        if (matchingMappings.Count == 0)
        {
            return null;
        }

        // Multiple shelf locations may contain items using the same ItemDataSO. Select one of their mapped
        // interaction points at random (note this does not affect instances where there is only one item of the
        // input ItemDataSO on the shelf)
        var randomMapping = matchingMappings[Random.Range(0, matchingMappings.Count)];
        return randomMapping.Point;
    }

    /// <summary>
    /// Get a copy of this shelf's <see cref="PlaceableLocation"/>s.
    /// </summary>
    /// <returns>A list of <see cref="PlaceableLocation"/>s.</returns>
    public List<PlaceableLocation> GetPlaceableLocations()
    {
        return placeableLocations.ToList();
    }

    /// <summary>
    /// Updates the item information popup content when the contents of a placeable location changes.
    /// </summary>
    /// <param name="holdable">The <see cref="Holdable"/> that was added or updated, or null if an item was removed.
    /// </param>
    private void OnPlaceableLocationUpdated(Holdable holdable)
    {
        if (holdable == null)
        {
            return;
        }

        if (holdable.TryGetComponent(out IItemInfoPopupUser itemInfoPopupUser))
        {
            itemInfoPopupUser.SetContentProvider(ItemServices.CreateListedItemInfo);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showMappingConnections)
        {
            mappingDebugger.ShowConnections(locationMappings);
        }
    }
#endif
}
