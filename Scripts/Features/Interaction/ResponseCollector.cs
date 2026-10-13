using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Centralized container for behavior components used by various interaction systems. Interaction scripts (such
/// as Hoverable, Interactable, Holdable, etc.) can request specific response types from this collector without
/// needing their own serialized lists.
/// </summary>
public class ResponseCollector : MonoBehaviour
{
    [Tooltip("If enabled, this will automatically collect all MonoBehaviour components on this GameObject "
        + "and its IMMEDIATE children only as potential responses. If disabled, the components can be manually "
        + "assigned in the Response Behaviours list.")]
    [SerializeField]
    private bool autoCollectResponses = true;
    
    [Tooltip("A list of behavior components that may implement one or more response interfaces. Interaction "
        + "scripts pull their required responses from this list (e.g. Hoverable pulls IHoverResponse objects).")]
    [HideIf(nameof(autoCollectResponses))]
    [SerializeField]
    private List<MonoBehaviour> responseBehaviours;

    /// <summary>
    /// A list of MonoBehaviour components that may implement one or more response interfaces. Interaction
    /// scripts query this list to retrieve the specific response types they need.
    /// </summary>
    public List<MonoBehaviour> ResponseBehaviours => responseBehaviours;

    /// <summary>
    /// Retrieves all components in <see cref="ResponseBehaviours"/> that implement the specified response
    /// interface from the current game object and only its IMMEDIATE children.
    /// </summary>
    /// <typeparam name="T">The response interface type to collect (e.g., IHoverResponse).</typeparam>
    /// <returns>A list of components implementing the requested response interface.</returns>
    public List<T> Collect<T>()
    {
        if (autoCollectResponses)
        {
            // Get all MonoBehaviour components on this game object and its immediate children to be filtered
            // for the specific response interface
            responseBehaviours = gameObject.GetComponentsInImmediateChildren<MonoBehaviour>().ToList();
        }

        List<T> responses = responseBehaviours.OfType<T>().ToList();
        return responses;
    }
}
