using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// A hover response that enables or disables a designated outline object when this object gains or loses
/// hover focus.
/// </summary>
public class HoverOutlineObject : MonoBehaviour, IHoverResponse
{
    [field: Title("Object Reference")]
    [field: Tooltip("The game object that will be turned on/off via OnFocus() and OffFocus(). If this is "
        + "null, the game object this is a component of will be used.")]
    [field: SerializeField]
    public GameObject Object { get; set; } = null;

#if UNITY_EDITOR
    [Title("Editor Only")]
    [Tooltip("If enabled, this game object's mesh will be auto-set to the parent's mesh.")]
    [SerializeField]
    private bool autoSet = true;

    // Makes this so I don't have to set the outline mesh every time I create new holdable items
    private void OnValidate()
    {
        if (!autoSet)
        {
            return;
        }

        if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out MeshFilter parentMeshFilter))
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = parentMeshFilter.sharedMesh;
        }
    }
#endif

    public void OnFocus(IHoverHandler hoverHandler)
    {
        if (Object == null)
        {
            Object = gameObject;
        }

        Object.SetActive(true);
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        Object.SetActive(false);
    }
}
