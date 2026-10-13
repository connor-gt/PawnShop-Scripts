using UnityEngine;

/// <summary>
/// Provides responses when the player interacts with an object.
/// </summary>
public interface IInteractResponse
{
    GameObject gameObject { get; }

    /// <summary>
    /// Do something when the player interacts with this object.
    /// </summary>
    void Interact();
}
