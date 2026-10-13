using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Manages customer spawning and the transition of customers into and out of the active pawn shop day sequence.
/// </summary>
public class CustomerHandler : MonoBehaviour
{
    [Title("Entity Spawn Data")]
    [Tooltip("This can be left blank to default what's being used in the EntityController.")]
    [SerializeField]
    private EntitySpawnDataSO spawnData;

    [Title("Entity Spawn Data")]
    [Tooltip("This can be left blank to default what's being used in the EntityController.")]
    [SerializeField]
    private EntitySpawnBehaviorSO spawnBehavior;

    private IEnumerator Start()
    {
        while (PawnShop.Instance == null)
        {
            yield return null;
        }

        if (spawnData != null)
        {
            PawnShop.Instance.EntityController.SetSpawnData(spawnData);
        }

        if (spawnBehavior != null)
        {
            PawnShop.Instance.EntityController.SetSpawnBehavior(spawnBehavior);
        }
    }

    /// <summary>
    /// Allows the pawn shop to begin accepting customers by enabling spawning.
    /// </summary>
    public void SpawnCustomers(bool spawnCustomers)
    {
        PawnShop.Instance.EntityController.EnableSpawning(spawnCustomers);
    }

    /// <summary>
    /// Stops spawning new customers and instructs existing customers whose brains use <see cref="IExitRequester"/>
    /// to finish what they are doing and leave the pawn shop.
    /// </summary>
    public void EndCustomerService()
    {
        SpawnCustomers(false);
        PawnShop.Instance.EntityController.WrapUpAndLeave();
    }
}
