/// <summary>
/// Defines the phases of a pawn shop chapter (a full day).
/// </summary>
public enum ChapterPhase
{
    /// <summary>
    /// The pawn shop is preparing to open.
    /// <para>
    /// The player may perform opening tasks and prepare the shop before opening it to customers.
    /// </para>
    /// </summary>
    Opening,

    /// <summary>
    /// The pawn shop is open to the general public and customers are active.
    /// </summary>
    Day,

    /// <summary>
    /// The pawn shop is no longer accepting customers and the player may perform closing tasks.
    /// </summary>
    Closing,

    /// <summary>
    /// The day's closing sequence has ended and the night sequence is active.
    /// </summary>
    Night
}
