using System;
using UnityEngine;

/// <summary>
/// Stores configuration data for a single <see cref="ItemCondition"/>.
/// </summary>
[Serializable]
public class ItemConditionData
{
    /// <summary>
    /// Gets or sets the condition represented by this entry.
    /// </summary>
    [field: Tooltip("The condition represented by this configuration entry.")]
    [field: SerializeField]
    public ItemCondition Condition { get; set; }

    /// <summary>
    /// Gets or sets the relative chance that this condition will be selected during weighted random generation.
    /// </summary>
    [field: Tooltip("Higher values increase the likelihood of this condition appearing.")]
    [field: SerializeField]
    public int Weight { get; set; }

    /// <summary>
    /// Gets or sets the multiplier applied to an item's intrinsic value when this condition is generated.
    /// </summary>
    [field: Tooltip("Determines how much this condition affects an item's estimated value.")]
    [field: SerializeField]
    public float Multiplier { get; set; }
}
