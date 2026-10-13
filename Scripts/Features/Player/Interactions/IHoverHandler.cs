using System;

/// <summary>
/// Provides the player state and functionality that hover responses may need when responding to hover events.
/// </summary>
public interface IHoverHandler
{
    /// <summary>
    /// Occurs when the player begins hovering over a <see cref="Hoverable"/> object.
    /// </summary>
    event Action<Hoverable> OnFocus;

    /// <summary>
    /// Occurs when the player stops hovering over a <see cref="Hoverable"/> object.
    /// </summary>
    event Action<Hoverable> OffFocus;

    /// <summary>
    /// Gets the <see cref="Hoverable"/> game object the player is currently hovering over.
    /// </summary>
    Hoverable CurrentHoverable { get; }

    /// <summary>
    /// Gets the player's current item-handling state and inventory.
    /// </summary>
    IPlayerInventoryState PlayerInventoryState { get; }

    /// <summary>
    /// Gets the functionality used to update or clear the displayed hover message.
    /// </summary>
    IHoverMessageHandler HoverMessage { get; }
}