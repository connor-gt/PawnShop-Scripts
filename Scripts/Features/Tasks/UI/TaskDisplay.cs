using TMPro;
using UnityEngine;

/// <summary>
/// Handles displaying and updating the UI for a specific <see cref="PlayerTask"/>.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TaskDisplay : MonoBehaviour
{
    /// <summary>
    /// Get the task that's associated with this display.
    /// </summary>
    public PlayerTask Task { get; private set; }

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Attempts to associate this display with a <see cref="PlayerTask"/> and initialize its display text.
    /// <para>
    /// If a task has already been assigned, this method does nothing and returns false.
    /// </para>
    /// </summary>
    /// <param name="task">The <see cref="PlayerTask"/> to associate with this display.</param>
    /// <returns>True if the task was successfully assigned, false if a task was already assigned.</returns>
    public bool TrySetTask(PlayerTask task)
    {
        if (Task != null)
        {
            return false;
        }

        Task = task;
        Task.OnCompleted += OnTaskCompleted;
        text.text = Task.RequirementMessage;
        return true;
    }

    private void OnDestroy()
    {
        if (Task != null)
        {
            Task.OnCompleted -= OnTaskCompleted;
        }
    }

    private void OnTaskCompleted(PlayerTask task)
    {
        MarkComplete();
    }

    /// <summary>
    /// Sets whether or not this task display is active.
    /// </summary>
    /// <param name="isActive">Whether the task display should be active.</param>
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Marks the displayed task as completed.
    /// </summary>
    public void MarkComplete()
    {
        text.color = Color.red;
        text.fontStyle = FontStyles.Strikethrough;
    }
}
