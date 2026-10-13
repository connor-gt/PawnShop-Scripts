using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interactable object that forwards interact events to one or more attached <see cref="IInteractResponse"/>
/// behaviors. Each response component defines how this object should visually or functionally react when the
/// player interacts with it.
/// </summary>
[RequireComponent(typeof(ResponseCollector))]
public class Interactable : MonoBehaviour, IInteractResponse
{
    private ResponseCollector responseCollector;
    private List<IInteractResponse> responses;

    private void Awake()
    {
        responseCollector = GetComponent<ResponseCollector>();
        responses = responseCollector.Collect<IInteractResponse>();

        if (responses.Contains(this))
        {
            responses.Remove(this);
        }
    }

    /// <summary>
    /// Loop through all attached <see cref="IInteractResponse"/> components and call their
    /// <see cref="IInteractResponse.Interact"/> method to trigger their interact response behavior.
    /// </summary>
    public void Interact()
    {
        for (int i = 0; i < responses.Count; i++)
        {
            responses[i].Interact();
        }
    }
}
