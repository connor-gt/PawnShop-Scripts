#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Base class for drawing gizmos.
/// </summary>
public abstract class GizmosDrawer : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the color of the gizmos.
    /// </summary>
    [Title("Generic Settings")]
    public Color Color = Color.white;

    /// <summary>
    /// Gets or sets the offset of the gizmos.
    /// </summary>
    public Vector3 Offset = Vector3.zero;

    /// <summary>
    /// Draw the gizmos at this MonoBehaviour's transform.position.
    /// </summary>
    public abstract void Draw();

    private void Reset()
    {
        gameObject.tag = "EditorOnly";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color;
        Draw();
    }
}
#endif
