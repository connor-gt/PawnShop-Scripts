using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Handles spawning the player. This script belongs on a single game object that will act as the spawn point.
/// </summary>
public class SpawnPlayer : MonoBehaviour
{
    public static SpawnPlayer Instance { get; private set; }

    /// <summary>
    /// Fired off once the player scene and gameplay UI scene finishes loading.
    /// </summary>
    public static event Action<GameObject> OnPlayerLoaded;

    [Title("Scenes")]
    [SerializeField]
    private SceneField playerScene;

    [SerializeField]
    private SceneField gameplayUIScene;

    [Title("Spawn Settings")]
    [Tooltip("For ensuring the player spawns slightly above ground to prevent rare glitches of the player "
        + "falling through.")]
    [SerializeField]
    [Range(0, 1)]
    private float spawnHeightOffset = 0.3f;

    [Tooltip("Optional delay to wait before spawning the player in Start(). This is useful to make sure other"
        + " have a chance to initialize before the player is spawned in.")]
    [SerializeField]
    [Min(0f)]
    private float spawnDelay = 0.33f;

    /// <summary>
    /// Gets a reference to the player's root game object.
    /// </summary>
    public GameObject Player { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneSwapManager.Instance.RegisterToBeAwaited(this);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{gameObject.name} is an excess SpawnPlayer instance, destroying this object.");
#endif
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        if (spawnDelay > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
        }

        await LoadPlayerAsync(transform);
    }

    /// <summary>
    /// Additively load the player scene in and move the player to the desired <paramref name="spawnLocation"/>.
    /// </summary>
    /// <param name="spawnLocation">The location the player will be spawned at. The player is also rotated to
    /// match this Transform's rotation.</param>
    /// <returns>A task that completes once the player scene has finished loading and the player is moved to
    /// the spawn location.</returns>
    public async UniTask LoadPlayerAsync(Transform spawnLocation)
    {
        if (SceneHandler.IsSceneLoaded(playerScene))
        {
#if UNITY_EDITOR
            Debug.LogWarning("SpawnPlayer attempting to spawn player while the player scene is already loaded in!");
#endif
            return;
        }

        await SceneHandler.LoadSceneAsync(playerScene, additive: true);
        Player = GameObject.FindWithTag("Player");

        // Audio is disabled to prevent the player from hearing things while being moved to the spawn location
        GameObject camera = Camera.main.gameObject;
        if (camera.TryGetComponent(out AudioListener audioListener))
        {
            audioListener.enabled = false;
        }

        // Move the player to the spawn location, apply the height offset, and rotate the player
        PlayerSystemsHelper playerHelper = Player.GetComponentInChildren<PlayerSystemsHelper>();
        playerHelper.MoveRootToTransform(spawnLocation, new Vector3(0f, spawnHeightOffset, 0f));

        // Re-enable audio
        if (audioListener != null)
        {
            audioListener.enabled = true;
        }

        await LoadGameplayUiAsync();
        OnPlayerLoaded?.Invoke(Player);
        SceneSwapManager.Instance.NotifySceneReady(this);
    }

    /// <summary>
    /// Asynchronously loads the gameplay UI scene additively, if it is valid and not already loaded.
    /// </summary>
    /// <returns>A task that completes once the gameplay UI scene has finished loading, or immediately if the
    /// scene reference is null or the scene is already loaded.</returns>
    public async UniTask LoadGameplayUiAsync()
    {
        if (gameplayUIScene == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("LoadGameplayUiScene was called but the parameter gameplayUiScene was null.");
#endif
            return;
        }
        else if (SceneHandler.IsSceneLoaded(gameplayUIScene))
        {
#if UNITY_EDITOR
            Debug.LogWarning("gameplayUiScene was attempted to load while it's already loaded in!");
#endif
            return;
        }

        await SceneHandler.LoadSceneAsync(gameplayUIScene, additive: true);
    }
}
