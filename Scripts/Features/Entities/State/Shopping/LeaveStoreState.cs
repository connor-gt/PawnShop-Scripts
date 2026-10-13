using UnityEngine;

/// <summary>
/// Once entities are ready to leave the pawn shop, they can enter this state.
/// <para>
/// This state is destructive in that it calls upon <see cref="EntityController.DestroyEntity"/> once the entity
/// reaches a despawn location.
/// </para>
/// </summary>
public class LeaveStoreState : State
{
    private readonly EntityController entityController;
    private readonly NavigationModule navigationModule;

    public LeaveStoreState(Entity entity, EntityController entityController) : base(entity)
    {
        this.entityController = entityController;
        navigationModule = entity.GetComponent<NavigationModule>();
    }

    public override void Enter()
    {
        base.Enter();
        Transform despawnLocation = entityController.DespawnLocation;
        navigationModule.SetDestination(despawnLocation);
    }

    public override void Update()
    {
        if (navigationModule.HasReachedDestination())
        {
            entityController.DestroyEntity(entity);
        }
    }
}
