using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    public static GameplayUIController Instance { get; private set; }

    /// <summary>
    /// Gets whether or not the gameplay UI is currently active and visible to the player.
    /// </summary>
    public bool IsActive { get; private set; }

    [Title("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Title("Fade Settings")]
    [Tooltip("The time it takes for the gameplay UI to fade in/out.")]
    [SerializeField]
    [Min(0f)]
    private float fadeTime = 0.33f;

    private Coroutine lerpRoutine = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Displays the gameplay UI.
    /// </summary>
    public void Show()
    {
        SetVisible(true);
    }

    /// <summary>
    /// Hides the gameplay UI.
    /// </summary>
    public void Hide()
    {
        SetVisible(false);
    }

    /// <summary>
    /// Sets whether the gameplay UI is visible and fades it to the desired visibility state.
    /// </summary>
    /// <param name="isVisible">Whether the gameplay UI should be visible.</param>
    private void SetVisible(bool isVisible)
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }

        IsActive = isVisible;
        float targetAlpha = isVisible ? 1f : 0f;

        lerpRoutine = StartCoroutine(
            LerpUtils.LerpFloat(canvasGroup.alpha, targetAlpha, fadeTime, a => canvasGroup.alpha = a)
        );
    }
}
