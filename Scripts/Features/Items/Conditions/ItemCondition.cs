/*
 * NOTE:
 *
 * Condition values are in descending order of quality so comparisons like: 
 * { ItemCondition.Pristine > ItemCondition.Good > ItemCondition.Broken } all remain true.
 * 
 * Values are also spaced apart to leave room for additional conditions to be added later if I want to do so.
 */

public enum ItemCondition
{
    /// <summary>
    /// Looks brand new.
    /// </summary>
    Pristine = 60,

    /// <summary>
    /// Very few signs of use.
    /// </summary>
    Excellent = 50,

    /// <summary>
    /// Shows normal wear.
    /// </summary>
    Good = 40,

    /// <summary>
    /// Noticeably used.
    /// </summary>
    Worn = 30,

    /// <summary>
    /// Functional, but flawed.
    /// </summary>
    Damaged = 20,

    /// <summary>
    /// Barely works, if at all.
    /// </summary>
    Broken = 10
}
