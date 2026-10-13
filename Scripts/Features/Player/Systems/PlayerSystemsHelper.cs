using Unity.Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Helper class for managing the player's systems, such as movement, input, and character controller.
/// </summary>
public class PlayerSystemsHelper : MonoBehaviour
{
    [Title("Player Root Reference")]
    [SerializeField]
    private Transform playerRoot;

    [Title("Character Controller Reference")]
    [SerializeField]
    private CharacterController characterController;

    [Title("Movement/Input References")]
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private CinemachineInputAxisController inputAxisController;

    /// <summary>
    /// Move and rotate the player's root transform to match the given target transform. The offset can be used
    /// as an optional positional adjustment.
    /// <para>
    /// The player's CharacterController component is temporarily disabled during the move to prevent any issues
    /// with the player's position being incorrectly set due to the CharacterController's collision detection.
    /// </para>
    /// </summary>
    /// <param name="target">The target transform to move the player to.</param>
    /// <param name="offset">The offset to apply to the player's position.</param>
    public void MoveRootToTransform(Transform target, Vector3 offset = default)
    {
        // Disable player's CharacterController component because I noticed an inconsistent bug in that the
        // player would occasionally spawn in and stay at world origin instead of being moved to the spawnPos.
        // Disabling CharacterController seems to fix this issue
        characterController.enabled = false;
        playerRoot.SetPositionAndRotation(target.position + offset, target.rotation);
        characterController.enabled = true;
    }

    /// <summary>
    /// Enable or disable the player's movement and input controls.
    /// </summary>
    /// <param name="enable">Whether to enable the controls.</param>
    public void EnableControls(bool enable)
    {
        InputManager.Instance.EnableMap(InputManager.Instance.Actions.Base, enable);

        inputAxisController.enabled = enable;
        playerMovement.enabled = enable;
    }
}
