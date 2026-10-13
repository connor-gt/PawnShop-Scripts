using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Defines the duration of a day sequence (how long the actual day will last after the opening sequence).
/// </summary>
[CreateAssetMenu(fileName = "NewDaySequenceTime", menuName = "Scriptable Objects/Day Management/New Day Sequence Time")]
public class DaySequenceTimeSO : ScriptableObject
{
    /// <summary>
    /// Gets the duration of the day sequence in seconds.
    /// </summary>
    [field: Title("Day Sequence Length")]
    [field: Tooltip("The amount of time, in seconds, that the day sequence lasts.")]
    [field: SuffixLabel("seconds")]
    [field: SerializeField]
    [field: Min(0f)]
    public float Duration { get; private set; }
}
