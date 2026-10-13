#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Editor-only class that's used for testing gameplay (like NPC behavior) over longer periods by speeding up
/// the game's simulation speed.
/// </summary>
public class TimeManager : MonoBehaviour
{
    [Tooltip("Whether or not this component can influence the time scale.")]
    [SerializeField]
    private bool adjustTime;

    [Tooltip("The speed at which gameplay occurs.")]
    [SerializeField]
    [Min(0f)]
    private float timeScale = 1f;

    private void OnDisable()
    {
        ResetTime();
    }

    private void Update()
    {
        if (adjustTime)
        {
            Time.timeScale = timeScale;
        }
    }

    /// <summary>
    /// Resets the time back to 1f.
    /// </summary>
    [Button]
    public void ResetTime()
    {
        Time.timeScale = 1f;
    }
}
#endif
