#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

public class DrawCubeGizmos : GizmosDrawer
{
    /// <summary>
    /// Gets or sets the size of the cube.
    /// </summary>
    [Title("Cube Settings")]
    public Vector3 Size = new(1f, 1f, 1f);

    public override void Draw()
    {
        Gizmos.DrawCube(transform.position + Offset, Size);
    }
}
#endif
