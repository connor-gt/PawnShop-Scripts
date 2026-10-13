#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

public class DrawMeshGizmos : GizmosDrawer
{
    [Title("Mesh Settings")]
    public Mesh Mesh;

    /// <summary>
    /// Gets or sets the scale of the mesh gizmo.
    /// </summary>
    [field: SerializeField]
    public Vector3 Scale { get; set; } = Vector3.one;

    public override void Draw()
    {
        Gizmos.DrawMesh(Mesh, transform.position + Offset, transform.rotation, Scale);
    }
}
#endif
