using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Represents the player's available responses after a specific dialogue line, including the available
/// choices and what dialogue line each choice continues to.
/// </summary>
[Serializable]
public class DialogueResponse
{
    /// <summary>
    /// Gets the player's choices they can make in response and what dialogue indexes those choices lead to
    /// next.
    /// </summary>
    [field: Tooltip("The player's choices they can make in response and what dialogue indexes those choices"
        + " lead to next.")]
    [field: ListDrawerSettings(ShowFoldout = false, ShowPaging = false)]
    [field: SerializeField]
    public DialogueChoiceLink[] ChoiceLinks { get; set; }
}
