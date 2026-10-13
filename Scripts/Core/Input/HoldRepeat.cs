using UnityEngine;

/// <summary>
/// Can be used to repeatadly trigger actions after certain delays while an input is being held.
/// </summary>
public class HoldRepeat
{
    /// <summary>
    /// The amount of time, in seconds, that the input must be held before the first repeat occurs.
    /// </summary>
    private readonly float initialDelay;

    /// <summary>
    /// The amount of time, in seconds, between subsequent repeats after the initial delay has elapsed.
    /// </summary>
    private readonly float repeatDelay;

    /// <summary>
    /// The next time at which <see cref="ShouldTick(bool)"/> should return true.
    /// </summary>
    private float nextRepeatTime;

    /// <summary>
    /// Tracks whether the input was pressed during the previous tick.
    /// </summary>
    private bool wasPressed;

    /// <summary>
    /// Creates a new <see cref="HoldRepeat"/> instance.
    /// </summary>
    /// <param name="initialDelay">The amount of time, in seconds, before the first repeat occurs.</param>
    /// <param name="repeatDelay">The amount of time, in seconds, between subsequent repeats.</param>
    public HoldRepeat(float initialDelay, float repeatDelay)
    {
        this.initialDelay = initialDelay;
        this.repeatDelay = repeatDelay;
    }

    /// <summary>
    /// Determines whether the held input should trigger during the current frame.
    /// </summary>
    /// <param name="isPressed">Whether the input is currently being held.</param>
    /// <returns>True if the action should trigger this frame, false if otherwise.</returns>
    public bool ShouldTick(bool isPressed)
    {
        if (!isPressed)
        {
            wasPressed = false;
            return false;
        }

        if (!wasPressed)
        {
            wasPressed = true;
            nextRepeatTime = Time.time + initialDelay;

            return false;
        }

        if (Time.time >= nextRepeatTime)
        {
            nextRepeatTime = Time.time + repeatDelay;
            return true;
        }

        return false;
    }
}
