using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Controls the UI that displays active tasks to the player.
/// </summary>
public class TaskDisplayUIController : MonoBehaviour
{
    /// <summary>
    /// Gets whether or not the task display UI is showing active tasks to the player.
    /// </summary>
    public bool IsActive { get; private set; }

    [Title("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Title("Task Prefab Parent")]
    [SerializeField]
    private RectTransform taskSectionParent;

    [Title("Task Prefab")]
    [SerializeField]
    private GameObject taskPrefab;

    /// <summary>
    /// The list of tasks that are currently displayed in the UI.
    /// </summary>
    private readonly List<TaskDisplay> taskDisplays = new();

    [Title("Fade Settings")]
    [Tooltip("The time it takes for the informational display to fade in/out.")]
    [SerializeField]
    [Min(0f)]
    private float fadeTime = 0.075f;

    [Title("Hide On Awake?")]
    [SerializeField]
    private bool hideOnAwake = true;

    private Coroutine lerpRoutine = null;

    private TaskController taskController;

    private void Awake()
    {
        if (hideOnAwake)
        {
            Hide();
        }
    }

    private void OnEnable()
    {
        taskController = PawnShop.Instance.TaskController;

        taskController.OnTaskRegistered += OnTaskRegistered;
        taskController.OnTaskUnregistered += OnTaskUnregistered;

        // ChapterDirector is not expected to reasonably be null during gameplay, but this check is here to make
        // testing easier so I don't have to have a director scene loaded in when I want to test tasks
        if (ChapterDirector.Instance != null)
        {
            ChapterDirector.Instance.OnChapterPhaseUpdated += OnEnterNewChapterPhase;
        }

        // Get the tasks that have already been registered before this script has loaded in (if any)
        List<PlayerTask> registeredTasks = taskController.GetTasks();
        for (int i = 0; i < registeredTasks.Count; i++)
        {
            OnTaskRegistered(registeredTasks[i]);
        }
    }

    private void OnDisable()
    {
        taskController.OnTaskRegistered -= OnTaskRegistered;
        taskController.OnTaskUnregistered -= OnTaskUnregistered;

        if (ChapterDirector.Instance != null)
        {
            ChapterDirector.Instance.OnChapterPhaseUpdated -= OnEnterNewChapterPhase;
        }
    }

    private void OnTaskRegistered(PlayerTask task)
    {
        UpdateUI();
    }

    private void OnTaskUnregistered(PlayerTask task)
    {
        for (int i = taskDisplays.Count - 1; i >= 0; i--)
        {
            if (taskDisplays[i].Task == task)
            {
                task.OnCompleted -= OnTaskCompleted;
                Destroy(taskDisplays[i].gameObject);
                taskDisplays.RemoveAt(i);
                break;
            }
        }

        UpdateUI();
    }

    private void OnEnterNewChapterPhase(ChapterPhase newPhase)
    {
        Clear();
        UpdateUI();
    }

    private void OnTaskCompleted(PlayerTask task)
    {
        UpdateUI();
    }

    /// <summary>
    /// Displays the task display.
    /// </summary>
    public void Show()
    {
        SetVisible(true);
    }

    /// <summary>
    /// Hides the task display.
    /// </summary>
    public void Hide()
    {
        SetVisible(false);
    }

    /// <summary>
    /// Sets whether the task display is visible and fades it to the desired visibility state.
    /// </summary>
    /// <param name="isVisible">Whether the task display should be visible.</param>
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

    /// <summary>
    /// Update the tasks that are displayed based on task prerequisites.
    /// <para>
    /// If all tasks that are registered/active are completed, the display is hidden via <see cref="Hide"/>.
    /// </para>
    /// </summary>
    public void UpdateUI()
    {
        if (ShouldDisplay(out List<PlayerTask> tasksToDisplay))
        {
            for (int i = 0; i < tasksToDisplay.Count; i++)
            {
                PlayerTask task = tasksToDisplay[i];
                if (!IsTaskDisplayed(task) && task.CanComplete())
                {
                    DisplayTask(task);
                }
            }

            if (!IsActive)
            {
                Show();
            }
        }
        else if (IsActive)
        {
            Hide();
            Clear();
        }
    }

    /// <summary>
    /// Gets tasks from <see cref="TaskController"/> and determines if any should be displayed
    /// <para>
    /// If the tasks' <see cref="PlayerTask.Phase"/> matches that of the <see cref="ChapterDirector.CurrentPhase"/>
    /// and the tasks are not completed, those are the tasks that should be displayed.
    /// </para>
    /// </summary>
    /// <param name="tasksToDisplay">Outputs the list of tasks to display.</param>
    /// <returns>True if there are any tasks to display, false if otherwise.</returns>
    private bool ShouldDisplay(out List<PlayerTask> tasksToDisplay)
    {
        // If the director is null for testing, just get all of the tasks
        if (ChapterDirector.Instance == null)
        {
            tasksToDisplay = taskController.GetTasks();
        }
        // Get the tasks associated with the current chapter phase
        else
        {
            tasksToDisplay = taskController.GetTasks(ChapterDirector.Instance.CurrentPhase);
        }

        // The task display UI should only be shown if there are uncompleted tasks
        for (int i = tasksToDisplay.Count - 1; i >= 0; i--)
        {
            if (tasksToDisplay[i].IsCompleted)
            {
                tasksToDisplay.RemoveAt(i);
            }
        }

        return tasksToDisplay.Count > 0;
    }

    /// <summary>
    /// Check whether or not a specific task is currently being displayed.
    /// </summary>
    /// <param name="task">The task to check.</param>
    /// <returns>True if the task is being dispalyed by the UI, false if otherwise.</returns>
    public bool IsTaskDisplayed(PlayerTask task)
    {
        for (int i = 0; i < taskDisplays.Count; i++)
        {
            if (taskDisplays[i].Task.Equals(task))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Display a task to the UI.
    /// <para>
    /// When a task is displayed, it's also added to the <see cref="taskDisplays"/> list.
    /// </para>
    /// </summary>
    /// <param name="task">The task to display.</param>
    private void DisplayTask(PlayerTask task)
    {
        GameObject instantiatedPrefab = Instantiate(taskPrefab, taskSectionParent);

        TaskDisplay taskDisplay = instantiatedPrefab.GetComponent<TaskDisplay>();
        taskDisplay.TrySetTask(task);
        taskDisplays.Add(taskDisplay);

        task.OnCompleted += OnTaskCompleted;
    }

    /// <summary>
    /// Clear all displayed tasks from the UI.
    /// <para>
    /// This also empties the <see cref="taskDisplays"/> list.
    /// </para>
    /// </summary>
    private void Clear()
    {
        for (int i = taskDisplays.Count - 1; i >= 0; i--)
        {
            taskDisplays[i].Task.OnCompleted -= OnTaskCompleted;
            Destroy(taskDisplays[i].gameObject);
            taskDisplays.RemoveAt(i);
        }
    }
}
