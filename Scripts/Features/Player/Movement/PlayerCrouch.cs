using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : MonoBehaviour
{
    public enum CrouchState
    {
        Standing,
        Crouching,
        Transitioning
    }

    public CrouchState State { get; private set; } = CrouchState.Standing;

    [Title("References")]
    // CharacterController is needed to adjust the height of the controller's collider
    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private PlayerSprint playerSprint;

    /// <summary>
    /// Gets or sets the player's movement speed while crouching.
    /// </summary>
    [field: Title("Crouching Speed")]
    [field: SerializeField]
    public float CrouchSpeed { get; set; } = 2f;

    [Title("Parameters")]
    [Tooltip("The height the player camera/collider crouches to when crouching.")]
    [SerializeField]
    [Min(0f)]
    private float crouchHeight = 1.25f;

    // Determined by CharacterController.height
    private float standingHeight;

    /// <summary>
    /// Gets or set the time it takes to crouch and uncrouch.
    /// </summary>
    [field: SerializeField]
    [Min(0f)]
    public float TimeToCrouch { get; set; } = 0.2f;
    private float animationTimeElapsed = 0f; // For handling the crouch's animation

    [Tooltip("Recommended to keep at an extremely small value (<= 0.0002). This float prevents jitterness caused by "
        + "the collider digging into the floor when moving up from the crouched position by making small movements")]
    [SerializeField]
    [Range(0f, 0.001f)]
    private float upwardCrouchSmoother = 0.0004f;

    /// <summary>
    /// Gets whether or not the player can currently crouch. The player can crouch if the player is not sprinting.
    /// </summary>
    private bool CanCrouch => playerSprint != null ? !playerSprint.IsSprinting : true;

    private static readonly Vector3 CROUCH_CENTER = new(0f, 0.5f, 0f);
    private Vector3 standingCenter = Vector3.zero;

    private float crouchStartHeight;
    private float crouchTargetHeight;
    private Vector3 crouchStartCenter;
    private Vector3 crouchTargetCenter;

    // For detecting colliders above player when crouching
    private float sphereCastDistance;

    private PlayerActions inputActions;

    // Flag to keep track of whether or not the player is currently pressing the crouch keybind
    private bool crouchInputActive;

    private void Awake()
    {
        inputActions = InputManager.Instance.Actions;

        standingHeight = characterController.height;
        sphereCastDistance = standingHeight - crouchHeight;
    }

    private void OnEnable()
    {
        inputActions.Base.Crouch.started += OnCrouchStarted;
        inputActions.Base.Crouch.canceled += OnCrouchCanceled;
    }

    private void OnDisable()
    {
        inputActions.Base.Crouch.started -= OnCrouchStarted;
        inputActions.Base.Crouch.canceled -= OnCrouchCanceled;
    }

    private void Update()
    {
        if (crouchInputActive && CanCrouch)
        {
            StartCrouch();
        }
        else if (!crouchInputActive)
        {
            StartStand();
        }

        AnimateCrouch();
    }

    private void OnCrouchStarted(InputAction.CallbackContext ctx)
    {
        crouchInputActive = true;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        crouchInputActive = false;
    }

    /// <summary>
    /// Start the process of crouching. This process is only started if State == CrouchState.Standing.
    /// </summary>
    private void StartCrouch()
    {
        if (State == CrouchState.Crouching || State == CrouchState.Transitioning)
        {
            return;
        }

        State = CrouchState.Transitioning;
        animationTimeElapsed = 0f;

        crouchStartHeight = characterController.height;
        crouchTargetHeight = crouchHeight;

        crouchStartCenter = characterController.center;
        crouchTargetCenter = CROUCH_CENTER;

        playerMovement.MoveSpeed = CrouchSpeed;
    }

    /// <summary>
    /// Start the process of standing. This process is only started if State == CrouchState.Crouching.
    /// </summary>
    private void StartStand()
    {
        if (State == CrouchState.Standing || State == CrouchState.Transitioning)
        {
            return;
        }

        // The player cannot/should not stand up if there is a collider above the player (prevents getting stuck)
        if (CheckCeiling())
        {
            return;
        }

        State = CrouchState.Transitioning;
        animationTimeElapsed = 0f;

        crouchStartHeight = characterController.height;
        crouchTargetHeight = standingHeight;

        crouchStartCenter = characterController.center;
        crouchTargetCenter = standingCenter;

        playerMovement.MoveSpeed = playerMovement.WalkSpeed;
    }

    /// <summary>
    /// Animate the transition state between Crouching and Standing by lerping between their target heights.
    /// </summary>
    private void AnimateCrouch()
    {
        if (State != CrouchState.Transitioning)
        {
            return;
        }

        animationTimeElapsed += Time.deltaTime;
        float lerp = animationTimeElapsed / TimeToCrouch;

        characterController.height = Mathf.Lerp(crouchStartHeight, crouchTargetHeight, lerp);
        characterController.center = Vector3.Lerp(crouchStartCenter, crouchTargetCenter, lerp);

        // Makes sure that there are no jittery movements when moving back up from a crouch
        characterController.Move(Random.onUnitSphere * upwardCrouchSmoother);

        if (lerp >= 1f)
        {
            // Snap to final values once the lerping is finished
            characterController.height = crouchTargetHeight;
            characterController.center = crouchTargetCenter;
            State = Mathf.Approximately(crouchTargetHeight, crouchHeight) ? CrouchState.Crouching : CrouchState.Standing;
        }
    }

    /// <summary>
    /// Check if a collider is above the player via Physics.SphereCast().
    /// </summary>
    /// <returns>True if there is a collider above the player, false if otherwise.</returns>
    private bool CheckCeiling()
    {
        Vector3 origin = transform.position + Vector3.up * characterController.radius;

        return Physics.SphereCast(
            origin,
            characterController.radius,
            Vector3.up,
            out _,
            sphereCastDistance
        );
    }
}
