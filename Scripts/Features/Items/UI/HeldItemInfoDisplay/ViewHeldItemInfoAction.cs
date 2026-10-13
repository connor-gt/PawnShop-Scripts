using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides an action that displays generic information about the item currently being held.
/// </summary>
public class ViewHeldItemInfoAction : MonoBehaviour, IHoldAction
{
    [field: Title("Input Action")]
    [field: SerializeField]
    public HoldActionInputSO HoldActionInput { get; private set; }

    public TemporaryInputBinding InputBinding { get; private set; }

    private ItemInstance itemInstance;

    private void Awake()
    {
        InputBinding = new TemporaryInputBinding(HoldActionInput.InputReference.action, ShowHeldItemInfo);
    }

    // Start is used instead of Awake because held prefabs don't actually have ItemInstance components, they
    // are rather added during runtime by HoldState when the held prefabs are instantiated
    private void Start()
    {
        itemInstance = GetComponent<ItemInstance>();
    }

    private void OnDisable()
    {
        // Make sure to hide the info UI display once the player moves to a different item in their inventory
        if (HeldItemInfoUIController.Instance.IsActive)
        {
            HeldItemInfoUIController.Instance.Hide();
        }
    }

    /// <summary>
    /// Show this held item's information to the player by revealing the 2D UI element and assigning it it's
    /// information.
    /// </summary>
    public void ShowHeldItemInfo(InputAction.CallbackContext ctx)
    {
        if (HeldItemInfoUIController.Instance.IsActive)
        {
            HeldItemInfoUIController.Instance.Hide();
            return;
        }
        
        HeldItemInfoUIController.Instance.Show(ItemServices.CreateDetailedSellableItemInfo(itemInstance.Item));
    }
}
