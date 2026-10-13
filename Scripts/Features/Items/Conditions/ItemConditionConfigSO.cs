using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Asset that is used to determine how conditions can be randomly assigned to generated items.
/// <para>
/// See <see cref="ItemConditionData"/>.
/// </para>
/// </summary>
[CreateAssetMenu(fileName = "NewItemConditionConfig", menuName = "Scriptable Objects/Item Data/New Item Condition Config")]
public class ItemConditionConfigSO : ScriptableObject
{
    /// <summary>
    /// Gets the configuration data for each supported item condition.
    /// </summary>
    [field: Tooltip("Controls how likely each condition is to appear and how much it affects an item's estimated value")]
    [field: SerializeField]
    public List<ItemConditionData> Conditions { get; private set; }

    /// <summary>
    /// Retrieves the configuration data associated with the specified <see cref="ItemCondition"/>.
    /// </summary>
    /// <param name="condition">The condition to retrieve data for.</param>
    /// <returns>The corresponding condition data, or null if no matching entry exists.</returns>
    public ItemConditionData GetData(ItemCondition condition)
    {
        foreach (var conditionData in Conditions)
        {
            if (conditionData.Condition == condition)
            {
                return conditionData;
            }
        }

        return null;
    }
}
