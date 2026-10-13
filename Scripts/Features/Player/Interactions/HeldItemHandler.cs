using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller for handling player interactions with holdable objects in the world. This component is
/// responsible for managing the player's currently held item state, detecting interactions with holdable
/// objects, and delegating interaction responses to those objects as appropriate.
/// </summary>
public class HeldItemHandler : MonoBehaviour, IPickupHandler, IPlayerInventoryState
{
    [Title("References")]
    [SerializeField]
    private HoverableHandler hoverableHandler;

    [SerializeField]
    private InteractableHandler interactableHandler;

    [SerializeField]
    private PlayerInventory playerInventory;

    /// <summary>
    /// Gets the player's inventory.
    /// </summary>
    public Inventory Inventory => playerInventory.Inventory;

    /// <summary>
    /// Gets the transform location where the item's "held" prefab version is instantiated.
    /// </summary>
    [field: Tooltip("The transform location where the item's \"held\" prefab version is instantiated.")]
    [field: SerializeField]
    public Transform HoldTransform { get; private set; }

    /// <summary>
    /// Gets the current hold state.
    /// </summary>
    public HoldState HoldState { get; private set; }

    private void Awake()
    {
        HoldState = new HoldState(HoldTransform);

        if (playerInventory.Count > 0)
        {
            HoldState.HoldNewItem(playerInventory.CurrentItem);
        }
    }

    private void OnEnable()
    {
        // This controller does not perform its own raycast to detect holdables. Instead, it listens for
        // interactions detected by the InteractableHandler and checks whether the interacted object can be held
        interactableHandler.OnInteract += CheckInteraction;

        hoverableHandler.OnFocus += CheckOnFocus;
        hoverableHandler.OffFocus += CheckOffFocus;

        InputManager.Instance.Actions.Base.ScrollInventory.performed += ScrollInventory;
    }

    private void OnDisable()
    {
        if (interactableHandler != null)
        {
            interactableHandler.OnInteract -= CheckInteraction;
        }

        hoverableHandler.OnFocus -= CheckOnFocus;
        hoverableHandler.OffFocus -= CheckOffFocus;

        InputManager.Instance.Actions.Base.ScrollInventory.performed -= ScrollInventory;
    }

    /// <summary>
    /// Handles interaction events forwarded by <see cref="InteractableHandler"/>. Objects implementing
    /// <see cref="IPickupable"/> receive a pickup request, while objects implementing <see cref="IPlaceable"/>
    /// receive a placement request. This handler provides the <see cref="IPickupHandler"/> context used by
    /// those objects to request the appropriate inventory operation.
    /// </summary>
    /// <param name="interactable">The interactable object detected by the interaction system.</param>
    private void CheckInteraction(Interactable interactable)
    {
        if (interactable.gameObject.TryGetComponent(out IPickupable pickupable))
        {
            pickupable.Pickup(this);
        }
        else if (interactable.gameObject.TryGetComponent(out IPlaceable placeable))
        {
            placeable.Place(this);
        }
    }

    /// <summary>
    /// Called when the player begins hovering over a <see cref="Hoverable"/> object. If the hovered object
    /// contains a <see cref="PlaceableLocation"/> and no other placeable location is currently active, this
    /// method assigns it as the active placement target so that preview logic can be handled by
    /// <see cref="HoldState"/>.
    /// </summary>
    /// <param name="hoverable">The hovered object detected by the hover system.</param>
    private void CheckOnFocus(Hoverable hoverable)
    {
        if (HoldState.CurrentPlaceableLocation == null
            && hoverable.gameObject.TryGetComponent(out PlaceableLocation placeableLocation))
        {
            HoldState.CurrentPlaceableLocation = placeableLocation;
        }
    }

    /// <summary>
    /// Called when the player stops hovering over a <see cref="Hoverable"/> object. Clears the current
    /// placeable location from <see cref="HoldState"/>, ensuring that any active placement preview is hidden
    /// and no longer considered valid.
    /// </summary>
    /// <param name="hoverable">The object that is no longer being hovered.</param>
    private void CheckOffFocus(Hoverable hoverable)
    {
        HoldState.CurrentPlaceableLocation = null;
    }

    /// <summary>
    /// Attempts to pick up the given <see cref="Holdable"/>. If the inventory has space, the item is added
    /// and the world instance is consumed. If this is the first item, it becomes the currently held item.
    /// </summary>
    /// <param name="holdable">The world item being picked up.</param>
    public void RequestPickup(Holdable holdable)
    {
        if (Inventory.IsFull)
        {
            return;
        }

        playerInventory.Add(holdable.Item);
        holdable.ConsumeWorldInstance();

        if (playerInventory.Count == 1)
        {
            HoldNewItem(playerInventory.CurrentItem);
        }
    }

    /// <summary>
    /// Attempts to place the currently held item into the specified location. If placement succeeds, the
    /// item is removed from the inventory and the next item (if any) becomes the held item. If successful
    /// placement leaves the inventory empty, the held state is cleared. Invalid placement leaves it unchanged.
    /// </summary>
    /// <param name="placeableLocation">The location to place the held item into.</param>
    public void RequestPlace(PlaceableLocation placeableLocation)
    {
        if (!HoldState.IsHolding || !placeableLocation.CanPlace(HoldState.CurrentlyHeldItem))
        {
            return;
        }

        placeableLocation.Place(HoldState.CurrentlyHeldItem);
        playerInventory.RemoveCurrentItem();

        // Hold the next item in the inventory if it exists. Otherwise, hold nothing
        if (playerInventory.HasCurrentItem)
        {
            HoldNewItem(playerInventory.CurrentItem);
        }
        else
        {
            ClearCurrentItem();
        }
    }

    /// <summary>
    /// Cycles the active inventory slot based on scroll input and updates the held item and any active
    /// placement preview accordingly.
    /// </summary>
    private void ScrollInventory(InputAction.CallbackContext ctx)
    {
        if (playerInventory.Count <= 1)
        {
            return;
        }

        // Scroll down == next, scroll up == previous. After doing testing, scrolling down to get to the next
        // item in the inventory feels more natural
        float scrollValue = ctx.ReadValue<Vector2>().y;
        if (scrollValue < 0)
        {
            playerInventory.Next();
        }
        else
        {
            playerInventory.Previous();
        }

        HoldNewItem(playerInventory.CurrentItem);
    }

    /// <summary>
    /// Replaces the currently held item with the specified item.
    /// <para>
    /// Any actions belonging to the previously held item are disabled before the held object is replaced.
    /// If the new held object contains <see cref="HeldItemActions"/>, its actions are enabled.
    /// </para>
    /// </summary>
    /// <param name="item">The item to begin holding.</param>
    private void HoldNewItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        // Disable the previous item's actions before replacing it so its input bindings do not remain active
        // after the player has switched to a different held item
        if (HoldState.IsHolding && HoldState.CurrentlyHeldObject.TryGetComponent(out HeldItemActions itemActions))
        {
            itemActions.DisableActions();
        }

        GameObject heldItem = HoldState.HoldNewItem(item);
        HoldState.RefreshPlaceableLocationPreview();

        // Provides the new held item with the player's hover context (if it needs it) so that systems that
        // depend on the player's current hover state have access to it (e.g., Scanner.cs)
        if (heldItem.TryGetComponent(out IHoverContextReceiver contextRequester))
        {
            contextRequester.Inject(hoverableHandler);
        }

        // Enable the new item's actions and update the UI (if the item has actions that can be performed while
        // being held)
        if (heldItem.TryGetComponent(out HeldItemActions newItemActions))
        {
            HeldItemActionsUIController.Instance.SetNewActionsText(newItemActions.GetHoldActions());
            newItemActions.EnableActions();
        }
        // Don't display any actions text if the held item has no actions associated with it
        else
        {
            HeldItemActionsUIController.Instance.Clear();
        }
    }

    /// <summary>
    /// Clears the currently held item.
    /// </summary>
    private void ClearCurrentItem()
    {
        HoldState.Clear();
        HeldItemActionsUIController.Instance.Clear();
    }
}
