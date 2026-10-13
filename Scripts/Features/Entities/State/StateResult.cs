public enum StateResultType
{
    /// <summary>
    /// The state has completed successfully and is ready for a transition.
    /// </summary>
    Completed,

    /// <summary>
    /// The state was cancelled internally.
    /// </summary>
    InternalCancel,

    /// <summary>
    /// The state was cancelled due to some external factor.
    /// </summary>
    ExternalCancel,

    /// <summary>
    /// The state was interrupted and did not complete its behavior.
    /// </summary>
    Interrupted
}

public enum StateCancelReason
{
    /// <summary>
    /// No cancel reason was provided.
    /// </summary>
    None,

    /// <summary>
    /// The state timed out.
    /// </summary>
    Timeout,

    /// <summary>
    /// The state requires a target to operate on (e.g., a position to move to, an item to grab), but no
    /// target was provided.
    /// </summary>
    MissingTarget,

    /// <summary>
    /// The state was cancelled due to the target becoming/being unavailable.
    /// </summary>
    TargetUnavailable,

    /// <summary>
    /// The state was unable to find a path.
    /// </summary>
    PathNotFound,

    /// <summary>
    /// The state requires the entity to have a non-full inventory, but the entity's inventory is full.
    /// </summary>
    InventoryFull,
    
    /// <summary>
    /// The state cancelled for a reason that is not found within the existing <see cref="StateCancelReason"/>
    /// options.
    /// </summary>
    Other
}

/// <summary>
/// A container for the result of a state upon exiting.
/// </summary>
public readonly struct StateResult
{
    public StateResultType ResultType { get; }
    public StateCancelReason CancelReason { get; }

    /// <summary>
    /// Creates a new <see cref="StateResult"/> with the specified result type. The cancel reason defaults
    /// to <see cref="StateCancelReason.None"/>.
    /// </summary>
    /// <param name="resultType">The result type for the state.</param>
    public StateResult(StateResultType resultType) : this(resultType, StateCancelReason.None) { }

    /// <summary>
    /// Creates a new <see cref="StateResult"/> with the specified result type and cancel reason.
    /// </summary>
    /// <param name="resultType">The result type for the state.</param>
    /// <param name="cancelReason">The cancel reason for the state.</param>
    public StateResult(StateResultType resultType, StateCancelReason cancelReason)
    {
        ResultType = resultType;
        CancelReason = cancelReason;
    }

    public static StateResult Completed =>
        new(StateResultType.Completed);

    public static StateResult InternalCancel(StateCancelReason reason) =>
        new(StateResultType.InternalCancel, reason);

    public static StateResult Interrupted =>
        new(StateResultType.Interrupted);
}
