using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    public event Action<PlayerTask> OnTaskRegistered;
    public event Action<PlayerTask> OnTaskUnregistered;

    /// <summary>
    /// Keeps track of what tasks belong to what <see cref="ChapterPhase"/>.
    /// </summary>
    private readonly Dictionary<ChapterPhase, List<PlayerTask>> taskDictionary = new();

    /// <summary>
    /// Register a task that needs to be completed by the player.
    /// <para>
    /// This fires off the <see cref="OnTaskRegistered"/> event.
    /// </para>
    /// </summary>
    /// <param name="task">The task that needs to be completed.</param>
    public void RegisterTask(PlayerTask task)
    {
        if (!taskDictionary.ContainsKey(task.Phase))
        {
            List<PlayerTask> newList = new();
            taskDictionary.Add(task.Phase, newList);
        }
        
        taskDictionary[task.Phase].Add(task);
        OnTaskRegistered?.Invoke(task);
    }

    /// <summary>
    /// Unregister a task.
    /// <para>
    /// This fires off the <see cref="OnTaskUnregistered"/> event.
    /// </para>
    /// </summary>
    /// <param name="task">The task to unregister.</param>
    /// <returns>True if the task was successfully unregistered, false if the task was never registered in the
    /// first place.</returns>
    public bool UnregisterTask(PlayerTask task)
    {
        if (taskDictionary.ContainsKey(task.Phase) && taskDictionary[task.Phase].Remove(task))
        {
            OnTaskUnregistered?.Invoke(task);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if a task is currently available to be completed.
    /// <para>
    /// A task is available if the task's <see cref="ChapterPhase"/> matches that of the 
    /// <see cref="ChapterDirector.CurrentPhase"/> and if all of its prerequisites are completed. If the
    /// <see cref="ChapterDirector.Instance"/> is null, the phase check is skipped, but prerequisites still apply.
    /// </para>
    /// <para>
    /// This method only determines whether or not the task is allowed to be completed whilst taking the pawn
    /// shop state and other tasks into account. The task itself is responsible for determining when its
    /// completion condition has been met.
    /// </para>
    /// </summary>
    /// <param name="task">The task to check whether or not it can be considered complete.</param>
    /// <returns>True if the task is currently available to be completed, false if otherwise.</returns>
    public bool IsTaskAvailable(PlayerTask task)
    {
        if (!ArePrerequisitesComplete(task))
        {
            return false;
        }

        // ChapterDirector is not expected to be null during gameplay in a build, but this makes testing easier
        // by allowing tasks to be considered available disregarding phases which are dependent on directors
        if (ChapterDirector.Instance == null)
        {
            return true;
        }

        return ChapterDirector.Instance.CurrentPhase == task.Phase;
    }

    /// <summary>
    /// Check if a task's prerequisite tasks are completed.
    /// <para>
    /// A prerequisite task is a task that has a lower <see cref="PlayerTask.TaskOrder"/> than the input task.
    /// </para>
    /// </summary>
    /// <param name="task">The task to check.</param>
    /// <returns>True if all tasks of the same <see cref="ChapterPhase"/> that have a lower order are completed,
    /// false if otherwise.</returns>
    private bool ArePrerequisitesComplete(PlayerTask task)
    {
        List<PlayerTask> taskList = taskDictionary[task.Phase];

        for (int i = 0; i < taskList.Count; i++)
        {
            if (taskList[i].TaskOrder < task.TaskOrder && !taskList[i].IsCompleted)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Check whether or not a list of tasks are completed.
    /// </summary>
    /// <param name="tasks">The tasks to see if they are all completed.</param>
    /// <returns>True if all of the <see cref="PlayerTask"/>s are completed, false if otherwise.</returns>
    public bool AreTasksComplete(IReadOnlyList<PlayerTask> tasks)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].IsCompleted)
            {
                return false;
            }
        }

        return true;     
    }
    
    /// <summary>
    /// Get all of the tasks that have been registered.
    /// </summary>
    /// <returns>A new list of tasks.</returns>
    public List<PlayerTask> GetTasks()
    {
        List<PlayerTask> tasks = new();

        foreach (var kvp in taskDictionary)
        {
            tasks.AddRange(kvp.Value);
        }

        return tasks;
    }

    /// <summary>
    /// Get the list of tasks associated with a specific <see cref="ChapterPhase"/>.
    /// </summary>
    /// <param name="phase">The <see cref="ChapterPhase"/> for the tasks.</param>
    /// <returns>A copied list of <see cref="PlayerTask"/>. If there are no tasks for the specified phase, then
    /// an empty list is returned.</returns>
    public List<PlayerTask> GetTasks(ChapterPhase phase)
    {
        List<PlayerTask> tasksForPhase = new();

        if (taskDictionary.ContainsKey(phase))
        {
            tasksForPhase.AddRange(taskDictionary[phase]);
        }

        return tasksForPhase;
    }

    /// <summary>
    /// Check if a <see cref="ChapterPhase"/> has any tasks associated with it.
    /// </summary>
    /// <param name="phase">The <see cref="ChapterPhase"/> to see if it has any tasks that need to be done.</param>
    /// <returns>True if the <see cref="ChapterPhase"/> has required tasks, false if otherwise.</returns>
    public bool DoesPhaseHaveTasks(ChapterPhase phase)
    {
        return taskDictionary.ContainsKey(phase) && taskDictionary[phase].Count > 0;
    }
}
