using Unity.Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Title("References")]
    [SerializeField]
    private CharacterController characterController;

    [Tooltip("Cinemachine reference is needed to determine the player's facing direction.")]
    [SerializeField]
    private CinemachineCamera cinemachine;

    /// <summary>
    /// Gets the base walk speed of the player.
    /// </summary>
    [field: Title("Base Speed")]
    [field: SerializeField]
    public float WalkSpeed { get; private set; } = 3f;

    /// <summary>
    /// Gets the current move direction of the player.
    /// </summary>
    public Vector3 Direction { get; private set; } = Vector3.zero;

    /// <summary>
    /// Gets or sets the current move speed of the player.
    /// </summary>
    public float MoveSpeed { get; set; } // Can be set by PlayerSprint

    private PlayerActions inputActions;

    private void Awake()
    {
        inputActions = InputManager.Instance.Actions;
        MoveSpeed = WalkSpeed;
    }

    private void Update()
    {
        Vector2 moveInput = GetMovementInput();

        Direction = (moveInput.y * cinemachine.transform.forward) + (moveInput.x * cinemachine.transform.right);
        float magnitude = Mathf.Clamp01(Direction.magnitude) * MoveSpeed;
        characterController.SimpleMove(Direction.normalized * magnitude);
    }

    /// <summary>
    /// Gets the current Vector2 value of the <see cref="PlayerActions.m_Base_Move"/> input from
    /// <see cref="PlayerActions"/>.
    /// </summary>
    /// <returns>A Vector2 representing the player's movement input.</returns>
    public Vector2 GetMovementInput()
    {
        return inputActions.Base.Move.ReadValue<Vector2>();
    }
}
