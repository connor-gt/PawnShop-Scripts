using UnityEngine;

/// <summary>
/// Base class for <see cref="Entity"/> spawn behavior.
/// </summary>
// NOTE: interfaces can't be used (I tried) since Unity cannot serialize interfaces to the inspector
public abstract class EntitySpawnBehaviorSO : ScriptableObject
{
    /// <summary>
    /// Initialize the spawn behavior (acts as Start()).
    /// </summary>
    /// <param name="entityController">The controller that contains methods for spawning entities.</param>
    public abstract void Initialize(EntityController entityController);

    /// <summary>
    /// Updates frame-based logic for the spawn behavior (acts as Update()).
    /// </summary>
    /// <param name="entityController">The controller that contains methods for spawning entities.</param>
    public abstract void Evaluate(EntityController entityController);
}
