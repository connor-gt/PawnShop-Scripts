using UnityEngine;

/// <summary>
/// Acts as a single filter for a <see cref="TriggerDetector"/>.
/// </summary>
public interface ITriggerFilter
{
    /// <summary>
    /// Check whether or not the collider that entered the <see cref="TriggerDetector"/> matches the detector's
    /// requirements.
    /// <para>
    /// For example, <see cref="PlayerTriggerFilter"/> makes the sure the collider's game object has the "Player"
    /// tag.
    /// </para>
    /// </summary>
    /// <param name="other">The collider that entered the detector.</param>
    /// <returns>True if the collider matches the requirement, false if otherwise.</returns>
    bool Matches(Collider other);
}
