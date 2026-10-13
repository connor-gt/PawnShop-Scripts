#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Provides utility methods for editor tools that need to mark a GameObject hierarchy as static.
/// </summary>
public class StaticUtility
{
    /// <summary>
    /// Marks the specified <paramref name="obj"/> and all of its children as static via recursion.
    /// </summary>
    /// <param name="obj">The GameObject whose entire hierarchy should be marked static.</param>
    /// <param name="isStatic">The static value to set.</param>
    public void SetStatic(GameObject obj, bool isStatic)
    {
        // obj.isStatic is never present in builds
        obj.isStatic = isStatic;

        foreach (Transform child in obj.transform)
        {
            SetStatic(child.gameObject, isStatic);
        }
    }
}
#endif
