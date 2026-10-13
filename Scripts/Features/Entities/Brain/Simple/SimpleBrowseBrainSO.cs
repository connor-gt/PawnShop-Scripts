using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleBrowseBrain", menuName = "Scriptable Objects/Brains/Simple Browse Brain")]
public class SimpleBrowseBrainSO : BrainSO
{
    [field: Title("Idle Settings")]
    [field: SerializeField]
    public MinMaxParameters IdleTimes { get; private set; }

    [field: Tooltip("The speed at which the entity rotates towards potential targets while idling.")]
    [field: SerializeField]
    [field: Min(0f)]
    public float RotateSpeed { get; private set; } = 10f;

    public override Brain CreateBrain(Entity entity)
    {
        return new SimpleBrowseBrain(entity, this, PawnShop.Instance.ShelfController);
    }
}
