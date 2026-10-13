using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

[RequireComponent(typeof(Hoverable), typeof(Interactable))]
public class Door : MonoBehaviour, IInteractResponse, IHoverResponse, IHoverMessageUser
{
    public string HoverMessage { get; set; }

    /// <summary>
    /// Gets whether or not this door is currently open.
    /// </summary>
    public bool IsOpen { get; private set; }

    [Title("Lerp Rotations")]
    [Tooltip("When the door has this rotation, it's considered closed.")]
    [SerializeField]
    private Vector3 closedRotation;
    private Quaternion closedQuaternion;

    [Tooltip("When the door has this rotation, it's considered open.")]
    [SerializeField]
    private Vector3 openRotation;
    private Quaternion openQuaternion;

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
        
        closedQuaternion = Quaternion.Euler(closedRotation);
        openQuaternion = Quaternion.Euler(openRotation);
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
        StartCoroutine(ChangeRoutine(closedQuaternion, openQuaternion));
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
        StartCoroutine(ChangeRoutine(openQuaternion, closedQuaternion));
    }

    /// <summary>
    /// Lerp the door from one rotation to another.
    /// </summary>
    /// <param name="from">The door's current rotation.</param>
    /// <param name="to">The rotation the door should have.</param>
    private IEnumerator ChangeRoutine(Quaternion from, Quaternion to)
    {
        InTransition = true;
        yield return StartCoroutine(LerpUtils.LerpRotation(from, to, lerpTime, r => transform.rotation = r));
        hoverable.RequestRefresh();
        InTransition = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Door))]
public class DoorEditor : OdinEditor
{
    SerializedProperty closedRotation;
    SerializedProperty openRotation;

    protected override void OnEnable()
    {
        base.OnEnable();
        closedRotation = serializedObject.FindProperty("closedRotation");
        openRotation = serializedObject.FindProperty("openRotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Door door = (Door)target;

        DrawDefaultInspector();
        GUILayout.Space(10);

        // Buttons that can be used to take the door's current rotation and set them as the open/closed rotations
        if (GUILayout.Button("Set Current As Closed Rotation"))
        {
            closedRotation.vector3Value = door.transform.rotation.eulerAngles;
            Debug.Log($"Saved Closed Rotation: {door.transform.localEulerAngles} for {door.name}");
        }

        if (GUILayout.Button("Set Current As Open Rotation"))
        {
            openRotation.vector3Value = door.transform.rotation.eulerAngles;
            Debug.Log($"Saved Open Rotation: {door.transform.localEulerAngles} for {door.name}");
        }

        GUILayout.Space(10);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
