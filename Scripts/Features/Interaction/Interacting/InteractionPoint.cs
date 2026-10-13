using UnityEngine;

/// <summary>
/// Represents a specific spot in the environment where an NPC can stand to perform an action (e.g. when
/// browsing a shelf, moving to this location and reserving that "browsing spot" for the shelf).
/// </summary>
public class InteractionPoint : MonoBehaviour
{
    /// <summary>
    /// Gets the <see cref="Entity"/> currently occupying this interaction point.
    /// </summary>
    public Entity Occupant { get; private set; }

    /// <summary>
    /// Gets whether this interaction point is currently reserved or occupied by an <see cref="Entity"/>.
    /// </summary>
    public bool IsOccupied => Occupant != null;

    /// <summary>
    /// Try to reserve this interaction point for the specified <see cref="Entity"/>. Reservation prevents
    /// other NPCs from selecting the same point while the entity is moving toward or interacting with it.
    /// <para>
    /// NOTE: If the <see cref="Entity"/> successfully reserves this spot, once it is done <see cref="Release()"/>
    /// must be called to free this interaction point.
    /// </para>
    /// </summary>
    /// <param name="entity">The entity that wants to reserve and move to this interaction point.</param>
    /// <returns>True if the this <see cref="InteractionPoint"/> was successfully reserved, false if otherwise.
    /// </returns>
    public bool TryReserve(Entity entity)
    {
        if (IsOccupied)
        {
            return false;
        }

        Occupant = entity;
        return true;
    }

    /// <summary>
    /// Releases this <see cref="InteractionPoint"/>, clearing the current occupant and making it available
    /// to other <see cref="Entity"/>s.
    /// </summary>
    public void Release()
    {
        Occupant = null;
    }
}
