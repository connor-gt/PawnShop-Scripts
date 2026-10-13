using UnityEngine;

/// <summary>
/// Restricts a <see cref="PlaceableLocation"/> to <see cref="Item"/>s with a specific <see cref="ItemDataSO"/>.
/// </summary>
public class PLRestrictToItemData : MonoBehaviour, IPlaceableLocationRestriction
{
    [Tooltip("Items must have this data asset to be placed at this placeable location.")]
    [SerializeField]
    private ItemDataSO data;

    public bool MeetsRestriction(Item item)
    {
        return item.Data == data;
    }
}
