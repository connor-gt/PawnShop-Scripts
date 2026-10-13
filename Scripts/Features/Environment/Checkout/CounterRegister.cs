using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CounterRegister : MonoBehaviour, IInteractResponse, IHoverResponse
{
    /// <summary>
    /// Gets the amount of cash that is currently in the register.
    /// </summary>
    [field: Title("Values - Do Not Edit")]
    [field: SerializeField]
    public int CashInRegister { get; private set; } = 0;

    public void OnFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.UpdateHoverText($"Current cash: {CashInRegister}");
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.ClearHoverText();
    }

    public void Interact() { }

    public void AddCash(int amount)
    {
        CashInRegister += amount;
    }

    public void SubtractCash(int amount)
    {
        CashInRegister -= amount;
    }
}
