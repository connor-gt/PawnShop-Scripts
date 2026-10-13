using System;
using UnityEngine;

/// <summary>
/// Contains helper methods related to <see cref="ItemCondition"/> values.
/// </summary>
public static class ItemConditionUtils
{
    // Lazy initialization since this is a static class without a reliable way to initialize it with Unity's
    // lifecycle events
    private static ItemCondition[] _conditions = null;

    /// <summary>
    /// Gets an array of all <see cref="ItemCondition"/>s defined in the enum.
    /// </summary>
    public static ItemCondition[] Conditions
    {
        get
        {
            if (_conditions == null)
            {
                _conditions = (ItemCondition[])Enum.GetValues(typeof(ItemCondition));
            }

            return _conditions;
        }
    }

    /// <summary>
    /// Chooses a random <see cref="ItemCondition"/> using the weights defined in the specified configuration.
    /// </summary>
    /// <param name="config">The configuration containing the weights for each item condition.</param>
    /// <returns>A randomly selected item condition.</returns>
    public static ItemCondition GetRandomCondition(ItemConditionConfigSO config)
    {
        return MathUtils.ChooseWeightedRandom(
            Conditions,
            condition => 
            {
                ItemConditionData data = config.GetData(condition);
                return data != null ? data.Weight : 0;
            }
        );
    }

    /// <summary>
    /// Gets the value multiplier associated with the specified <see cref="ItemCondition"/>.
    /// </summary>
    /// <param name="condition">The condition whose multiplier should be retrieved.</param>
    /// <param name="config">The configuration containing multiplier data.</param>
    /// <returns>The multiplier associated with the condition, or <c>1.0f</c> if no data exists.</returns>
    public static float GetMultiplier(this ItemCondition condition, ItemConditionConfigSO config)
    {
        ItemConditionData data = config.GetData(condition);
        return data != null ? data.Multiplier : 1f;
    }

    /// <summary>
    /// Gets a condition's flavor text.
    /// </summary>
    /// <param name="condition">The condition to get the flavor text for.</param>
    /// <returns>A string flavor text (e.g., "Looks brand new." or "Shows normal wear."). If the condition does
    /// not happen to have a flavor text assigned, "No flavor text provided." is returned.</returns>
    public static string GetFlavorText(this ItemCondition condition)
    {
        switch (condition)
        {
            case ItemCondition.Pristine:
                return "Looks brand new.";
            case ItemCondition.Excellent:
                return "Very few signs of use.";
            case ItemCondition.Good:
                return "Shows normal wear.";
            case ItemCondition.Worn:
                return "Noticeably used.";
            case ItemCondition.Damaged:
                return"Functional, but flawed.";
            case ItemCondition.Broken:
                return "Barely works, if at all.";
            default:
                return "No flavor text provided.";
        }
    }
}
