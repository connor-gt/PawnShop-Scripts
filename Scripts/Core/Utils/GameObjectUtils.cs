using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Utility class for common GameObject related operations.
/// </summary>
public static class GameObjectUtils
{
    /// <summary>
    /// Checks if the <see cref="MonoBehaviour"/> is not null, enabled, and its game object is active.
    /// </summary>
    /// <typeparam name="T">The type of the MonoBehaviour.</typeparam>
    /// <param name="monoBehaviour">The MonoBehaviour to check.</param>
    /// <returns>True if the MonoBehaviour is fully active. False if otherwise.</returns>
    public static bool FullyActive<T>(T monoBehaviour) where T : MonoBehaviour
    {
        return monoBehaviour != null && monoBehaviour.enabled && monoBehaviour.gameObject.activeSelf;
    }

    /// <summary>
    /// Returns all components of type <typeparamref name="T"/> found on the given GameObject and its
    /// immediate children only. This method does not recurse deeper into the hierarchy and includes
    /// components on inactive child objects.
    /// </summary>
    /// <typeparam name="T">The component type to search for.</typeparam>
    /// <param name="obj">The GameObject whose components and immediate children will be scanned.</param>
    /// <returns>An array containing all <typeparamref name="T"/> components.</returns>
    public static T[] GetComponentsInImmediateChildren<T>(this GameObject obj) where T : Component
    {
        T[] parentComponents = obj.GetComponents<T>();

        IEnumerable<T> childComponents = obj.transform.Cast<Transform>()
            .SelectMany(child => child.GetComponents<T>());

        // Combine the components of the parent obj with the children obj to one array
        T[] totalComponents = parentComponents.Concat(childComponents).ToArray();
        return totalComponents;
    }

    /// <summary>
    /// Gets a component from a game object.
    /// <para>
    /// If the game object does not have the component, the component is added.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of  to add or find.</typeparam>
    /// <param name="obj">The game object to get the component from or add to.</param>
    /// <returns>The existing component or the newly added component if it was missing.</returns>
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
        }

        return component;
    }
    
    /// <summary>
    /// Add components to a game object. The components are only added if they don't already exist on the game
    /// object.
    /// </summary>
    /// <param name="obj">The game object to add the components to.</param>
    /// <param name="componentTypes">A list of component types that should be added to the object.</param>
    /// <returns>The number of components that were added.</returns>
    public static int AddComponentsIfMissing(this GameObject obj, params Type[] componentTypes)
    {
        int added = 0;
        
        for (int i = 0; i < componentTypes.Length; i++)
        {
            Type currentType = componentTypes[i];
            Component component = obj.GetComponent(currentType);
            if (component == null)
            {
                obj.AddComponent(currentType);
                added++;
            }
        }

        return added;
    }

    /// <summary>
    /// Calculates a hash value representing the structure of the hierarchy rooted at the given transform.
    /// </summary>
    /// <param name="root">The root transform of the hierarchy.</param>
    /// <returns>The calculated hash value.</returns>
    public static int CalculateHierarchyHash(Transform root)
    {
        int hash = root.GetEntityId().GetHashCode();

        foreach (Transform child in root)
        {
            hash ^= CalculateHierarchyHash(child);
        }

        return hash;
    }

    /// <summary>
    /// Lerp rotate towards a target. Certain axes can be disallowed. By default, only the Y axis is rotated
    /// to make sure the <see cref="pivot"/> transform rotates horizontally.
    /// <para>
    /// This method should be called in some sort of Update method that calls continuously.
    /// </para>
    /// </summary>
    /// <param name="pivot">The transform thats being rotated.</param>
    /// <param name="target">The target transform the <see cref="pivot"/> should look at.</param>
    /// <param name="rotationSpeed">How fast the <see cref="pivot"/> rotates.</param>
    /// <param name="allowedAxes">The axes that the rotation should be performed on.</param>
    public static void RotateTowardsTarget(this Transform pivot, Transform target, float rotationSpeed, 
        Axis allowedAxes = Axis.Y)
    {
        float tolerance = 0.001f;
        Vector3 direction = target.position - pivot.position;

        // Don't bother rotating if the target is on top of the pivot
        if (direction.sqrMagnitude < tolerance)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 currentEuler = pivot.rotation.eulerAngles;
        Vector3 targetEuler = targetRotation.eulerAngles;

        // Only include the allowed axes in the rotation lerp
        Vector3 maskedEuler = new Vector3(
            allowedAxes.HasFlag(Axis.X) ? targetEuler.x : currentEuler.x,
            allowedAxes.HasFlag(Axis.Y) ? targetEuler.y : currentEuler.y,
            allowedAxes.HasFlag(Axis.Z) ? targetEuler.z : currentEuler.z
        );

        pivot.rotation = Quaternion.Slerp(
            pivot.rotation,
            Quaternion.Euler(maskedEuler),
            Time.deltaTime * rotationSpeed
        );
    }

    /// <summary>
    /// Check if a transform is facing another transform.
    /// <para>
    /// Common threshold values:
    /// <list type="bullet">
    /// <item><description><c>-1</c>: The target may be directly behind the pivot.</description></item>
    /// <item><description><c>0</c>: The target only needs to be somewhere in front of the pivot.</description></item>
    /// <item><description><c>0.95</c>: The pivot must be facing almost directly at the target.</description></item>
    /// <item><description><c>1</c>: The pivot must be perfectly aligned with the target.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="pivot">The transform whose forward direction is used for the comparison.</param>
    /// <param name="target">The transform to check against.</param>
    /// <param name="threshold">The minimum dot product required for this method to return true.</param>
    /// <param name="flattenedAxes">Axes to ignore when calculating the facing direction. For example,
    /// <see cref="Axis.Y"/> ignores vertical differences, allowing the comparison to be performed only
    /// on the horizontal plane.</param>
    /// <returns>True if the pivot is facing the target within the specified threshold, false if otherwise.</returns>
    public static bool IsFacingTarget(this Transform pivot, Transform target, float threshold = 0.95f,
        Axis flattenedAxes = Axis.Y)
    {
        threshold = Mathf.Clamp(threshold, -1f, 1f);

        Vector3 direction = target.position - pivot.position;
        Vector3 maskedDirection = new Vector3(
            flattenedAxes.HasFlag(Axis.X) ? 0 : direction.x,
            flattenedAxes.HasFlag(Axis.Y) ? 0 : direction.y,
            flattenedAxes.HasFlag(Axis.Z) ? 0 : direction.z
        );

        maskedDirection.Normalize();

        Vector3 pivotForward = pivot.forward;
        Vector3 maskedPivotForward = new Vector3(
            flattenedAxes.HasFlag(Axis.X) ? 0 : pivotForward.x,
            flattenedAxes.HasFlag(Axis.Y) ? 0 : pivotForward.y,
            flattenedAxes.HasFlag(Axis.Z) ? 0 : pivotForward.z
        );

        maskedPivotForward.Normalize();

        float dotProduct = Vector3.Dot(maskedPivotForward, maskedDirection);
        return dotProduct >= threshold;
    }

    /// <summary>
    /// Checks whether the transform is within the specified distance of another transform using a squared
    /// distance comparison rather than a square root comparison like Vector3.Distance().
    /// </summary>
    /// <param name="transform">The transform from which the distance is measured.</param>
    /// <param name="other">The other transform to compare against.</param>
    /// <param name="distance">The maximum allowed distance between the two transforms.</param>
    /// <returns>True if the transforms are within the specified distance, false if otherwise.</returns>
    public static bool WithinDistance(this Transform transform, Transform other, float distance)
    {
        float distanceSqrd = distance * distance;
        return (other.position - transform.position).sqrMagnitude <= distanceSqrd;
    }

    /// <summary>
    /// Retrieves the minimum and maximum world-space bounds of a GameObject (that has a Renderer component)
    /// along a specific axis.
    /// </summary>
    /// <param name="obj">The target GameObject to check for a Renderer component.</param>
    /// <param name="axis">The coordinate axis to get the bounds for.</param>
    /// <returns>A Vector2 that has the min and max world-space bounds (Min, Max). If no Renderer component is
    /// found on the game object, Vector2.zero is returned.</returns>
    public static Vector2 GetMinMaxBound(GameObject obj, Axis axis)
    {
        if (obj.TryGetComponent(out Renderer renderer))
        {
            return GetMinMaxBound(renderer, axis);
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Extracts a min/max boundary range from a 3D world-space Renderer bounding box.
    /// </summary>
    /// <param name="renderer">The Renderer component supplying the world bounds.</param>
    /// <param name="axis">The coordinate axis to get the bounds for.</param>
    /// <returns>A Vector2 that has the min and max world-space bounds (Min, Max). If no Renderer component is
    /// found on the game object, Vector2.zero is returned.</returns>
    public static Vector2 GetMinMaxBound(Renderer renderer, Axis axis)
    {
        Bounds bounds = renderer.bounds;

        switch (axis)
        {
            case Axis.X:
                return new Vector2(bounds.min.x, bounds.max.x);
            case Axis.Y:
                return new Vector2(bounds.min.y, bounds.max.y);
            case Axis.Z:
                return new Vector2(bounds.min.z, bounds.max.z);
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Set a game object's and all of its childrens' layers to the specified layer.
    /// </summary>
    /// <param name="obj">The game object and its children to update.</param>
    /// <param name="layer">The layer to set the game object and its children to.</param>
    public static void SetLayerRecursive(this GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}
