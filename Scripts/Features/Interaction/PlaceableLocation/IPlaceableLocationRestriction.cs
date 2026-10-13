/// <summary>
/// Defines a restriction that determines whether an <see cref="Item"/> can be placed at a <see cref="PlaceableLocation"/>.
/// </summary>
public interface IPlaceableLocationRestriction
{
    /// <summary>
    /// Determines whether the given <see cref="Item"/> meets this restriction.
    /// </summary>
    /// <param name="item">The item being checked against this restriction.</param>
    /// <returns>True if the item meets the restriction, false if otherwise.</returns>
    bool MeetsRestriction(Item item);
}
