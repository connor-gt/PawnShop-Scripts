using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{
    [Title("References")]
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private PlayerCrouch playerCrouch;
    
    /// <summary>
    /// Gets or sets the sprint speed of the player.
    /// </summary>
    [field: Title("Sprinting Speed")]
    [field: SerializeField]
    public float SprintSpeed { get; set; } = 5f;

    /// <summary>
    /// Gets whether or not the player is currently sprinting.
    /// </summary>
    public bool IsSprinting { get; private set; }

    /// <summary>
    /// Gets whether or not the player can currently sprint. The player can sprint if the player is standing.
    /// </summary>
    private bool CanSprint => playerCrouch != null ? playerCrouch.State == PlayerCrouch.CrouchState.Standing : true;

    private PlayerActions inputActions;

    private void Awake()
    {
        inputActions = InputManager.Instance.Actions;
    }

    private void OnEnable()
    {
        inputActions.Base.Sprint.started += OnSprintStarted;
        inputActions.Base.Sprint.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        inputActions.Base.Sprint.started -= OnSprintStarted;
        inputActions.Base.Sprint.canceled -= OnSprintCanceled;

        // Reset the player's movement speed if this script is disabled while the player is sprinting
        playerMovement.MoveSpeed = playerMovement.WalkSpeed;
        IsSprinting = false;
    }

    private void OnSprintStarted(InputAction.CallbackContext ctx)
    {
        if (!CanSprint)
        {
            return;
        }

        playerMovement.MoveSpeed = SprintSpeed;
        IsSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        if (!CanSprint)
        {
            return;
        }

        playerMovement.MoveSpeed = playerMovement.WalkSpeed;
        IsSprinting = false;
    }
}
