using UnityEngine;

/// <summary>
/// Restricts a <see cref="PlaceableLocation"/> to sellable <see cref="Item"/> objects.
/// </summary>
public class PLRestrictToSellables : MonoBehaviour, IPlaceableLocationRestriction
{
    public bool MeetsRestriction(Item item)
    {
        return item.IsSellable;
    }
}
