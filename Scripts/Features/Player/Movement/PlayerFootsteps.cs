using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Title("References")]
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private PlayerSprint playerSprint;

    [SerializeField]
    private PlayerCrouch playerCrouch;

    [field: SerializeField]
    public AudioSource AudioSource { get; private set; }

    /// <summary>
    /// Gets or sets the interval at which the footstep sound effect will be played while the player is walking.
    /// </summary>
    [field: Title("Settings")]
    [field: Tooltip("The base interval at which the footstep sound effect will be played.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float WalkStepInterval { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the volume that the footstep sound effect will be played at while the player is walking.
    /// </summary>
    public Vector2 WalkVolume = new(0.775f, 0.825f);

    /// <summary>
    /// Gets or sets the interval at which the footstep sound effect will be played while the player is sprinting.
    /// </summary>
    [field: Tooltip("The interval at which the footstep sound effect will be played while the player is sprinting.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float SprintStepInterval { get; set; } = 0.3f;

    /// <summary>
    /// Gets or sets the volume that the footstep sound effect will be played at while the player is sprinting.
    /// </summary>
    public Vector2 SprintVolume = new(1f, 1f);

    /// <summary>
    /// Gets or sets the interval at which the footstep sound effect will be played while the player is crouching.
    /// </summary>
    [field: Tooltip("The interval at which the footstep sound effect will be played while the player is crouching.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float CrouchStepInterval { get; set; } = 0.6f;

    /// <summary>
    /// Gets or sets the volume that the footstep sound effect will be played at while the player is crouching.
    /// </summary>
    public Vector2 CrouchVolume = new(0.5f, 0.55f);

    /// <summary>
    /// Gets the volume that should be played based on the player's current movement.
    /// </summary>
    private Vector2 GetVolume =>
        playerSprint.IsSprinting ? SprintVolume :
        playerCrouch.State == PlayerCrouch.CrouchState.Crouching ? CrouchVolume : WalkVolume;

    /// <summary>
    /// Gets the sound effect offset that should be used based on the player's current movement.
    /// </summary>
    private float GetOffset =>
        playerSprint.IsSprinting ? SprintStepInterval :
        playerCrouch.State == PlayerCrouch.CrouchState.Crouching ? CrouchStepInterval : WalkStepInterval;

    [field: Title("Sound Effects")]
    [field: SerializeField]
    public SoundEffectSO DefaultSFX { get; private set; }

    [field: SerializeField]
    public SoundEffectSO WoodSFX { get; private set; }

    [field: SerializeField]
    public SoundEffectSO GrassSFX { get; private set; }

    private SoundEffectPlayer soundEffectPlayer;

    [Title("Raycast")]
    [Tooltip("Footsteps only play when the player is above these layers. Raycast direction is Vector3.down")]
    [SerializeField]
    private LayerMask layerMask;

    /// <summary>
    /// For keeping track of the current tag detected by the raycast for playing certain sound effects on
    /// certain surfaces.
    /// </summary>
    private string currentTag = "Untagged";

    /// <summary>
    /// Offset for the raycast so it doesn't start at (0, 0, 0) (assuming the player's transform.position is
    /// (0, 0, 0)).
    /// </summary>
    [Tooltip("Offset for the raycast so it doesn't start at (0, 0, 0) (assuming the player's transform.position"
        + "is (0, 0, 0)).")]
    [SerializeField]
    private Vector3 raycastOffset = new(0f, 1f, 0f);

    [Tooltip("The maximum distance the raycast will have when fired off.")]
    [SerializeField]
    [Min(0f)]
    private float raycastMaxDistance = 3f;

    /// <summary>
    /// For keeping track of when to play footstep sound effects.
    /// </summary>
    private float timer = 0f;

    private void Awake()
    {
        soundEffectPlayer = new SoundEffectPlayer(DefaultSFX);
    }

    private void Update()
    {
        HandleAudio();
    }

    private void HandleAudio()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f && playerMovement.Direction.sqrMagnitude > 0f)
        {
            if (Physics.Raycast(
                transform.position + raycastOffset,
                Vector3.down,
                out RaycastHit hit,
                raycastMaxDistance,
                layerMask)
            ) {
                // Swap the sound effect if the ground layer changes (i.e. moving from grass to tile)
                if (!hit.collider.CompareTag(currentTag))
                {
                    currentTag = hit.collider.tag;
                    soundEffectPlayer.SoundEffect = GetSFXBasedOnRaycastHit(hit);
                }

                soundEffectPlayer.Volume = GetVolume;
                soundEffectPlayer.Play(AudioSource);
            }

            timer = GetOffset;
        }
    }

    /// <summary>
    /// Determines which <see cref="SoundEffectSO"/> should be used based on the surface detected by the
    /// raycast.
    /// </summary>
    /// <param name="hit">The raycast hit. The hit's collider is evaluated via a switch statement.</param>
    /// <returns>A sound effect based on the current tag and if none match, returns <see cref="DefaultSFX"/>."
    /// </returns>
    private SoundEffectSO GetSFXBasedOnRaycastHit(RaycastHit hit)
    {
        switch (hit.collider.tag)
        {
            case "Ground/Wood":
                return WoodSFX;
            case "Ground/Grass":
                return GrassSFX;
            case "Ground/Default":
            default:
                return DefaultSFX;
        }
    }
}
