using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleSellItemsBrain", menuName = "Scriptable Objects/Brains/Simple Sell Items Brain")]
public class SimpleSellItemsBrainSO : BrainSO
{
    [field: Title("Idle Settings")]
    [field: SerializeField]
    public float IdleTime { get; private set; } = 1f;

    [field: Title("Wander Settings")]
    [field: SerializeField]
    [field: Min(0f)]
    public float WanderRange { get; private set; } = 5f;

    [field: Title("Rotate Settings")]
    [field: Tooltip("The speed at which the entity rotates towards potential targets (e.g., while idling or "
        + "waiting in a queue line whilst facing the counter).")]
    [field: SerializeField]
    [field: Min(0f)]
    public float RotateSpeed { get; private set; } = 10f;

    [field: Title("Generic Dialogue")]
    [field: SerializeField]
    public DialogueSO GenericDialogue { get; private set; }

    [field: Title("Negotiation Specific Dialogue")]
    [field: SerializeField]
    public NegotiationDialogue NegotiationDialogue { get; private set; }

    public override Brain CreateBrain(Entity entity)
    {
        if (!entity.TryGetComponent(out SellerProfileModule sellerProfileModule))
        {
            throw new ArgumentException($"{name} expects {entity.name} to have a seller profile module!");
        }

        return new SimpleSellItemsBrain(
            entity,
            this,
            sellerProfileModule,
            sellerProfileModule.Inventory,
            PawnShop.Instance.EntityController,
            PawnShop.Instance.SellCounter
        );
    }
}
