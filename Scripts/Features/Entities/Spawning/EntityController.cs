using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [field: Title("Spawn Settings")]
    [Tooltip("Defines the entities that can be spawned.")]
    [SerializeField]
    private EntitySpawnDataSO spawnData;

    [Tooltip("Defines how and when entity spawning should occur.")]
    [SerializeField]
    private EntitySpawnBehaviorSO spawnBehavior;

    /// <summary>
    /// Gets or sets whether or not this controller is currently allowed to spawn entities.
    /// </summary>
    public bool CanSpawn { get; private set; }

    /// <summary>
    /// Gets the transform location where entities are spawned.
    /// </summary>
    [field: Title("Entity Spawn Location")]
    [field: SerializeField]
    public Transform SpawnLocation { get; private set; }

    /// <summary>
    /// Gets the transform location where entities should go to despawn (be destroyed).
    /// </summary>
    [field: Title("Entity Despawn Location")]
    [field: SerializeField]
    public Transform DespawnLocation { get; private set; }

    /// <summary>
    /// Gets the currently active/instantiated entities.
    /// </summary>
    private readonly List<Entity> activeEntities = new();

#if UNITY_EDITOR
    [Title("Inspector Override")]
    [Tooltip("If set to false, no spawns will happen at all")]
    [SerializeField]
    private bool enableSpawning = true;
#endif

    private void Start()
    {
#if UNITY_EDITOR
        if (!enableSpawning) return;
#endif
        if (spawnBehavior != null)
        {
            spawnBehavior.Initialize(this);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!enableSpawning) return;
#endif
        if (CanSpawn && spawnBehavior != null)
        {
            spawnBehavior.Evaluate(this);
        }
    }

    /// <summary>
    /// Enables or disables entity spawning.
    /// </summary>
    /// <param name="enableSpawning">Whether or not the controller should be allowed to spawn entities.</param>
    public void EnableSpawning(bool enableSpawning)
    {
        CanSpawn = enableSpawning;
    }
    
    /// <summary>
    /// Attempts to spawn an entity using the specified prefab and world position.
    /// <para>
    /// Spawning fails if the maximum number of active entities defined by the current spawn data has already
    /// been reached.
    /// </para>
    /// </summary>
    /// <param name="prefab">The entity prefab to instantiate.</param>
    /// <param name="location">The location of where the entity should be spawned.</param>
    /// <returns>True if the entity was successfully spawned, false if otherwise.</returns>
    public bool TrySpawnEntity(GameObject prefab, Vector3 location)
    {
        if (activeEntities.Count >= spawnData.MaxActiveEntities)
        {
            return false;
        }

        Entity entity = EntityFactory.SpawnEntity(prefab, location);
        activeEntities.Add(entity);
        return true;
    }

    /// <inheritdoc cref="TrySpawnEntity(GameObject, Vector3)"/>
    public bool TrySpawnEntity(GameObject prefab, Transform location)
    {
        return TrySpawnEntity(prefab, location.position);
    }

    /// <summary>
    /// Attempts to spawn a randomly selected entity.
    /// </summary>
    /// <returns>True if the entity was successfully spawned, false if otherwise.</returns>
    public bool TrySpawnRandomEntity()
    {
        GameObject randomPrefab = spawnData.GetRandomPrefab();
        return TrySpawnEntity(randomPrefab, SpawnLocation);
    }

    /// <summary>
    /// Removes the specified entity from the list of active entities and destroys its associated game object.
    /// </summary>
    /// <param name="entity">The entity to remove and destroy.</param>
    public void DestroyEntity(Entity entity)
    {
        if (activeEntities.Contains(entity))
        {
            activeEntities.Remove(entity);
        }

        Destroy(entity.gameObject);
    }

    /// <summary>
    /// Get the entities that are currently active.
    /// <para>
    /// <b>Note:</b> Since this is a referenced list and not a snapshot list, it is best to use this immediately
    /// on calling as copying and saving it for later may result stale data.
    /// </para>
    /// </summary>
    /// <returns>A readonly referenced list of the current active entities.</returns>
    public IReadOnlyList<Entity> GetEntities()
    {
        return activeEntities;
    }

    /// <summary>
    /// Requests that all active entities whose brains use <see cref="IExitRequester"/> finish what they are
    /// currently doing and leave the shop.
    /// </summary>
    public void WrapUpAndLeave()
    {
        for (int i = 0; i < activeEntities.Count; i++)
        {
            if (activeEntities[i].Brain is IExitRequester exitRequester)
            {
                exitRequester.WrapUpAndLeave();
            }
        }
    }

    /// <summary>
    /// Sets the spawn data used by this controller.
    /// </summary>
    /// <param name="spawnData">The data defining which entities can be spawned and their spawn limits.</param>
    public void SetSpawnData(EntitySpawnDataSO spawnData)
    {
        this.spawnData = spawnData;
    }

    /// <summary>
    /// Sets the behavior used to determine when entities should be spawned.
    /// <para>
    /// Passing in null removes spawn behavior entirely and stops entities from spawning.
    /// </para>
    /// </summary>
    /// <param name="spawnBehavior">The spawn behavior to evaluate while spawning is enabled.</param>
    public void SetSpawnBehavior(EntitySpawnBehaviorSO spawnBehavior)
    {
        this.spawnBehavior = spawnBehavior;

        if (this.spawnBehavior != null)
        {
            this.spawnBehavior.Initialize(this);
        }
    }
}
