using System;
using System.Collections.Generic;

/// <summary>
/// Defines a relationship between an <see cref="InteractionPoint"/> and a set of related interaction targets.
/// <para>
/// An example is for shelves: a single interaction point corresponds to multiple item slots or
/// <see cref="PlaceableLocation"/> instances, making sure <see cref="Entity"/>s don't overlap/crowd each other
/// trying to get to the same item on a shelf, or different items in the same location but on different levels.
/// </para>
/// </summary>
/// <typeparam name="T">The type of interaction-related objects associated with the point.</typeparam>
[Serializable]
public class InteractionMapping<T>
{
    /// <summary>
    /// The interaction point that <see cref="Entity"/>s should move to when interacting with the associated
    /// objects.
    /// </summary>
    public InteractionPoint Point = null;

    /// <summary>
    /// A list of interaction-related objects associated with the interaction point.
    /// </summary>
    public List<T> Interactions = null;
}
