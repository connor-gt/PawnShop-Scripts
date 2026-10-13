#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Editor only script that logs statements in the console. Any script that wants to debug log statements
/// should get a reference to the logger that is in the scene.
/// </summary>
public class Logger : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the toggle for this logger showing debug statements.
    /// </summary>
    [field: Title("Settings")]
    [field: SerializeField]
    public bool ShowLogs { get; set; } = true;

    /// <summary>
    /// Gets or sets the color of regular <see cref="Debug.Log"/> statements (does not affect
    /// <see cref="Debug.LogWarning"/> nor <see cref="Debug.LogError"/>).
    /// </summary>
    [field: Tooltip("The color of regular Debug.Log statements (does not change warning or error logs).")]
    [field: SerializeField]
    public Color StatementColor { get; set; } = Color.lightGray;
    
    /// <summary>
    /// Log a <see cref="Debug.Log"/> statement. The color can be changed via <see cref="StatementColor"/>.
    /// </summary>
    /// <param name="message">The message object.</param>
    /// <param name="sender">The sender object. Used for seeing what object logged the statement.</param>
    public void Log(object message, Object sender)
    {
        if (!ShowLogs)
        {
            return;
        }

        string hex = ColorUtility.ToHtmlStringRGBA(StatementColor);
        Debug.Log($"<color=#{hex}>{message}</color>", sender);
    }

    /// <summary>
    /// Log a <see cref="Debug.LogWarning"/> statement.
    /// </summary>
    /// <param name="message">The message object.</param>
    /// <param name="sender">The sender object. Used for seeing what object logged the warning.</param>
    public void LogWarning(object message, Object sender)
    {
        if (!ShowLogs)
        {
            return;
        }

        Debug.LogWarning(message, sender);
    }

    /// <summary>
    /// Log a <see cref="Debug.LogError"/> statement.
    /// </summary>
    /// <param name="message">The message object.</param>
    /// <param name="sender">The sender object. Used for seeing what object logged the error.</param>
    public void LogError(object message, Object sender)
    {
        if (!ShowLogs)
        {
            return;
        }

        Debug.LogError(message, sender);
    }
}
#endif
