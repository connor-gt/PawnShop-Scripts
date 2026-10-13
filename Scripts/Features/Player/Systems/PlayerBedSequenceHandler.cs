using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Handles temporarily taking control away from the player during awake/sleep cutscenes.
/// <para>
/// This disables player input, movement, the player's character controller, and hides held objects while switching
/// the active Cinemachine camera to the cutscene camera.
/// </para>
/// <para>
/// This works by assigning the player's main cinemachine camera a low priority and assumes there is another virtual
/// Cinemachine camera with a higher priority. The transition is handled by Unity's blending between the cameras.
/// </para>
/// </summary>
public class PlayerBedSequenceHandler : MonoBehaviour
{
    [Title("Cinemachine References")]
    [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [SerializeField]
    private CinemachineBrain cinemachineBrain;

    /// <summary>
    /// The priority assigned to the player camera while the player is in the bed transition cutscene.
    /// <para>
    /// This should be lower than the camera priority used during normal gameplay.
    /// </para>
    /// </summary>
    [Title("Priority")]
    [Tooltip("Priority used while the player is in the going to bed transition cutscene. Should be lower than the"
        + " normal gameplay priority found on the player's main cinemachine camera.")]
    [SerializeField]
    private int goingToBedPriority = -99;

    /// <summary>
    /// The player camera's priority during normal gameplay.
    /// </summary>
    private int defaultPriority;

    [Title("Hold Transform Reference")]
    [Tooltip("This is disabled when entering a cutscene and re-enabled when control is given back to the player.")]
    [SerializeField]
    private GameObject holdTransform;

    [Title("Character Controller Reference")]
    [SerializeField]
    private CharacterController characterController;

    [Title("Interaction References")]
    [SerializeField]
    private HoverableHandler hoverableHandler; // For disabling to clear hover text if needed

    [Title("Helper Reference")]
    [SerializeField]
    private PlayerSystemsHelper playerSystemsHelper;

    private void Awake()
    {
        defaultPriority = cinemachineCamera.Priority;
    }

    /// <summary>
    /// Starts the cutscene for going to bed.
    /// </summary>
    [Sirenix.OdinInspector.Button]
    public void GoToBed()
    {
        StartCoroutine(GoToBedTransition());
    }

    /// <summary>
    /// Starts the cutscene for waking up.
    /// </summary>
    [Sirenix.OdinInspector.Button]
    public void WakeUp()
    {
        StartCoroutine(WakeUpTransition());
    }

    /// <summary>
    /// Starts the camera blend into the bed and waits until it has finished.
    /// </summary>
    public IEnumerator GoToBedTransition()
    {
        yield return TransitionRoutine(goingToBed: true);
    }

    /// <summary>
    /// Starts the camera blend out of the bed and waits until it has finished.
    /// </summary>
    public IEnumerator WakeUpTransition()
    {
        yield return TransitionRoutine(goingToBed: false);
    }

    /// <summary>
    /// Changes the camera priority and enables/disables player controls depending on whether the transition
    /// cutscene is being entered into/exited.
    /// <para>
    /// If waking up, player control is not given back until the blend between cameras is finished.
    /// </para>
    /// </summary>
    /// <param name="goingToBed">Whether the player is going to bed.</param>
    private IEnumerator TransitionRoutine(bool goingToBed)
    {
        cinemachineCamera.Priority = goingToBed ? goingToBedPriority : defaultPriority;

        if (goingToBed)
        {
            playerSystemsHelper.EnableControls(false);
            characterController.enabled = false;
            hoverableHandler.enabled = false;
            holdTransform.SetActive(false);

            // It's reasonable to expect the gameplay UI scene to always be loaded with the player
            GameplayUIController.Instance.Hide();
        }

        // Give Cinemachine a moment to begin the blend before checking whether the camera is participating
        yield return new WaitForSeconds(0.15f);

        // Wait until the transition has finished in either direction
        while (cinemachineCamera.IsParticipatingInBlend())
        {
            yield return null;
        }

        if (!goingToBed)
        {
            playerSystemsHelper.EnableControls(true);
            characterController.enabled = true;
            hoverableHandler.enabled = true;
            holdTransform.SetActive(true);

            GameplayUIController.Instance.Show();
        }
    }

    /// <summary>
    /// Instantly switches the player's camera to the bed camera by temporarily setting the blend to a cut.
    /// </summary>
    public async UniTask InstantCameraSwitchToBed()
    {
        if (GameplayUIController.Instance != null)
        {
            GameplayUIController.Instance.Hide();
        }

        await InstantCameraSwitchToBedRoutine().WithCancellation(this.GetCancellationTokenOnDestroy());
    }

    /// <summary>
    /// Handles the instant camera switch to the bed camera by temporarily setting the blend to a cut and then
    /// restoring the default blend after a few frames.
    /// </summary>
    private IEnumerator InstantCameraSwitchToBedRoutine()
    {
        // Save the default blend and temporarily set the blend to a cut so that the camera transition is instant
        CinemachineBlendDefinition defaultBlend = cinemachineBrain.DefaultBlend;
        CinemachineBlendDefinition cutBlend = new() { Style = CinemachineBlendDefinition.Styles.Cut };

        cinemachineBrain.DefaultBlend = cutBlend;
        cinemachineCamera.Priority = goingToBedPriority;

        yield return new WaitForSeconds(0.1f); // Wait a few frames to ensure the camera cut is finished
        cinemachineBrain.DefaultBlend = defaultBlend;
    }
}
