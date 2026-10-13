using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewDialogueChoice", menuName = "Scriptable Objects/Dialogue/New Dialogue Choice")]
public class DialogueChoiceSO : ScriptableObject
{
    /// <summary>
    /// Gets the input action that the player must perform to select this dialogue choice.
    /// </summary>
    [field: Tooltip("The input action that the player must perform to select this dialogue choice.")]
    [field: SerializeField]
    public InputActionReference InputAction { get; private set; }

    /// <summary>
    /// Gets the text that is displayed to the player for this dialogue choice.
    /// </summary>
    [field: Tooltip("The text displayed to the player for this dialogue choice.")]
    [field: SerializeField]
    public string Label { get; private set; }
}
