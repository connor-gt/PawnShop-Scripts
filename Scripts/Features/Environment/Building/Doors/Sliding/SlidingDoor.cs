using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

[RequireComponent(typeof(Hoverable))]
public class SlidingDoor : MonoBehaviour, IInteractResponse, IHoverResponse, IHoverMessageUser
{
    public string HoverMessage { get; set; }

    /// <summary>
    /// Gets whether or not this door is currently open.
    /// </summary>
    public bool IsOpen { get; private set; }

    [Title("Positions")]
    [Tooltip("When the door has this position, it's considered closed.")]
    [SerializeField]
    private Vector3 closedPosition;

    [Tooltip("When the door has this position, it's considered open.")]
    [SerializeField]
    private Vector3 openPosition;

    [Title("Lerp Time")]
    [Tooltip("The time it takes for the door to open/close.")]
    [SerializeField]
    [Min(0f)]
    private float lerpTime = 0.4f;

    /// <summary>
    /// Gets whether or not this door is currently transitioning from open to close or close to open.
    /// </summary>
    public bool InTransition { get; private set; }

    private Hoverable hoverable;

    private void Awake()
    {
        hoverable = GetComponent<Hoverable>();
    }

    public void Interact()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void OnFocus(IHoverHandler hoverHandler)
    {
        if (IsOpen)
        {
            HoverMessage = InputManager.Instance.GetFormattedBindingDisplay(
                InputManager.Instance.Actions.Base.Interact, "Close"
            );
        }
        else
        {
            HoverMessage = InputManager.Instance.GetFormattedBindingDisplay(
                InputManager.Instance.Actions.Base.Interact, "Open"
            );
        }

        hoverHandler.HoverMessage.UpdateHoverText(HoverMessage);
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.ClearHoverText();
    }

    /// <summary>
    /// Open the door. If this door is already open, this does nothing.
    /// </summary>
    public void Open()
    {
        if (IsOpen || InTransition)
        {
            return;
        }

        IsOpen = true;
        StartCoroutine(ChangeRoutine(closedPosition, openPosition));
    }

    /// <summary>
    /// Close the door. If this door is already closed, this does nothing.
    /// </summary>
    public void Close()
    {
        if (!IsOpen || InTransition)
        {
            return;
        }

        IsOpen = false;
        StartCoroutine(ChangeRoutine(openPosition, closedPosition));
    }

    /// <summary>
    /// Lerp the door from one position to another.
    /// </summary>
    /// <param name="from">The door's current position.</param>
    /// <param name="to">The position the door should have.</param>
    private IEnumerator ChangeRoutine(Vector3 from, Vector3 to)
    {
        InTransition = true;
        yield return StartCoroutine(LerpUtils.LerpPosition(from, to, lerpTime, p => transform.position = p));
        hoverable.RequestRefresh();
        InTransition = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SlidingDoor))]
public class SlidingDoorEditor : OdinEditor
{
    SerializedProperty closedPosition;
    SerializedProperty openPosition;

    protected override void OnEnable()
    {
        base.OnEnable();
        closedPosition = serializedObject.FindProperty("closedPosition");
        openPosition = serializedObject.FindProperty("openPosition");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SlidingDoor door = (SlidingDoor)target;

        DrawDefaultInspector();
        GUILayout.Space(10);

        // Buttons that can be used to take the door's current position and set them as the open/closed positions
        if (GUILayout.Button("Set Current As Closed Position"))
        {
            closedPosition.vector3Value = door.transform.position;
            Debug.Log($"Saved Closed Position: {door.transform.position} for {door.name}");
        }

        if (GUILayout.Button("Set Current As Open Position"))
        {
            openPosition.vector3Value = door.transform.position;
            Debug.Log($"Saved Open Position: {door.transform.position} for {door.name}");
        }

        GUILayout.Space(10);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

