/// <summary>
/// Defines an object that can receive the player's hover context for accessing the current hover state and
/// responding to changes in the player's hover target.
/// </summary>
public interface IHoverContextReceiver
{
    /// <summary>
    /// Provides this object with the player's current hover context.
    /// </summary>
    /// <param name="hoverHandler">The player's hover handler.</param>
    void Inject(IHoverHandler hoverHandler);
}
