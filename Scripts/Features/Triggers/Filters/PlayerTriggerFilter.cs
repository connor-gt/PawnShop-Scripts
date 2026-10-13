using UnityEngine;

public class PlayerTriggerFilter : MonoBehaviour, ITriggerFilter
{
    /// <summary>
    /// Checks if the input collider's game object has the "Player" tag.
    /// </summary>
    /// <param name="other">The collider that entered the detector.</param>
    /// <returns>True if the game object has the "Player" tag, false if otherwise.</returns>
    public bool Matches(Collider other)
    {
        return other.CompareTag("Player");
    }
}
