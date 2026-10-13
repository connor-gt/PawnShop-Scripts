using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Testing brain that's used to have NPCs walk between a set of points
/// </summary>
[CreateAssetMenu(fileName = "NewMoveBetweenPointsBrain", menuName = "Scriptable Objects/Brains/Move Between Points Brain")]
public class MoveBetweenPointsBrainSO : BrainSO
{
    [field: Title("Idle Settings")]
    [field: Tooltip("This NPC will wait between this amount of seconds between points.")]
    [field: SerializeField]
    public MinMaxParameters IdleTimes { get; private set; }

    public override Brain CreateBrain(Entity entity)
    {
        return new MoveBetweenPointsBrain(entity, this);
    }
}
