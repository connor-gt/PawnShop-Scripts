/// <summary>
/// Data container for a shelf spot and its corresponding <see cref="Holdable"/> and <see cref="InteractionPoint"/>.
/// </summary>
public class ShelfItem
{
    /// <summary>
    /// Gets the holdable component associated with this shelf item.
    /// </summary>
    public Holdable Holdable { get; }

    /// <summary>
    /// Gets the interaction point associated with this shelf item.
    /// </summary>
    public InteractionPoint InteractionPoint { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShelfItem"/> class.
    /// </summary>
    /// <param name="holdable">The holdable component associated with the shelf item.</param>
    /// <param name="interactionPoint">The interaction point associated with the shelf item.</param>
    public ShelfItem(Holdable holdable, InteractionPoint interactionPoint)
    {
        Holdable = holdable;
        InteractionPoint = interactionPoint;
    }
}
