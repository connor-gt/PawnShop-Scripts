using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIntroductionNPCBrain", menuName = "Scriptable Objects/Brains/Introduction NPC Brain")]
public class IntroductionNPCBrainSO : BrainSO
{
    [field: Title("Counter Settings")]
    [field: SerializeField]
    [field: Min(0f)]
    public float RotateSpeed { get; private set; } = 10f;

    [field: Tooltip("How long to wait before trying the counter again if its queue is full.")]
    [field: SuffixLabel("seconds")]
    [field: SerializeField]
    [field: Min(0.1f)]
    public float RetryDelay { get; private set; } = 2f;

    [field: Title("Dialogue")]
    [field: Required]
    [field: SerializeField]
    public DialogueSO IntroductionDialogue { get; private set; }

    public override Brain CreateBrain(Entity entity)
    {
        if (IntroductionDialogue == null)
        {
            throw new ArgumentException($"{name} needs an introduction dialogue!");
        }

        return new IntroductionNPCBrain(
            entity,
            this,
            PawnShop.Instance.SellCounter,
            PawnShop.Instance.EntityController
        );
    }
}
