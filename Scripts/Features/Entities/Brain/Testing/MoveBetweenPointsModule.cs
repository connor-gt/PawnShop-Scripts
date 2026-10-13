using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Module that's used to provide scene-specific locations for a <see cref="MoveBetweenPointsBrain"/>.
/// </summary>
public class MoveBetweenPointsModule : MonoBehaviour
{
    /// <summary>
    /// Gets the locations this NPC should move between.
    /// </summary>
    [field: Title("Locations to Walk Between")]
    [field: Tooltip("This NPC will walk between these locations in order.")]
    [field: SerializeField]
    public Transform[] Locations { get; private set; }

    /// <summary>
    /// The index of the next location to move to.
    /// </summary>
    private int locationsIndex;

    private void Awake()
    {
        // Starts as -1 so when GetNext() is called for the first time, it increments to 0
        locationsIndex = -1;
    }

    /// <summary>
    /// Gets the next location in <see cref="Locations"/>, cycling back to the first location after reaching the end.
    /// </summary>
    /// <returns>The next location this NPC should move to.</returns>
    public Transform GetNext()
    {
        locationsIndex = (locationsIndex + 1) % Locations.Length;
        return Locations[locationsIndex];
    }
}
