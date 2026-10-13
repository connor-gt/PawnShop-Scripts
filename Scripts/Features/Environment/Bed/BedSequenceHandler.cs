using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents one blink instance.
/// </summary>
[Serializable]
public struct BlinkSegment
{
    /// <summary>
    /// How long it takes for the lids to open for this blink.
    /// </summary>
    public float openDuration;

    /// <summary>
    /// How long the eyes stay open for this blink.
    /// </summary>
    public float openPauseDuration;

    /// <summary>
    /// How long it takes for the lids to close for this blink.
    /// </summary>
    public float closeDuration;

    /// <summary>
    /// How long the eyes stay closed after this blink.
    /// </summary>
    public float closedPauseDuration;
}

public class BedSequenceHandler : MonoBehaviour
{
    public event Action OnPlayerAwake;
    public event Action OnPlayerAsleep;

    [Title("Wake Up Location Reference")]
    [Tooltip("The location the player is at when waking up from bed.")]
    [SerializeField]
    private Transform wakeUpPoint;

    [Title("Blink Settings")]
    [SerializeField]
    private Shader bedBlinkShader;

    [Tooltip("The Canvas sorting order used by the bed sequence cover.")]
    [SerializeField]
    private int coverSortingOrder = 10;

    [Title("Blink Data")]
    [SerializeField]
    private BlinkSegment[] blinks;

    [Title("Eye Lid Settings")]
    [Tooltip("Controls how soft the eyelid edge appears.")]
    [SerializeField]
    [Range(0.001f, 0.1f)]
    private float lidEdgeSoftness = 0.02f;

    [Tooltip("Controls how much the eyelid opening curves toward its corners.")]
    [SerializeField]
    [Range(0f, 0.2f)]
    private float lidCurve = 0.08f;

    private RawImage screenCover;
    private Material screenCoverMaterial;
    private float eyeOpen = 1f;
    private Coroutine activeSequence;

    // DO NOT EDIT - Unless the shader names need to be changed for whatever reason
    private static readonly int EyeOpenProperty = Shader.PropertyToID("_EyeOpen");
    private static readonly int EdgeSoftnessProperty = Shader.PropertyToID("_EdgeSoftness");
    private static readonly int LidCurveProperty = Shader.PropertyToID("_LidCurve");

    // Lazy initialization of the player & bed sequence handler since it's unknown when exactly the player will
    // load in relative to this script
    private GameObject _player;
    private GameObject Player
    {
        get
        {
            if (_player == null)
            {
                _player = PawnShop.Instance.Player;
            }

            return _player;
        }
    }

    private PlayerBedSequenceHandler _playerBedSequenceHandler;
    private PlayerBedSequenceHandler PlayerBedSequenceHandler
    {
        get
        {
            if (_playerBedSequenceHandler == null)
            {
                _playerBedSequenceHandler = Player.GetComponentInChildren<PlayerBedSequenceHandler>();
            }

            return _playerBedSequenceHandler;
        }
    }

    private void Awake()
    {
        CreateScreenCover();
    }

    private void OnDestroy()
    {
        if (screenCoverMaterial != null)
        {
            Destroy(screenCoverMaterial);
        }
    }

    /// <summary>
    /// Initialize the wake up sequence by moving the player to the wake up point and switching the camera to the
    /// bed camera. When <see cref="WakeUpSequence"/> is called, a blinking effect will occur, the player's camera
    /// will transition to the main camera, then the player will be able to move around normally.
    /// </summary>
    [Button]
    public async UniTask InitializeWakeUpSequence()
    {
        // Keep the bed scene covered until the wake-up sequence
        SetLidsClosed();

        // Move the player to the wake up point (so the camera transitions from the bed to this spot)
        PlayerSystemsHelper playerHelper = Player.GetComponentInChildren<PlayerSystemsHelper>();
        playerHelper.MoveRootToTransform(wakeUpPoint);
        playerHelper.EnableControls(false);

        // Set the player's camera to be the virtual camera on the bed
        await PlayerBedSequenceHandler.InstantCameraSwitchToBed();
    }

    /// <summary>
    /// Begin the wake up sequence.
    /// </summary>
    [Button]
    public void WakeUpSequence()
    {
        if (activeSequence == null)
        {
            activeSequence = StartCoroutine(WakeUpRoutine());
        }
    }

    /// <summary>
    /// Begin the go to bed sequence.
    /// </summary>
    [Button]
    public void GoToBedSequence()
    {
        if (activeSequence == null)
        {
            activeSequence = StartCoroutine(GoToBedRoutine());
        }
    }

    /// <summary>
    /// Handles the wake up sequence.
    /// <para>
    /// Waits for the scene swap to finish, blinks the player's eyes a few times, then starts the cinemachine blend
    /// to the main camera while opening the lids to reveal the scene.
    /// </para>
    /// </summary>
    private IEnumerator WakeUpRoutine()
    {
        // Wait until SceneSwapManager has finished its own reveal so its cover and the bed cover don't fight
        while (SceneSwapManager.Instance.IsLoadingScene)
        {
            yield return null;
        }

        yield return StartCoroutine(BlinkRoutine(endWithEyesOpen: true, reverseOrder: false));
        yield return StartCoroutine(PlayerBedSequenceHandler.WakeUpTransition());

        activeSequence = null;
        OnPlayerAwake?.Invoke();
    }

    /// <summary>
    /// Handles the go to bed sequence.
    /// <para>
    /// Waits for the cinemachine blend to finish, blinks the player's eyes a few times, then completes the day.
    /// </para>
    /// </summary>
    private IEnumerator GoToBedRoutine()
    {
        // Let the existing Cinemachine blend finish before closing the player's eyes
        yield return PlayerBedSequenceHandler.GoToBedTransition();
        yield return StartCoroutine(BlinkRoutine(endWithEyesOpen: false, reverseOrder: true));

        activeSequence = null;
        OnPlayerAsleep?.Invoke();
    }

    /// <summary>
    /// Blinks the player's eyes a number of times by opening and closing the lids.
    /// <para>
    /// The number of blinks and the durations is set by <see cref="blinks"/>.
    /// </para>
    /// </summary>
    /// <param name="endWithEyesOpen">Whether or not to end the blinking with the screen visible or not.</param>
    /// <param name="reverseOrder">Whether to play the blink segments from last to first.</param>
    [Button]
    private IEnumerator BlinkRoutine(bool endWithEyesOpen, bool reverseOrder)
    {
        // Known quirk: when going in reverse for the going to bed sequence, the last blink isn't played. Since
        // I have 2 blinks in the inspector, only one blink plays before staying dark but I think this works in
        // my favor since closing the eyes then going to bed is a nice and simple transition

        int blinkIndex = reverseOrder ? blinks.Length - 1 : 0;
        int finalBlinkIndex = reverseOrder ? 0 : blinks.Length - 1;
        int indexStep = reverseOrder ? -1 : 1;

        for (; blinkIndex != finalBlinkIndex; blinkIndex += indexStep)
        {
            yield return StartCoroutine(OpenLidsRoutine(blinks[blinkIndex].openDuration));
            yield return new WaitForSecondsRealtime(blinks[blinkIndex].openPauseDuration);
            yield return StartCoroutine(CloseLidsRoutine(blinks[blinkIndex].closeDuration));
            yield return new WaitForSecondsRealtime(blinks[blinkIndex].closedPauseDuration);
        }

        yield return StartCoroutine(
            endWithEyesOpen
            ? OpenLidsRoutine(blinks[finalBlinkIndex].openDuration)
            : CloseLidsRoutine(blinks[finalBlinkIndex].closeDuration));
    }

    /// <summary>
    /// Creates a screen-space Canvas owned by this bed sequence and places its black lids off-screen.
    /// </summary>
    private void CreateScreenCover()
    {
        GameObject canvasObject = new("Bed Sequence Canvas", typeof(RectTransform));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = coverSortingOrder;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        if (bedBlinkShader == null)
        {
            Debug.LogError("Bed blink shader is not assigned.", this);
            return;
        }

        screenCoverMaterial = new Material(bedBlinkShader)
        {
            name = "Bed Sequence Blink (Runtime)"
        };
        screenCoverMaterial.SetFloat(EdgeSoftnessProperty, lidEdgeSoftness);
        screenCoverMaterial.SetFloat(LidCurveProperty, lidCurve);

        GameObject coverObject = new("Bed Sequence Cover", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        coverObject.transform.SetParent(canvasObject.transform, false);

        RectTransform coverRectTransform = coverObject.GetComponent<RectTransform>();
        coverRectTransform.anchorMin = Vector2.zero;
        coverRectTransform.anchorMax = Vector2.one;
        coverRectTransform.offsetMin = Vector2.zero;
        coverRectTransform.offsetMax = Vector2.zero;

        screenCover = coverObject.GetComponent<RawImage>();
        screenCover.texture = Texture2D.whiteTexture;
        screenCover.color = Color.white;
        screenCover.material = screenCoverMaterial;
        screenCover.raycastTarget = false;

        Canvas.ForceUpdateCanvases();
        SetLidsOpen();
    }

    /// <summary>
    /// Opens the lids over the specified duration, moving them off-screen to reveal the scene.
    /// </summary>
    /// <param name="duration">The duration of the opening animation.</param>
    private IEnumerator OpenLidsRoutine(float duration)
    {
        yield return LerpUtils.LerpFloat(eyeOpen, 1f, duration, SetEyeOpen, useUnscaledTime: true);
    }

    /// <summary>
    /// Closes the lids over the specified duration, moving them back into place.
    /// </summary>
    /// <param name="duration">The duration of the closing animation.</param>
    private IEnumerator CloseLidsRoutine(float duration)
    {
        yield return LerpUtils.LerpFloat(eyeOpen, 0f, duration, SetEyeOpen, useUnscaledTime: true);
    }

    /// <summary>
    /// Set the lids to their closed position immediately.
    /// </summary>
    [Button]
    private void SetLidsClosed()
    {
        SetEyeOpen(0f);
    }

    /// <summary>
    /// Set the lids to their open position immediately, moving them off-screen to reveal the scene.
    /// </summary>
    [Button]
    private void SetLidsOpen()
    {
        SetEyeOpen(1f);
    }

    /// <summary>
    /// Sets the eye openness value and updates the shader property on the screen cover material.
    /// <para>
    /// 1 == eyes are fully open, 0 == eyes are fully closed. Any value in between is lerped.
    /// </para>
    /// </summary>
    /// <param name="openness">The desired eye openness value.</param>
    private void SetEyeOpen(float openness)
    {
        eyeOpen = Mathf.Clamp01(openness);

        if (screenCoverMaterial == null)
        {
            return;
        }

        screenCoverMaterial.SetFloat(EyeOpenProperty, eyeOpen);
    }
}
