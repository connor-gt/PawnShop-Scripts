using UnityEngine;

public class OpenCloseSign : MonoBehaviour, IInteractResponse, IHoverResponse, IHoverMessageUser
{
    public string HoverMessage { get; set; } = "";

    private Hoverable hoverable;

    private void Awake()
    {
        hoverable = GetComponent<Hoverable>();
    }

    public void Interact()
    {
        PawnShop.Instance.OpenShop();
        hoverable.RequestRefresh();
    }

    public void OnFocus(IHoverHandler hoverHandler)
    {
        if (!PawnShop.Instance.IsOpen)
        {
            HoverMessage = InputManager.Instance.GetFormattedBindingDisplay(
                InputManager.Instance.Actions.Base.Interact, "Open the shop"
            );
        }
        else
        {
            HoverMessage = "The shop is already open!";
        }

        hoverHandler.HoverMessage.UpdateHoverText(HoverMessage);
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.ClearHoverText();
    }
}
