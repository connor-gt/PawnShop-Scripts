using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for physics-related methods.
/// </summary>
public static class PhysicsUtils
{
    /// <summary>
    /// Performs a forward raycast from the given source transform and attempts to retrieve a component of type
    /// <typeparamref name="T"/> from the hit object.
    /// </summary>
    /// <typeparam name="T">The component type to search for on the hit collider's GameObject.</typeparam>
    /// <param name="source">The transform from which the ray is cast. Its forward direction determines the
    /// ray direction.</param>
    /// <param name="range">The maximum distance the ray is allowed to travel.</param>
    /// <param name="layerMask">A mask restricting which layers the raycast is allowed to hit.</param>
    /// <param name="entity">When the method returns true, contains the component of type <typeparamref name="T"/>
    /// found on the hit object. Otherwise, this value is set to default.</param>
    /// <returns>True if the raycast hits an object within <paramref name="range"/> on the specified
    /// <paramref name="layerMask"/> and that object contains a component of type <typeparamref name="T"/>.
    /// Otherwise, false is returned.</returns>
    public static bool ObjectInRange<T>(Transform source, float range, LayerMask layerMask, out T entity)
    {
        Ray ray = new Ray(source.position, source.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, layerMask))
        {
            if (hit.collider.gameObject.TryGetComponent(out T e))
            {
                entity = e;
                return true;
            }
        }

        entity = default;
        return false;
    }

    /// <summary>
    /// Performs an allocation-free forward raycast from a source transform and collects all hit objects that
    /// contain a component of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The component type to search for on hit objects (e.g., IHoverResponse, Interactable,
    /// etc.).</typeparam>
    /// <param name="source">The transform from which the ray is cast. Its forward direction determines the
    /// ray direction.</param>
    /// <param name="range">The maximum distance the ray will travel.</param>
    /// <param name="layerMask">The layer mask to use for the raycast.</param>
    /// <param name="buffer">A preallocated array used to store raycast hits. This avoids allocations that would
    /// occur with <see cref="Physics.RaycastAll"/>. Only the first <c>hitCount</c> entries will contain valid
    /// data.</param>
    /// <param name="entitiesFound">A preallocated list that will be cleared and filled with all components of
    /// type <typeparamref name="T"/> found on hit objects.</param>
    /// <returns>The number of entities found.</returns>
    public static int ObjectsInRange<T>(Transform source, float range, LayerMask layerMask, RaycastHit[] buffer,
        List<T> entitiesFound)
    {
        Array.Clear(buffer, 0, buffer.Length);
        entitiesFound.Clear();

        Ray ray = new Ray(source.position, source.forward);
        int hitCount = Physics.RaycastNonAlloc(ray, buffer, range, layerMask);
        for (int i = 0; i < hitCount; i++)
        {
            if (buffer[i].collider.gameObject.TryGetComponent(out T e))
            {
                entitiesFound.Add(e);
            }
        }

        return entitiesFound.Count;
    }
}
