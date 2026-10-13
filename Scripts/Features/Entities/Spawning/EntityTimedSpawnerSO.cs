using UnityEngine;

[CreateAssetMenu(fileName = "NewTimedSpawner", menuName = "Scriptable Objects/Spawn/New Timed Spawner")]
public class EntityTimedSpawnerSO : EntitySpawnBehaviorSO
{
    [SerializeField]
    private float spawnTimer = 5f;

    private float timer;

    public override void Initialize(EntityController entityController)
    {
        timer = 0f;
    }

    public override void Evaluate(EntityController entityController)
    {
        timer += Time.deltaTime;
        if (timer >= spawnTimer)
        {
            entityController.TrySpawnRandomEntity();
            timer = 0f;
        }
    }
}
