using System;
using UnityEngine;

/// <summary>
/// Responsible for generating and instantiating <see cref="Entity"/> objects during runtime.
/// </summary>
public static class EntityFactory
{
    /// <summary>
    /// Spawn an <see cref="Entity"/> game object from the given prefab at a specified location.
    /// </summary>
    /// <param name="prefab">The prefab to instantiate.</param>
    /// <param name="location">The location to spawn the <see cref="Entity"/>.</param>
    /// <returns>The instantiated <see cref="Entity"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the prefab does not contain an <see cref="Entity"/>
    /// component.</exception>
    public static Entity SpawnEntity(GameObject prefab, Vector3 location)
    {
        GameObject instantiated = GameObject.Instantiate(prefab, location, Quaternion.identity);
        Entity entity = instantiated.GetComponent<Entity>();

        if (entity == null)
        {
            GameObject.Destroy(instantiated);
            throw new ArgumentException($"The argument {prefab.name} does not have an Entity component!");
        }

        return entity;
    }

    /// <inheritdoc cref="SpawnEntity(GameObject, Vector3)"/>
    public static Entity SpawnEntity(GameObject prefab, Transform location)
    {
        return SpawnEntity(prefab, location.position);
    }
}
