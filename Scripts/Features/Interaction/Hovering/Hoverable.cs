using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hoverable object that forwards focus events to one or more attached <see cref="IHoverResponse"/> behaviors.
/// Each response component defines how this object should visually or functionally react when the player begins
/// or stops hovering over it.
/// </summary>
[RequireComponent(typeof(ResponseCollector))]
public class Hoverable : MonoBehaviour, IHoverResponse
{
    /// <summary>
    /// Invoked when this <see cref="Hoverable"/>'s hover information changes and should be refreshed.
    /// </summary>
    public event Action OnRequestRefresh;

    private ResponseCollector responseCollector;
    private List<IHoverResponse> responses;

    private void Awake()
    {
        responseCollector = GetComponent<ResponseCollector>();
        responses = responseCollector.Collect<IHoverResponse>();

        if (responses.Contains(this))
        {
            responses.Remove(this);
        }
    }

    /// <summary>
    /// Loop through all attached <see cref="IHoverResponse"/> components and call their
    /// <see cref="IHoverResponse.OnFocus"/> method to trigger their hover response behavior.
    /// </summary>
    /// <param name="context">The context of the hover event</param>
    public void OnFocus(IHoverHandler context)
    {
        for (int i = 0; i < responses.Count; i++)
        {
            responses[i].OnFocus(context);
        }
    }

    /// <summary>
    /// Loop through all attached <see cref="IHoverResponse"/> components and call their
    /// <see cref="IHoverResponse.OffFocus"/> method to trigger their hover response behavior.
    /// </summary>
    /// <param name="context">The context of the hover event</param>
    public void OffFocus(IHoverHandler context)
    {
        for (int i = 0; i < responses.Count; i++)
        {
            responses[i].OffFocus(context);
        }
    }

    /// <summary>
    /// Requests that any object currently hovering this <see cref="Hoverable"/> refresh its hover state.
    /// </summary>
    public void RequestRefresh()
    {
        OnRequestRefresh?.Invoke();
    }
}
