#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that can be added to a script that has a list of <see cref="InteractionMapping"/> to show the
/// connections between an <see cref="InteractionPoint"/> and its associated interactions.
/// </summary>
// NOTE: This is a pure C# class instead of a MonoBehaviour because it's likely any script I make with a list
// of interaction mappings will have the list be private. I think this is the least non-intrusive way to debug
public class InteractionMappingDebugger
{
    /// <summary>
    /// Gets or sets the color of the line that's drawn between <see cref="InteractionPoint"/>s and their
    /// interaction game objects.
    /// </summary>
    public Color LineColor { get; set; } = Color.yellow;

    /// <summary>
    /// Draws debug lines between each <see cref="InteractionPoint"/> and its associated interaction objects.
    /// </summary>
    /// <typeparam name="T">The type of interaction object contained by each mapping.</typeparam>
    /// <param name="interactionMappings">The interaction mappings whose connections should be displayed.</param>
    public void ShowConnections<T>(IReadOnlyList<InteractionMapping<T>> interactionMappings) where T : MonoBehaviour
    {
        if (interactionMappings == null)
        {
            return;
        }

        foreach (var mapping in interactionMappings)
        {
            foreach (var interactionMB in mapping.Interactions)
            {
                if (mapping.Point == null || interactionMB == null)
                {
                    continue;
                }

                Gizmos.color = LineColor;
                Gizmos.DrawLine(mapping.Point.transform.position, interactionMB.transform.position);
            }
        }
    }
}
#endif
