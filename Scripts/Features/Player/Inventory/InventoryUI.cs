using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Title("Main Inventory Slots")]
    [Tooltip("The image that shows the inventory's previous item icon.")]
    [SerializeField]
    private Image previous;

    [Tooltip("The image that shows the inventory's currently selected item icon.")]
    [SerializeField]
    private Image current;

    [Tooltip("The image that shows the inventory's next item icon.")]
    [SerializeField]
    private Image next;

    [Title("Dot Indicators")]
    [Tooltip("Prefab used to create each inventory dot indicator.")]
    [SerializeField]
    private GameObject dotIndicatorPrefab;

    [Tooltip("The parent transform that holds all dot indicators.")]
    [SerializeField]
    private Transform dotIndCanvasParent;

    [Tooltip("Color used for filled inventory dots.")]
    [SerializeField]
    private Color filledColor = Color.lightGray;

    [Tooltip("Color used for empty inventory dots.")]
    [SerializeField]
    private Color emptyColor = Color.darkGray;

    [Tooltip("Glow image that highlights the currently selected dot.")]
    [SerializeField]
    private Image currentIndicatorGlow;

    /// <summary>
    /// The indicators that go under the item preview icons.
    /// </summary>
    private readonly List<Image> dotIndicators = new();

    // Lazy initialization since it's unknown when exactly the player will spawn
    private PlayerInventory _playerInventory = null;
    private PlayerInventory PlayerInventory
    {
        get
        {
            if (_playerInventory == null && PawnShop.Instance != null && PawnShop.Instance.Player != null)
            {
                _playerInventory = PawnShop.Instance.Player.GetComponentInChildren<PlayerInventory>();
            }

            return _playerInventory;
        }
    }

    private IEnumerator Start()
    {
        while (PlayerInventory == null)
        {
            yield return null;
        }

        PlayerInventory.OnUpdated += UpdateUI;
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (PlayerInventory != null)
        {
            PlayerInventory.OnUpdated -= UpdateUI;
        }
    }
    
    /// <summary>
    /// Updates all inventory UI elements to reflect the player's current inventory state.
    /// </summary>
    public void UpdateUI()
    {
        UpdateIcons();
        UpdateDotIndicators();
    }

    /// <summary>
    /// Refreshes the inventory preview by displaying the icons for the currently selected item, as well as
    /// the adjacent previous and next items when available.
    /// </summary>
    public void UpdateIcons()
    {
        current.color = new Color(current.color.r, current.color.g, current.color.b, 0);
        previous.color = new Color(previous.color.r, previous.color.g, previous.color.b, 0);
        next.color = new Color(next.color.r, next.color.g, next.color.b, 0);

        Item currentItem = PlayerInventory.CurrentItem;
        if (currentItem != null)
        {
            current.sprite = currentItem.Data.HoldableData.InventoryIcon;
            current.color = new Color(current.color.r, current.color.g, current.color.b, 1);
        }

        Item nextItem = PlayerInventory.PeekNext();
        if (nextItem != null && nextItem != currentItem)
        {
            next.sprite = nextItem.Data.HoldableData.InventoryIcon;
            next.color = new Color(next.color.r, next.color.g, next.color.b, 1);
        }

        Item previousItem = PlayerInventory.PeekPrevious();
        if (previousItem != null && previousItem != currentItem && previousItem != nextItem)
        {
            previous.sprite = previousItem.Data.HoldableData.InventoryIcon;
            previous.color = new Color(previous.color.r, previous.color.g, previous.color.b, 1);
        }
    }

    /// <summary>
    /// Updates the dot indicators to match the inventory's capacity and current item count. This also rebuilds
    /// the indicator list if the inventory size changes and highlights the active slot.
    /// </summary>
    public void UpdateDotIndicators()
    {
        // If the inventory size changes, make sure to update the amount of indicators as well
        if (dotIndicators.Count != PlayerInventory.MaxItems)
        {
            for (int i = dotIndicators.Count - 1; i >= 0; i--)
            {
                Destroy(dotIndicators[i].gameObject);
                dotIndicators.RemoveAt(i);
            }

            for (int i = 0; i < PlayerInventory.MaxItems; i++)
            {
                GameObject newDotIndicator = Instantiate(dotIndicatorPrefab, dotIndCanvasParent);
                Image dotIndicator = newDotIndicator.GetComponent<Image>();
                dotIndicators.Add(dotIndicator);
            }
        }

        for (int i = 0; i < dotIndicators.Count; i++)
        {
            if (PlayerInventory.Count > 0 && i < PlayerInventory.Count)
            {
                dotIndicators[i].color = filledColor;

                if (i == PlayerInventory.CurrentIndex)
                {
                    // TODO: MAKE HIGHLIGHTED
                    dotIndicators[i].color = Color.green;
                }
            }
            else
            {
                dotIndicators[i].color = emptyColor;
            }
        }
    }
}
