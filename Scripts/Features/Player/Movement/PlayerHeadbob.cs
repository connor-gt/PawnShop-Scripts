using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerHeadbob : MonoBehaviour
{
    [Title("References")]
    [Tooltip("Not the cinemachine camera's transform but the game object the cinemachine tracks.")]
    [SerializeField]
    private Transform camHeadTransform;

    [Tooltip("Required for determining if the player is moving and in which direction for the headbob effect.")]
    [SerializeField]
    private PlayerMovement playerMovement;
    
    [Tooltip("Required for determining the headbob speed and amount based on if the player is sprinting.")]
    [SerializeField]
    private PlayerSprint playerSprint;

    [Title("Headbob Settings")]
    [Tooltip("The minimum speed needed for the headbob to take place (needed for controller input).")]
    [SerializeField]
    [Min(0f)]
    private float minMovementValue = 0.1f;

    /// <summary>
    /// Gets or sets the speed (frequency) of the bobbing effect while walking.
    /// </summary>
    [field: SerializeField]
    public float WalkBobSpeed { get; set; } = 14f;

    /// <summary>
    /// Gets or sets the amount (amplitude) of the bobbing effect while walking.
    /// </summary>
    [field: SerializeField]
    public float WalkBobAmount { get; set; } = 0.05f;

    /// <summary>
    /// Gets or sets the speed (frequency) of the bobbing effect while sprinting.
    /// </summary>
    [field: SerializeField]
    public float SprintBobSpeed { get; set; } = 18f;

    /// <summary>
    /// Gets or sets the amount (amplitude) of the bobbing effect while sprinting.
    /// </summary>
    [field: SerializeField]
    public float SprintBobAmount { get; set; } = 0.1f;
    
    /// <summary>
    /// The position at which the camera rests/resets to while the player is not moving.
    /// </summary>
    private float defaultYPos;

    /// <summary>
    /// Tolerance is used for getting the camera back up to the defaultYPos (technically very, very close but
    /// not exactly precise) when the player is no longer moving.
    /// </summary>
    private const float DEFAULT_Y_POS_TOLERANCE = 0.003f;

    private float timer = 0f;

    /// <summary>
    /// Gets whether or not the camera is currently in a bobbing effect (is also a good indicator if the player
    /// is moving).
    /// </summary>
    public bool IsBobbing { get; private set; }

    private void Awake()
    {
        defaultYPos = camHeadTransform.localPosition.y;
    }

    private void OnDisable()
    {
        // Makes sure that the player camera transform.position.y is reset if headbob is disabled
        if (camHeadTransform != null)
        {
            camHeadTransform.position = new Vector3(
                camHeadTransform.position.x,
                defaultYPos,
                camHeadTransform.position.z
            );
        }
    }

    private void Update()
    {
        HandleHeadbob();
    }

    private void HandleHeadbob()
    {
        Vector3 pos = camHeadTransform.localPosition;

        // Handle the headbob while moving
        if (Mathf.Abs(playerMovement.Direction.x) > minMovementValue
            || Mathf.Abs(playerMovement.Direction.z) > minMovementValue)
        {
            timer += Time.deltaTime * (playerSprint.IsSprinting ? SprintBobSpeed : WalkBobSpeed);

            // Wrap the timer to keep it within 0 and 2 * pi -- realistically this doesn't need to be done but
            // it can keep the timer from scaling off into infinity (which in normal gameplay shouldn't matter)
            timer %= Mathf.PI * 2f;

            pos.y = defaultYPos + Mathf.Sin(timer) * (playerSprint.IsSprinting ? SprintBobAmount : WalkBobAmount);
            camHeadTransform.localPosition = pos;

            IsBobbing = true;
        }
        // If the player is not moving and the current head position is not the resting position, move it back
        // to the resting position (defaultYPos)
        else if (IsBobbing
                 && (pos.y < defaultYPos - DEFAULT_Y_POS_TOLERANCE || pos.y > defaultYPos + DEFAULT_Y_POS_TOLERANCE))
        {
            pos.y = defaultYPos;
            camHeadTransform.localPosition = pos;

            IsBobbing = false;
        }
    }
}
