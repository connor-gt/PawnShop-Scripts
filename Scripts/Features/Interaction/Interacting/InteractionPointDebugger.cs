#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Draws a ray between an <see cref="InteractionPoint"/> and its occupant (if any).
/// </summary>
public class InteractionPointerDebugger : MonoBehaviour
{
    [Title("Interaction Point Reference")]
    public InteractionPoint Point;

    [Title("Debug")]
    [SerializeField]
    private bool drawRay = true;

    private void Awake()
    {
        if (Point == null)
        {
            Point = GetComponent<InteractionPoint>();
        }
    }

    private void Update()
    {
        if (drawRay && Point.IsOccupied)
        {
            Debug.DrawLine(Point.transform.position, Point.Occupant.transform.position, Color.green);
        }
    }
}
#endif
