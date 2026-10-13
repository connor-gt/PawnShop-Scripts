using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class GenericSellerBrainSO : BrainSO
{
    [field: Title("Idle Settings")]
    [field: SerializeField]
    public MinMaxParameters RandomIdleTime { get; private set; }

    [field: Title("Rotate Settings")]
    [field: SerializeField]
    public float RotateSpeed { get; private set; }

    [field: Title("Negotiation Specific Dialogue")]
    [field: SerializeField]
    public NegotiationDialogue NegotiationDialogue { get; private set; }

    public override Brain CreateBrain(Entity entity)
    {
        if (!entity.TryGetComponent(out SellerProfileModule sellerProfileModule))
        {
            throw new ArgumentException($"{name} expects {entity.name} to have a buyer profile module!");
        }

        return new GenericSellerBrain(
            entity,
            this,
            sellerProfileModule,
            PawnShop.Instance.SellCounter,
            PawnShop.Instance.ShelfController,
            PawnShop.Instance.EntityController
        );
    }
}
