using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Handles swapping between scenes with a fade using a canvas group.
/// <para>
/// Keeps the screen covered black while a scene is loading and waits for all registered scene objects to report
/// that they are ready before revealing the newly loaded scene.
/// </para>
/// </summary>
public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance { get; private set; }

    [Tooltip("The fade panel's canvas group component")]
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Title("Fade Settings")]
    [Tooltip("This delay time is only used when the game first boots up.")]
    [SerializeField]
    [Min(0f)]
    private float gameStartFadeDelay = 2f;

    [field: Tooltip("The time it takes for the black panel to fade in/out before changing scenes.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float FadeTime { get; private set; } = 1f;

    [field: Tooltip("Delays the fade in after the scene is ready to be shown.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float DelayTime { get; private set; } = 0.5f;

    /// <summary>
    /// Gets whether the initial reveal or a scene transition is still in progress.
    /// </summary>
    public bool IsLoadingScene { get; private set; }

    /// <summary>
    /// Keeps track of objects that need to be ready before the game scene should be shown.
    /// </summary>
    private readonly HashSet<MonoBehaviour> waitingForReady = new();
    private bool registrationOpen;

    /// <summary>
    /// Gets whether the current scene is still registering objects or waiting for registered objects to be ready.
    /// </summary>
    public bool IsAwaitingSceneReady => registrationOpen || waitingForReady.Count > 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            canvasGroup.alpha = 1f;

            // Wait for the first scene to finish registering its objects before the initial fade in
            IsLoadingScene = true;
            BeginSceneRegistration();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async UniTaskVoid Start()
    {
        CancellationToken onDestroyToken = this.GetCancellationTokenOnDestroy();

        try
        {
            await WaitForSceneReady(onDestroyToken);

            // Handles the initial game fade in (only occurs once every time on startup)
            await UniTask.Delay(
                TimeSpan.FromSeconds(gameStartFadeDelay), ignoreTimeScale: true, cancellationToken: onDestroyToken
            );

            await FadeIn(FadeTime, DelayTime, onDestroyToken);
        }
        catch (OperationCanceledException) when (onDestroyToken.IsCancellationRequested) { }
        finally
        {
            IsLoadingScene = false;
        }
    }
    
    /// <summary>
    /// Loads a new scene using the default fade out, delay, and fade in timings. This scene will not be loaded
    /// additively and will replace all existing scenes.
    /// <para>
    /// The new scene must finish registration, and its registered objects must report when they are ready.
    /// </para>
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns>A <see cref="UniTask"/> that completes once the fade out, scene load, ready notification, delay,
    /// and fade in sequence has finished.</returns>
    public async UniTask LoadSceneWithFade(string sceneName)
    {
        await LoadSceneWithFade(sceneName, FadeTime, DelayTime);
    }

    /// <summary>
    /// Loads a new scene with the specified fade timings and waits for the new scene to be ready before fading in.
    /// <para>
    /// The new scene must finish registration, and its registered objects must report when they are ready.
    /// </para>
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <param name="fadeTime">The duration of both the fade out and fade in animations.</param>
    /// <param name="delayTime">The delay after the scene is ready and before fading in.</param>
    /// <returns>A <see cref="UniTask"/> that completes once the entire transition sequence has finished.</returns>
    public async UniTask LoadSceneWithFade(string sceneName, float fadeTime, float delayTime)
    {
        if (IsLoadingScene)
        {
            return;
        }

        IsLoadingScene = true;
        CancellationToken onDestroyToken = this.GetCancellationTokenOnDestroy();

        try
        {
            // Wait until the screen is black before loading the new scene
            await FadeOut(fadeTime, onDestroyToken);

            // Open registration before loading so Awake calls in the new scene are not missed
            BeginSceneRegistration();
            await SceneHandler.LoadSceneAsync(sceneName);

            // Reset the time scale if the game was paused and loading a new scene
            Time.timeScale = 1f;

            await WaitForSceneReady(onDestroyToken);
            await FadeIn(fadeTime, delayTime, onDestroyToken);
        }
        catch (OperationCanceledException) when (onDestroyToken.IsCancellationRequested) { }
        catch (Exception e)
        {
            Debug.LogException(e);

            // A failed transition should not leave the screen permanently black
            canvasGroup.alpha = 0f;
        }
        finally
        {
            registrationOpen = false;
            waitingForReady.Clear();
            IsLoadingScene = false;
        }
    }

    /// <summary>
    /// Waits until registration is complete and all registered objects have reported that they are ready.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    private async UniTask WaitForSceneReady(CancellationToken token)
    {
        await UniTask.WaitUntil(() => !IsAwaitingSceneReady, cancellationToken: token);
    }

    /// <summary>
    /// Fades the black panel in to cover the screen.
    /// </summary>
    /// <param name="fadeTime">The time it takes to fade out.</param>
    /// <param name="token">The cancellation token.</param>
    private async UniTask FadeOut(float fadeTime, CancellationToken token)
    {
        await LerpUtils.LerpFloat(canvasGroup.alpha, 1f, fadeTime, a => canvasGroup.alpha = a, useUnscaledTime: true)
            .WithCancellation(token);
    }

    /// <summary>
    /// Fades the black panel out to reveal the screen.
    /// </summary>
    /// <param name="fadeTime">The time it takes to fade in.</param>
    /// <param name="delayTime">The time to wait before fading in.</param>
    /// <param name="token">The cancellation token.</param>
    private async UniTask FadeIn(float fadeTime, float delayTime, CancellationToken token)
    {
        // Await delay
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime), ignoreTimeScale: true, cancellationToken: token);

        await LerpUtils.LerpFloat(canvasGroup.alpha, 0f, fadeTime, a => canvasGroup.alpha = a, useUnscaledTime: true)
            .WithCancellation(token);
    }

    /// <summary>
    /// Registers an object whose setup must finish before the current scene is shown. Call this during Awake,
    /// before the scene closes registration.
    /// </summary>
    /// <param name="sceneObject">The object that will report when it is ready.</param>
    public void RegisterToBeAwaited(MonoBehaviour sceneObject)
    {
        if (sceneObject == null)
        {
            throw new ArgumentNullException(nameof(sceneObject));
        }

        if (!registrationOpen)
        {
            Debug.LogWarning($"{sceneObject.name} tried to register after scene registration was completed.");
            return;
        }

        waitingForReady.Add(sceneObject);
    }

    /// <summary>
    /// Signals that an object registered for the current scene has finished its setup.
    /// </summary>
    /// <param name="sceneObject">The object that finished its setup.</param>
    public void NotifySceneReady(MonoBehaviour sceneObject)
    {
        if (!waitingForReady.Remove(sceneObject))
        {
            string objectName = sceneObject == null ? "An object" : sceneObject.name;
            Debug.LogWarning($"{objectName} reported ready without being registered for this scene.");
        }
    }

    private void BeginSceneRegistration()
    {
        waitingForReady.Clear();
        registrationOpen = true;
    }

    /// <summary>
    /// Closes registration for the current scene. The fade in can begin once all registered objects are ready.
    /// <para>
    /// This should be called after all scenes that can contain registering objects have finished loading.
    /// </para>
    /// </summary>
    public void CompleteSceneRegistration()
    {
        registrationOpen = false;
    }
}
