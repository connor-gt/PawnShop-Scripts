using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEntitySpawnData", menuName = "Scriptable Objects/Spawn/New Entity Spawn Data")]
public class EntitySpawnDataSO : ScriptableObject
{
    // TODO: maybe replace with a serializable class that has prefabs to spawn but also weights for weighted
    // random spawning?
    [field: Title("NPCs")]
    [field: RequiredListLength(1, null)]
    [field: SerializeField]
    public GameObject[] SpawnPool { get; private set; }

    /// <summary>
    /// Gets the maximum amount of entities that should be active during the game at the same time.
    /// <para>
    /// <b>Note:</b> This is not a hard limit but something the <see cref="EntityController"/> should check
    /// before spawning another <see cref="Entity"/>.
    /// </para>
    /// </summary>
    [field: Tooltip("The maximum amount of entities that can be active at the same time.")]
    [field: SerializeField]
    [field: Min(0)]
    public int MaxActiveEntities { get; private set; } = 14;

    public GameObject GetRandomPrefab()
    {
        GameObject random = SpawnPool[Random.Range(0, SpawnPool.Length)];
        return random;
    }
}
