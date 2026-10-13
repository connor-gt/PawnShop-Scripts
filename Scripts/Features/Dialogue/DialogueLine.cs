using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Represents a single line of dialogue in the game.
/// </summary>
[Serializable]
public class DialogueLine
{
    /// <summary>
    /// Gets the text of the dialogue line.
    /// </summary>
    [field: TextArea(3, 6)]
    [field: SerializeField]
    public string Line { get; private set; }

    /// <summary>
    /// Gets the player's available response after this dialogue line, including the available choices and
    /// what dialogue line each choice continues to.
    /// </summary>
    [field: InlineProperty]
    [field: HideLabel]
    [field: SerializeField]
    public DialogueResponse Response { get; private set; }
}
