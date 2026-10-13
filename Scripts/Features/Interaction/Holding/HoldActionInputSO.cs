using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Defines the relative display order of a hold action in the hold-action UI.
/// </summary>
public enum HoldActionDisplayOrder
{
    /// <summary>
    /// Displays the action as the primary action, before secondary and information actions.
    /// </summary>
    Primary = 10,

    /// <summary>
    /// Displays the action after the primary action and before information actions.
    /// </summary>
    Secondary = 20,

    /// <summary>
    /// Displays the action as an informational action, after primary and secondary actions.
    /// </summary>
    Information = 30
}

/// <summary>
/// Defines the input and display information for a hold action.
/// </summary>
[CreateAssetMenu(fileName = "NewHoldActionInput", menuName = "Scriptable Objects/New Hold Action Input")]
public class HoldActionInputSO : ScriptableObject
{
    /// <summary>
    /// Gets the input action used to perform this hold action.
    /// </summary>
    [field: Title("Input Action")]
    [field: Tooltip("The input action used to perform this hold action.")]
    [field: SerializeField]
    public InputActionReference InputReference { get; private set; }

    /// <summary>
    /// Gets the label for this action.
    /// <para>
    /// For example, for a flashlight this could be "Toggle Light".
    /// </para>
    /// </summary>
    [field: Title("Action Label")]
    [field: Tooltip("The text displayed for this action in the hold-action UI.")]
    [field: SerializeField]
    public string Label { get; private set; }

    /// <summary>
    /// Gets the relative position of this action in the hold-action UI.
    /// </summary>
    [field: Title("Display Order")]
    [field: Tooltip("Determines where this action appears relative to other hold actions in the UI.")]
    [field: SerializeField]
    public HoldActionDisplayOrder DisplayOrder { get; private set; } = HoldActionDisplayOrder.Primary;
}
