/// <summary>
/// Provides access to the player's current item-handling state and inventory.
/// </summary>
public interface IPlayerInventoryState
{
    /// <summary>
    /// Gets the player's current hold state.
    /// </summary>
    HoldState HoldState { get; }

    /// <summary>
    /// Gets the player's inventory.
    /// </summary>
    Inventory Inventory { get; }
}
