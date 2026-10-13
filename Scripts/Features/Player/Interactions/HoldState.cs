using UnityEngine;

/// <summary>
/// Maintains all runtime data related to the player's held item.
/// </summary>
public class HoldState
{
    /// <summary>
    /// Gets the transform location where the currently held item's game object is instantiated.
    /// </summary>
    public readonly Transform HoldTransform;

    /// <summary>
    /// Gets the item the player is currently holding. This is null if the player is not currently holding
    /// anything.
    /// </summary>
    public Item CurrentlyHeldItem { get; private set; } = null;

    /// <summary>
    /// Gets the currently held game object. If this is not null, this was instantiated via
    /// <see cref="CurrentlyHeldItem"/>'s holdable data prefab.
    /// </summary>
    public GameObject CurrentlyHeldObject { get; private set; } = null;

    /// <summary>
    /// Gets whether or not an item is currently being held.
    /// </summary>
    public bool IsHolding => CurrentlyHeldItem != null;

    private PlaceableLocation _currentPlaceableLocation = null;

    /// <summary>
    /// Gets or sets the placeable location currently hovered over by the player. Assigning a new location
    /// automatically handles preview lifecycle management.
    /// </summary>
    public PlaceableLocation CurrentPlaceableLocation
    {
        get => _currentPlaceableLocation;
        set
        {
            if (value == null)
            {
                if (_currentPlaceableLocation != null)
                {
                    _currentPlaceableLocation.HidePreview();
                }
                
                _currentPlaceableLocation = null;
                return;
            }

            if (value.IsEmpty && CurrentlyHeldItem != null)
            {
                value.ShowPreview(CurrentlyHeldItem);
            }

            _currentPlaceableLocation = value;
        }
    }

    /// <summary>
    /// Gets whether or not the currently held item can be placed.
    /// </summary>
    public bool CanPlace => CurrentPlaceableLocation != null;

    /// <summary>
    /// Creates a new <see cref="HoldState"/> instance bound to the specified <paramref name="holdTransform"/>.
    /// All held item visuals will be instantiated as children of this transform.
    /// </summary>
    /// <param name="holdTransform">The transform where held item prefabs should be attached.</param>
    public HoldState(Transform holdTransform)
    {
        HoldTransform = holdTransform;
    }

    /// <summary>
    /// Transitions the hold state into a "holding" state by picking up the specified <see cref="Holdable"/>
    /// item. The world instance of the item is not deleted and is expected to be destroyed by the caller if
    /// needed.
    /// </summary>
    /// <param name="holdable">The world item being picked up.</param>
    /// <returns>The instantiated held item game object.</returns>
    public GameObject HoldNewItem(Holdable holdable)
    {
        return HoldNewItem(holdable.Item);
    }

    /// <inheritdoc cref="HoldNewItem(Holdable)"/>
    /// <param name="item">The item to instantiate as a held object.</param>
    public GameObject HoldNewItem(Item item)
    {
        Clear();

        CurrentlyHeldItem = item;
        CurrentlyHeldObject = Object.Instantiate(CurrentlyHeldItem.Data.HoldableData.HeldPrefab, HoldTransform);

        // Make sure the instantiated held item prefab has the runtime identity of the actual item. The downside
        // injecting the identity like this and adding the component is that other scripts cannot access itemInstance
        // in Awake(), but it's worth the trade off so I don't have to manually add this component to all held prefabs
        ItemInstance itemInstance = CurrentlyHeldObject.GetOrAddComponent<ItemInstance>();
        itemInstance.SetItem(item);

        if (item.Data.HoldableData.OverridePosition)
        {
            CurrentlyHeldObject.transform.localPosition = CurrentlyHeldItem.Data.HoldableData.Offset;
            CurrentlyHeldObject.transform.localEulerAngles = CurrentlyHeldItem.Data.HoldableData.Rotation;
        }

        return CurrentlyHeldObject;
    }

    /// <summary>
    /// Resets the hold state by destroying the currently held visual object (if any) and clearing all held
    /// item data. After calling this method, the player is considered to be holding nothing.
    /// </summary>
    public void Clear()
    {
        if (CurrentlyHeldObject != null)
        {
            Object.Destroy(CurrentlyHeldObject);
        }

        CurrentlyHeldObject = null;
        CurrentlyHeldItem = null;
    }
    
    /// <summary>
    /// Places the currently held item into the world at the specified location.
    /// </summary>
    /// <param name="placeLocation">The transform whose position and rotation determines where the item is
    /// placed.</param>
    /// <returns>The instantiated world <see cref="Holdable"/> object.</returns>
    public Holdable PlaceCurrentItem(Transform placeLocation)
    {
        Holdable created = Object.Instantiate(
            CurrentlyHeldItem.Data.WorldPrefab,
            placeLocation.position,
            placeLocation.rotation
        ).GetComponent<Holdable>();

        Clear();

        return created;
    }

    /// <summary>
    /// Refreshes the preview that's shown on <see cref="PlaceableLocation"/>s of the currently held item in
    /// the world.
    /// </summary>
    public void RefreshPlaceableLocationPreview()
    {
        if (CurrentPlaceableLocation == null)
        {
            return;
        }

        CurrentPlaceableLocation.HidePreview();

        if (IsHolding)
        {
            CurrentPlaceableLocation.ShowPreview(CurrentlyHeldItem);
        }
    }
}
