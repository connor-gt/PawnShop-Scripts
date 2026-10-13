/// <summary>
/// Represents an object that can receive a placement request from a pickup handler.
/// </summary>
public interface IPlaceable
{
    /// <summary>
    /// Places the currently held item at this location.
    /// </summary>
    /// <param name="pickupHandler">The handler responsible for the player's held item.</param>
    void Place(IPickupHandler pickupHandler);
}
