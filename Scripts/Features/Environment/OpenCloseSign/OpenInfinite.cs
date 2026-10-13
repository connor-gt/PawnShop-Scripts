using UnityEngine;

/*
 * THIS SCRIPT SHOULD EVENTUALLY BE DELETED
 */

/// <summary>
/// Counter part to <see cref="OpenCloseSign"/> that allows for infinite playtime disregarding day phases.
/// </summary>
public class OpenInfinite : MonoBehaviour, IInteractResponse, IHoverResponse, IHoverMessageUser
{
    public string HoverMessage { get; set; } = "";

    private Hoverable hoverable;

    private void Awake()
    {
        hoverable = GetComponent<Hoverable>();
    }

    public void Interact()
    {
#if UNITY_EDITOR
        Debug.Log("Infinite shop!");
#endif
        PawnShop.Instance.EntityController.EnableSpawning(true);
        hoverable.RequestRefresh();
    }

    public void OnFocus(IHoverHandler hoverHandler)
    {
        if (!PawnShop.Instance.IsOpen)
        {
            HoverMessage = InputManager.Instance.GetFormattedBindingDisplay(
                InputManager.Instance.Actions.Base.Interact, "Open the shop (infinite)"
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
