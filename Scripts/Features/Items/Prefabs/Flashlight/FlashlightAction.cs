using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightAction : MonoBehaviour, IHoldAction
{
    [field: Title("Input Action")]
    [field: SerializeField]
    public HoldActionInputSO HoldActionInput { get; private set; }

    public TemporaryInputBinding InputBinding { get; private set; }

    [field: Title("Light")]
    [field: SerializeField]
    public Light LightSource { get; private set; }

    /// <summary>
    /// Gets or sets the time it takes to fade the light in/out.
    /// </summary>
    [field: Title("Settings")]
    [field: Tooltip("The time it takes the light to fade in/out.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float FadeTime { get; set; } = 0.2f;

    [Tooltip("If enabled, the light will be turned off in Awake().")]
    [SerializeField]
    private bool turnOffOnAwake = true;

    /// <summary>
    /// Gets the default intensity of the light source, which is the starting intensity set in the inspector.
    /// </summary>
    public float DefaultIntensity { get; private set; }

    /// <summary>
    /// Gets whether or not the light is currently transitioning states. While the light is transitioning from
    /// on to off or vice versa, a new transition cannot be started.
    /// </summary>
    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        InputBinding = new TemporaryInputBinding(HoldActionInput.InputReference.action, Toggle);

        DefaultIntensity = LightSource.intensity;

        if (turnOffOnAwake)
        {
            LightSource.intensity = 0f;
            LightSource.enabled = false;
        }
    }

    /// <summary>
    /// Toggle the flashlight on/off.
    /// </summary>
    public async void Toggle(InputAction.CallbackContext ctx)
    {
        try
        {
            if (!LightSource.enabled)
            {
                await FadeIn();
            }
            else
            {
                await FadeOut();
            }
        } catch (OperationCanceledException) { }
        finally
        {
            IsTransitioning = false;
        }
    }

    /// <summary>
    /// Fade the flashlight in by lerping its intensity from its current value (should be 0f since the light
    /// is turning on and fading in) to <see cref="DefaultIntensity"/>.
    /// </summary>
    /// <returns>A <see cref="UniTask"/> that completes once the entire transition sequence has finished.</returns>
    public async UniTask FadeIn()
    {
        if (IsTransitioning)
        {
            return;
        }
        
        IsTransitioning = true;
        LightSource.enabled = true;

        var token = this.GetCancellationTokenOnDestroy();
        await LerpUtils.LerpFloat(LightSource.intensity, DefaultIntensity, FadeTime, i => LightSource.intensity = i)
            .WithCancellation(token);

        IsTransitioning = false;
    }

    /// <summary>
    /// Fade the flashlight out by lerping its intensity from its current value (should be > 0f since the light
    /// is fading out then turning off) to 0f. The light source is disabled when this method is completed.
    /// </summary>
    /// <returns>A <see cref="UniTask"/> that completes once the entire transition sequence has finished.</returns>
    public async UniTask FadeOut()
    {
        if (IsTransitioning)
        {
            return;
        }

        IsTransitioning = true;

        var token = this.GetCancellationTokenOnDestroy();
        await LerpUtils.LerpFloat(LightSource.intensity, 0f, FadeTime, i => LightSource.intensity = i)
            .WithCancellation(token);

        LightSource.enabled = false;
        IsTransitioning = false;
    }
}
