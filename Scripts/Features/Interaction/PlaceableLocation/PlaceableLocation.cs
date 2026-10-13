using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// A player-interactable surface where <see cref="Item"/> objects can be placed and later picked up.
/// </summary>
public class PlaceableLocation : MonoBehaviour, IPlaceable
{
    /// <summary>
    /// This event is fired off when a <see cref="Holdable"/> is added or removed from this location. If a
    /// <see cref="Holdable"/> is removed, null is returned.
    /// </summary>
    public event Action<Holdable> OnUpdated;

    [Title("Collider")]
    [Tooltip("The collider for this placeable location. This is used to detect when an item is placed in "
        + "this location and is turned off/on when an item is placed/removed.")]
    [SerializeField]
    private Collider _collider;

    [Title("Optional Starting Item")]
    [Tooltip("An optional item that will be placed in this location in Awake().")]
    [SerializeField]
    private Item startingItem;

// For visualizing placeable locations within the scene view
#if UNITY_EDITOR
    private GizmosDrawer gizmosDrawer;

    [Title("Gizmos Colors")]
    [Tooltip("The color to use for the gizmo when this location has a starting item.")]
    [SerializeField]
    private Color hasStartingItemColor = new Color(0f, 0.95f, 0f, 0.3f);

    [Tooltip("The color to use for the gizmo when this location does not have a starting item.")]
    [SerializeField]
    private Color noStartingItemColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
#endif

    private IPlaceableLocationRestriction[] restrictions;

    /// <summary>
    /// Gets the current <see cref="Holdable"/> that is occupying this <see cref="PlaceableLocation"/>.
    /// </summary>
    public Holdable CurrentHoldable { get; private set; }

    /// <summary>
    /// Gets whether or not this <see cref="PlaceableLocation"/> is empty.
    /// </summary>
    public bool IsEmpty => CurrentHoldable == null;

    /// <summary>
    /// Gets the current game object that is being shown as the preview. If null, there is no preview being shown.
    /// </summary>
    public GameObject PreviewObject { get; private set; } = null;

    private void Awake()
    {
        restrictions = GetComponents<IPlaceableLocationRestriction>();

        if (startingItem.Data != null)
        {
            Place(startingItem);
        }
    }

    /// <summary>
    /// Returns whether the given item can be placed in this location.
    /// </summary>
    /// <param name="item">The item being checked for placement.</param>
    /// <returns>True if the location is empty and the item meets all restrictions (if there are any).</returns>
    public bool CanPlace(Item item)
    {
        if (!IsEmpty)
        {
            return false;
        }

        // Acts as a AND behavior for the various restrictions (if there are any)
        for (int i = 0; i < restrictions.Length; i++)
        {
            if (!restrictions[i].MeetsRestriction(item))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Displays a valid or invalid placement preview for the given item.
    /// </summary>
    /// <param name="item">The item to preview placement for.</param>
    public void ShowPreview(Item item)
    {
        if (CanPlace(item))
        {
            PreviewObject = Instantiate(item.Data.HoldableData.ValidPreviewPrefab, transform);
        }
        else
        {
            PreviewObject = Instantiate(item.Data.HoldableData.InvalidPreviewPrefab, transform);
        }

        PreviewObject.transform.rotation = transform.rotation;
    }

    /// <summary>
    /// Hides the preview object, if it exists.
    /// </summary>
    public void HidePreview()
    {
        if (PreviewObject != null)
        {
            Destroy(PreviewObject);
            PreviewObject = null;
        }
    }

    /// <summary>
    /// Places the given item in this location, if it is valid to do so.
    /// </summary>
    /// <param name="item">The item to place.</param>
    /// <returns>The placed item, or null if the placement was invalid and nothing was actually placed.</returns>
    public Holdable Place(Item item)
    {
        if (!CanPlace(item))
        {
            return null;
        }

        // Disable the collider so it doesn’t intercept the player’s raycast when the placed item’s collider is smaller
        _collider.enabled = false;
        HidePreview();

        Holdable created = item.InstantiateWorldPrefab(transform).GetComponent<Holdable>();
        created.transform.rotation = transform.rotation;
        CurrentHoldable = created;
        OnUpdated?.Invoke(created);

        // Handles re-enabling the collider when the currently placed item is picked up
        created.OnPickedUp += OnCurrentHoldablePickedUp;

        return created;
    }

    /// <summary>
    /// Handles placing a holdable item at this <see cref="PlaceableLocation"/> when the player interacts with it.
    /// </summary>
    /// <param name="context">The hold context (needed for getting the currently held object).</param>
    public void Place(IPickupHandler context)
    {
        context.RequestPlace(this);
    }

    /// <summary>
    /// Subscribes to the <see cref="Holdable.OnPickedUp"/> event of the currently placed <see cref="CurrentHoldable"/>
    /// in order to re-enable this location's collider when the item is picked up.
    /// </summary>
    /// <param name="holdable">The holdable item that was placed here and is now being picked up.</param>
    private void OnCurrentHoldablePickedUp(Holdable holdable)
    {
        CurrentHoldable = null;
        _collider.enabled = true;
        OnUpdated?.Invoke(null);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gizmosDrawer == null)
        {
            gizmosDrawer = GetComponentInChildren<GizmosDrawer>();
            if (gizmosDrawer == null)
            {
                return;
            }
        }

        // If this location has a starting item, change the gizmo color to green. Otherwise, use the default color
        gizmosDrawer.Color = startingItem.Data != null ? hasStartingItemColor : noStartingItemColor;
    }
#endif
}
