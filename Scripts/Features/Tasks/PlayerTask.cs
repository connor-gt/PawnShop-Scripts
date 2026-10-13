using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Base class for tasks the player needs to complete throughout a day's phases.
/// </summary>
public abstract class PlayerTask : MonoBehaviour
{
    /// <summary>
    /// Invoked when this task is completed.
    /// </summary>
    public event Action<PlayerTask> OnCompleted;

    /// <summary>
    /// Gets the <see cref="ChapterPhase"/> this task belongs to.
    /// <para>
    /// For example, if this task belongs to <see cref="ChapterPhase.Closing"/>, then this task is only
    /// available and displayed to the player during that phase.
    /// </para>
    /// </summary>
    [field: Title("What Phase For This Task?")]
    [field: Tooltip("The chapter phase that this task needs to completed in.")]
    [field: SerializeField]
    public ChapterPhase Phase { get; private set; }

    /// <summary>
    /// Gets the order group that determines when this task becomes available.
    /// <para>
    /// Tasks with the same order are grouped together and can be completed in any order. Tasks with a higher
    /// order become available to completed when tasks with an order below it are completed.
    /// </para>
    /// <para>
    /// For example, if there are 3 closing tasks of "Clean spill", "Lock doors", and "Go to bed", with the
    /// first 2 having an order of 0 and "Go to bed" having 10, the first 2 are displayed to the player and can
    /// be completed in any order but "Go to bed" is not displayed to the player and cannot be completed until
    /// the previous tasks are finished.
    /// </para>
    /// </summary>
    [field: Title("Order Grouping")]
    [field: Tooltip("Determines when this task becomes available relative to other tasks. This task is also "
        + "grouped with other tasks that have the same order.")]
    [field: SerializeField]
    [field: Range(-10, 10)]
    public int TaskOrder { get; private set; } = 0;

    /// <summary>
    /// Gets the message that's displayed to the player that describes what the player needs to do in order to
    /// complete this task.
    /// <para>
    /// For example: "Open the pawn shop by flipping the open/close sign to open".
    /// </para>
    /// </summary>
    [field: Title("Instruction Message")]
    [field: Tooltip("The message that's displayed to the player that describes how to complete this task.")]
    [field: SerializeField]
    public string RequirementMessage { get; private set; } = "Do something to complete this task";

    /// <summary>
    /// Gets whether or not this task has been completed by the player.
    /// </summary>
    public bool IsCompleted { get; private set; }

    private TaskController taskController;

    protected virtual void Start()
    {
        IsCompleted = false;
        taskController = PawnShop.Instance.TaskController;
        taskController.RegisterTask(this);
    }

    protected virtual void OnDestroy()
    {
        if (taskController != null)
        {
            taskController.UnregisterTask(this);
        }
    }

    /// <summary>
    /// Attempt to consider this task as completed.
    /// <para>
    /// See <see cref="CanComplete"/> to check the necessary conditions.
    /// </para>
    /// </summary>
    /// <returns>True if this task can be considered complete and is then completed, false if otherwise.</returns>
    protected bool TryComplete()
    {
        if (!IsCompleted && CanComplete())
        {
            NotifyCompleted();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Notify subscribers this task has been completed.
    /// </summary>
    private void NotifyCompleted()
    {
        IsCompleted = true;
        OnCompleted?.Invoke(this);
    }

    /// <summary>
    /// Determines whether this task is currently available to be completed.
    /// <para>
    /// This does not determine whether this task's completion condition(s) have been met, but only if the task
    /// is available to be completed taking into account the pawn shop's current state and other prerequisite
    /// tasks.
    /// </para>
    /// <para>
    /// Availability is checked by <see cref="TaskController.IsTaskAvailable(PlayerTask)"/> through
    /// <see cref="PawnShop.TaskController"/>. If the chapter director is missing, the phase check is skipped,
    /// but prerequisite tasks must still be completed.
    /// </para>
    /// </summary>
    /// <returns>True if this task is currently available to be completed, false if otherwise.</returns>
    public bool CanComplete()
    {
        return PawnShop.Instance.TaskController.IsTaskAvailable(this);
    }
}
