using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Determines how a <see cref="TriggerDetector"/> responds to matching trigger entries.
/// </summary>
public enum TriggerMode
{
    /// <summary>
    /// Allows the trigger to respond every time a matching object enters.
    /// </summary>
    Repeatable,

    /// <summary>
    /// Allows the trigger to respond only the first time a matching object enters.
    /// <para>
    /// Only <see cref="OnEntered"/> is triggered, <see cref="OnExited"/> is not.
    /// </para>
    /// </summary>
    Once
}

[RequireComponent(typeof(Collider))]
public class TriggerDetector : MonoBehaviour
{
    /// <summary>
    /// Occurs when a matching collider enters the trigger.
    /// </summary>
    public event Action<Collider> OnEntered;

    /// <summary>
    /// Occurs when a matching collider exits the trigger.
    /// </summary>
    public event Action<Collider> OnExited;

    [Title("Trigger Setting")]
    [Tooltip("Determines whether the trigger responds every time a matching collider enters or only the first time.")]
    [SerializeField]
    private TriggerMode mode = TriggerMode.Repeatable;

    private ITriggerFilter[] filters; // The filters act like an AND composition
    private Collider col;

    /// <summary>
    /// Gets how many times this detector has triggered.
    /// </summary>
    public int TimesTriggered { get; private set; }

#if UNITY_EDITOR
    [Title("Debug")]
    [SerializeField]
    private Logger logger;
    
    private bool ShowLogs => logger != null && logger.ShowLogs;
#endif

    private void Awake()
    {
        filters = GetComponents<ITriggerFilter>();

        col = GetComponent<Collider>();
        col.isTrigger = true;

        TimesTriggered = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ShouldTrigger(other))
        {
            return;
        }

        TimesTriggered++;
        OnEntered?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ShouldTrigger(other))
        {
            return;
        }

        OnExited?.Invoke(other);
    }

    /// <summary>
    /// Determines whether the specified collider matches all filters attached to this trigger.
    /// </summary>
    /// <param name="other">The collider to evaluate.</param>
    /// <returns>True if the collider passes all of the filters, false if otherwise. False is also returned if
    /// the trigger mode is set to <see cref="TriggerMode.Once"/> and this has already been triggered.</returns>
    private bool ShouldTrigger(Collider other)
    {
        if (mode == TriggerMode.Once && TimesTriggered > 0)
        {
            return false;
        }

        for (int i = 0; i < filters.Length; i++)
        {
            if (!filters[i].Matches(other))
            {
#if UNITY_EDITOR
                if (ShowLogs) logger.Log($"{other.name} doesn't match filter {filters[i].GetType().Name}", this);
#endif
                return false;
            }
        }

#if UNITY_EDITOR
                if (ShowLogs) logger.Log($"{other.name} successfully triggered {gameObject.name}", this);
#endif
        return true;
    }
}
