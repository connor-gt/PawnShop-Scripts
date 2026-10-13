using UnityEngine;

/// <summary>
/// Provides responses when the player hovers over an object that implements this interface.
/// </summary>
public interface IHoverResponse
{
    /// <summary>
    /// Responds to the player beginning to hover over this object.
    /// </summary>
    /// <param name="hoverHandler">The hover handler providing access to the player's hover-related state and
    /// functionality.</param>
    void OnFocus(IHoverHandler hoverHandler);

    /// <summary>
    /// Responds to the player stopping hovering over this object.
    /// </summary>
    /// <param name="hoverHandler">The hover handler providing access to the player's hover-related state and
    /// functionality.</param>
    void OffFocus(IHoverHandler hoverHandler);
}
