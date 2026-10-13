using TMPro;
using UnityEngine;

/// <summary>
/// Controls the gameplay UI that displays the actions available for the currently held item.
/// </summary>
public class HeldItemActionsUIController : MonoBehaviour
{
    public static HeldItemActionsUIController Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI[] actionsText;

    /// <summary>
    /// Gets the maximum number of actions that can be displayed in the UI.
    /// </summary>
    public int MaxActions => actionsText.Length;

    /// <summary>
    /// Keeps track of the current index of <see cref="actionsText"/>.
    /// </summary>
    private int currentIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Clear();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Clears the currently displayed actions and displays the specified held item actions.
    /// <para>
    /// Actions are displayed in the order they appear in the provided array. If more actions are provided than
    /// there are configured UI text elements, the remaining actions are not displayed.
    /// </para>
    /// </summary>
    /// <param name="heldItemActions"></param>
    public void SetNewActionsText(IHoldAction[] heldItemActions)
    {
        Clear();

        for (int i = 0; i < heldItemActions.Length; i++)
        {
            if (!TryAddAction(heldItemActions[i]))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Adding too held item actions to {gameObject.name}", this);
#endif
                break;
            }
        }
    }

    /// <summary>
    /// Attempts to display the specified held item action using the next available UI text element.
    /// </summary>
    /// <param name="heldItemAction">The held item action to display.</param>
    /// <returns>True if the action was displayed, false if otherwise (meaning no UI text elements were available).
    /// </returns>
    public bool TryAddAction(IHoldAction heldItemAction)
    {
        if (currentIndex >= actionsText.Length)
        {
            return false;
        }

        actionsText[currentIndex].text = InputManager.Instance.GetFormattedBindingDisplay(
            heldItemAction.HoldActionInput.InputReference.action, heldItemAction.HoldActionInput.Label
        );

        actionsText[currentIndex].gameObject.SetActive(true);
        currentIndex++;
        return true;
    }

    /// <summary>
    /// Clears all currently displayed held item actions from the UI.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < actionsText.Length; i++)
        {
            actionsText[i].text = "";
            actionsText[i].gameObject.SetActive(false);
        }

        currentIndex = 0;
    }
}
