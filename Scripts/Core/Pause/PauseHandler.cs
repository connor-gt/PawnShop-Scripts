using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseHandler : MonoBehaviour
{
    public static event Action OnPause;
    public static event Action OnResume;

    [Title("Pause Menu GUI")]
    [Tooltip("The pause menu is shown when paused and hidden when unpaused.")]
    [SerializeField]
    private CanvasGroup pauseMenuCG;

    [Tooltip("Hide the pause menu when the game starts.")]
    [SerializeField]
    private bool hideOnStart = true;

    /// <summary>
    /// Gets whether or not the game is currently paused.
    /// </summary>
    public static bool IsPaused { get; private set; }

    // A slight delay from pausing and re-pausing to ensure no mixups occur with enabling/disabling action maps
    private bool pauseBlocked;
    private const float TIME_TO_BLOCK = 0.2f;

    private void OnEnable()
    {
        InputManager.Instance.Actions.Pause.Toggle.performed += HandlePausePerformed;
    }

    private void OnDisable()
    {
        InputManager.Instance.Actions.Pause.Toggle.performed -= HandlePausePerformed;

        // Re-enable the base map in case the player pauses, goes back to the main menu, then enters another
        // game scene. If this does not happen, then if loading back into a game scene the player cannot move
        InputManager.Instance.EnableMap(InputManager.Instance.Actions.Base, true);
    }

    private void Start()
    {
        if (hideOnStart)
        {
            pauseMenuCG.alpha = 0f;
        }
    }

    private void HandlePausePerformed(InputAction.CallbackContext ctx)
    {
        if (pauseBlocked)
        {
            return;
        }

        if (IsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }

        pauseBlocked = true;
        this.Invoke(() => pauseBlocked = false, TIME_TO_BLOCK);
    }

    public void Pause()
    {
        IsPaused = true;
        CursorManager.Instance.UnlockCursor();

        InputManager.Instance.EnableMap(InputManager.Instance.Actions.Base, false);
        InputManager.Instance.EnableMap(InputManager.Instance.Actions.UI, true);

        // Show pause menu
        pauseMenuCG.alpha = 1f;

        Time.timeScale = 0;
        OnPause?.Invoke();
    }

    public void Resume()
    {
        IsPaused = false;
        CursorManager.Instance.LockCursor();

        // Hide pause menu
        pauseMenuCG.alpha = 0f;

        InputManager.Instance.EnableMap(InputManager.Instance.Actions.UI, false);
        InputManager.Instance.EnableMap(InputManager.Instance.Actions.Base, true);

        Time.timeScale = 1;
        OnResume?.Invoke();
    }
}
