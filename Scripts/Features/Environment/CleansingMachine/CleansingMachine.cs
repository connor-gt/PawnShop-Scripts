using UnityEngine;

public class CleansingMachine : MonoBehaviour, IInteractResponse, IHoverResponse, IHoverMessageUser
{
    public string HoverMessage { get; set; }

    private IHoverHandler hoverHandler;

    public void Interact()
    {
        HoldState currentHoldState = hoverHandler.PlayerInventoryState.HoldState;

        if (currentHoldState.CurrentlyHeldItem != null)
        {
            Debug.Log($"Cleansing {currentHoldState.CurrentlyHeldItem.Data.Name}");
        }
    }

    public void OnFocus(IHoverHandler hoverHandler)
    {
        this.hoverHandler = hoverHandler;
        HoverMessage = "Cleansing Machine";
        hoverHandler.HoverMessage.UpdateHoverText(HoverMessage);
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.ClearHoverText();
        this.hoverHandler = null;
    }
}
