using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGenericShopperBrain", menuName = "Scriptable Objects/Brains/Generic Shopper Brain")]
public class GenericShopperBrainSO : BrainSO
{
    [field: Title("Idle Settings")]
    [field: SerializeField]
    public MinMaxParameters RandomIdleTime { get; private set; }

    [field: SerializeField]
    [field: Min(0f)]
    public float FixedIdleTime { get; private set; } = 1f;

    [field: Title("Wander Settings")]
    [field: SerializeField]
    [field: Min(0f)]
    public float WanderRange { get; private set; } = 8f;

    [field: Title("Rotate Settings")]
    [field: Tooltip("The speed at which the entity rotates towards potential targets (e.g., while idling or "
        + "waiting in a checkout line whilst facing the counter).")]
    [field: SerializeField]
    [field: Min(0f)]
    public float RotateSpeed { get; private set; } = 10f;

    [field: Tooltip("How long the entity will stand there after finishing rotating towards a target (e.g., how "
        + "long with entity will stand in front of an item before deciding to grab it).")]
    [field: SerializeField]
    [field: Min(0f)]
    public float DelayTimeAfterRotating { get; private set; } = 2f;

    [field: Title("Generic Dialogue")]
    [field: SerializeField]
    public DialogueSO GenericDialogue { get; private set; }

    public override Brain CreateBrain(Entity entity)
    {
        if (!entity.TryGetComponent(out BuyerProfileModule buyerProfileModule))
        {
            throw new ArgumentException($"{name} expects {entity.name} to have a buyer profile module!");
        }

        return new GenericShopperBrain(
            entity,
            this,
            buyerProfileModule,
            PawnShop.Instance.ShelfController,
            PawnShop.Instance.EntityController,
            PawnShop.Instance.PurchaseCounter
        );
    }
}
