/// <summary>
/// Provides operations for requesting pickup and placement of items on behalf of the player.
/// </summary>
public interface IPickupHandler
{
    /// <summary>
    /// Requests that the specified holdable be picked up by the player.
    /// </summary>
    /// <param name="holdable">The holdable object to pick up.</param>
    void RequestPickup(Holdable holdable);

    /// <summary>
    /// Requests that the player's currently held item be placed at the specified location.
    /// </summary>
    /// <param name="placeableLocation">The location at which to place the held item.</param>
    void RequestPlace(PlaceableLocation placeableLocation);
}
