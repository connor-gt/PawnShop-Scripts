using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleWanderBrain", menuName = "Scriptable Objects/Brains/Simple Wander Brain")]
public class SimpleWanderBrainSO : BrainSO
{
    [field: Title("Idle Settings")]
    [field: SerializeField]
    public MinMaxParameters IdleTimes { get; private set; }

    [field: Title("Wander Settings")]
    [field: SerializeField]
    [field: Min(0f)]
    public float WanderRange { get; private set; } = 5f;

    public override Brain CreateBrain(Entity entity)
    {
        return new SimpleWanderBrain(entity, this);
    }
}
