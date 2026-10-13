#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

public class DrawSphereGizmos : GizmosDrawer
{
    /// <summary>
    /// Gets or sets the radius of the sphere.
    /// </summary>
    [field: Title("Sphere Settings")]
    [field: SerializeField]
    public float Radius { get; set; } = 1f;

    public override void Draw()
    {
        Gizmos.DrawSphere(transform.position + Offset, Radius);
    }
}
#endif
