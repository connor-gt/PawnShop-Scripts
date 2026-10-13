using UnityEngine;

/// <summary>
/// Defines the data and factory method used to create a runtime <see cref="Brain"/> instance for an
/// <see cref="Entity"/>.
/// </summary>
public abstract class BrainSO : ScriptableObject
{
    /// <summary>
    /// Creates a runtime <see cref="Brain"/> instance for the given entity.
    /// </summary>
    /// <param name="entity">The entity that will own the <see cref="Brain"/>.</param>
    /// <returns>The created <see cref="Brain"/> instance.</returns>
    public abstract Brain CreateBrain(Entity entity);
}
