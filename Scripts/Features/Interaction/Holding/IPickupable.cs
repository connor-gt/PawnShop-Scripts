/// <summary>
/// Represents an object that can be picked up by the player.
/// </summary>
public interface IPickupable
{
    /// <summary>
    /// Handles a request to pick up this object.
    /// </summary>
    /// <param name="pickupHandler">The handler responsible for the player's held item.</param>
    void Pickup(IPickupHandler pickupHandler);
}
