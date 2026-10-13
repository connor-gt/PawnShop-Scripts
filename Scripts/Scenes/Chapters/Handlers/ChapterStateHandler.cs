using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Manages the current phase and timing of a pawn shop day sequence.
/// </summary>
public class ChapterStateHandler : MonoBehaviour
{
    /// <summary>
    /// Invoked when the active <see cref="ChapterPhase"/> updates to the next phase.
    /// </summary>
    public event Action<ChapterPhase> OnChapterPhaseUpdated;

    /// <summary>
    /// Gets the current phase of the pawn shop day sequence.
    /// </summary>
    [field: Title("Current Day Phase - Exposed For Debug & Testing")]
    [field: SerializeField]
    public ChapterPhase CurrentPhase { get; private set; }

    [Title("Day Time Duration")]
    [Tooltip("The asset that's used to determine the day sequence duration.")]
    [SerializeField]
    private DaySequenceTimeSO dayTimeDuration;

    /// <summary>
    /// Gets the configured duration of the day sequence in seconds.
    /// </summary>
    public float DayTimeDuration => dayTimeDuration.Duration;

    /// <summary>
    /// Gets the amount of time that has elapsed since the day sequence began, in seconds.
    /// </summary>
    [ShowInInspector, ReadOnly]
    public float DayTimer { get; private set; }

    /// <summary>
    /// Gets the amount of time that has elapsed since the night sequence began, in seconds.
    /// </summary>
    [ShowInInspector, ReadOnly]
    public float NightTimer { get; private set; }

    /// <summary>
    /// Gets whether the day sequence timer is currently running.
    /// </summary>
    public bool IsDaySequenceActive => CurrentPhase == ChapterPhase.Day;

    private void Update()
    {
        switch (CurrentPhase)
        {
            case ChapterPhase.Opening:
            case ChapterPhase.Closing:
                break;
            case ChapterPhase.Day:
                HandleDayRoutine();
                break;
            case ChapterPhase.Night:
                NightTimer += Time.deltaTime;
                break;
        }
    }

    /// <summary>
    /// Begins the timed portion of the day sequence.
    /// <para>
    /// If the day sequence is already active, this method does nothing.
    /// </para>
    /// </summary>
    public void BeginDaySequence()
    {
        if (IsDaySequenceActive)
        {
            return;
        }

        EnterPhase(ChapterPhase.Day);
    }

    /// <summary>
    /// Updates the day timer and ends the day sequence when its configured duration has been reached.
    /// </summary>
    private void HandleDayRoutine()
    {
        DayTimer += Time.deltaTime;

        if (DayTimer > DayTimeDuration)
        {
            DayTimer = DayTimeDuration;
            EndDaySequence();
        }
    }

    /// <summary>
    /// Ends the active day sequence, transitions to the closing phase, and notifies subscribers.
    /// </summary>
    public void EndDaySequence()
    {
        if (!IsDaySequenceActive)
        {
            return;
        }

        EnterPhase(ChapterPhase.Closing);
    }

    /// <summary>
    /// Begins the night sequence.
    /// <para>
    /// If the night sequence is already active, this method does nothing.
    /// </para>
    /// </summary>
    public void BeginNightSequence()
    {
        EnterPhase(ChapterPhase.Night);
    }

    /// <summary>
    /// Enter a new phase and invoke the <see cref="OnChapterPhaseUpdated"/> event.
    /// <para>
    /// If the <see cref="CurrentPhase"/> equals the input phase, nothing happens.
    /// </para>
    /// </summary>
    /// <param name="newPhase">The <see cref="ChapterPhase"/> to go to.</param>
    [Button("Set New Phase")]
    [DisableInEditorMode]
    public void EnterPhase(ChapterPhase newPhase)
    {
        if (CurrentPhase == newPhase)
        {
            return;
        }

        CurrentPhase = newPhase;
        OnChapterPhaseUpdated?.Invoke(CurrentPhase);
#if UNITY_EDITOR
        Debug.Log($"Entering phase {newPhase}");
#endif
    }
}
