/// <summary>
/// Provides functionality for updating and clearing the hover message displayed to the player.
/// </summary>
public interface IHoverMessageHandler
{
    /// <summary>
    /// Updates the displayed hover text.
    /// </summary>
    /// <param name="text">The text to display.</param>
    void UpdateHoverText(string text);

    /// <summary>
    /// Clears the currently displayed hover text.
    /// </summary>
    void ClearHoverText();
}
