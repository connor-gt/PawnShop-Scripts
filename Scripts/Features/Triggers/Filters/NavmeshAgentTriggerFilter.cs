using UnityEngine;
using UnityEngine.AI;

public class NavmeshAgentTriggerFilter : MonoBehaviour, ITriggerFilter
{
    /// <summary>
    /// Checks if the collider that entered the <see cref="TriggerDetector"/> has a <see cref="NavMeshAgent"/>
    /// component attached to it.
    /// </summary>
    /// <param name="other">The collider that entered the detector.</param>
    /// <returns>True if the collider has a <see cref="NavMeshAgent"/> component attached to it, false if
    /// otherwise.</returns>
    public bool Matches(Collider other)
    {
        return other.GetComponent<NavMeshAgent>() != null;
    }
}
