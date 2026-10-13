using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A module that is responsible for handling navigation/movement of an NPC entity via <see cref="NavMeshAgent"/>.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavigationModule : MonoBehaviour
{
    /// <summary>
    /// Gets the NavMeshAgent component that this <see cref="NavigationModule"/> manages.
    /// </summary>
    public NavMeshAgent Agent { get; private set; }

    /// <summary>
    /// Get this agent's query filter, which is useful for managing multiple agent types on multiple navmesh
    /// surfaces.
    /// </summary>
    public NavMeshQueryFilter NavMeshQueryFilter { get; private set; }

    /// <summary>
    /// If the agent is within this distance to the destination it's moving towards, it's considered to have
    /// reached the end of the path (accounts for minor discrepancies in path finding/reaching destinations).
    /// </summary>
    private const float PATH_END_THRESHOLD = 0.1f;

    /// <summary>
    /// If the agent's velocity is below this threshold, it's considered to be stopped.
    /// </summary>
    private const float STOPPED_VELOCITY_THRESHOLD = 0.1f;

    private NavMeshPath navMeshPath;

    [Title("Avoidance Priority")]
    [Tooltip("If enabled, this entity will get a random and unique avoidance priority from EntityServices.")]
    [SerializeField]
    private bool randomlyAssignPriority = true;

    // The minimum begins at 20 so that 1-19 are reserved for scripted NPCs who should have the highest priorities
    private const int MIN_RANDOM_PRIORITY = 20; // Inclusive
    private const int MAX_RANDOM_PRIORITY = 101; // Exclusive

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        navMeshPath = new NavMeshPath();
        NavMeshQueryFilter = new NavMeshQueryFilter
        {
            agentTypeID = Agent.agentTypeID,
            areaMask = Agent.areaMask
        };
    }

    private void OnEnable()
    {
        // So NPCs don't get stuck on each other
        if (randomlyAssignPriority)
        {
            Agent.avoidancePriority = EntityServices.GetUniqueAvoidancePriority(
                MIN_RANDOM_PRIORITY,
                MAX_RANDOM_PRIORITY
            );
        }
        else
        {
            EntityServices.RegisterAvoidancePriority(Agent.avoidancePriority);
        }
    }

    private void OnDisable()
    {
        EntityServices.ReleaseAvoidancePriority(Agent.avoidancePriority);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        // Automatically add modules related to movement when this component is added to an object without
        // necessarily requiring these modules. Since I expect most entities with this module to also
        // have other related functions, these are added automatically and can be removed if needed
        gameObject.AddComponentsIfMissing(
            typeof(MovementAnimationModule),
            typeof(FootstepPlayerModule)
        );
    }
#endif

    public void SetDestination(Transform destination)
    {
        SetDestination(destination.position);
    }

    public void SetDestination(Vector3 destination)
    {
        Agent.SetDestination(destination);
    }

    public void TeleportTo(Transform destination)
    {
        TeleportTo(destination.position);
    }

    public void TeleportTo(Vector3 destination)
    {
        Agent.Warp(destination);
        SetDestination(destination); // Makes sure the agent's pathfinding is updated after teleporting
    }

    /// <summary>
    /// Get this <see cref="Agent"/>'s current destination.
    /// </summary>
    /// <returns>The agent's current destination in world space.</returns>
    public Vector3 GetDestination()
    {
        return Agent.destination;
    }

    /// <summary>
    /// Get whether or not the <see cref="NavMeshAgent"/> has reached its destination or is currently moving
    /// towards it.
    /// </summary>
    /// <returns>True if the agent has reached its destination and is stopped, false if otherwise.</returns>
    public bool HasReachedDestination()
    {
        bool closeEnough = Agent.remainingDistance <= Agent.stoppingDistance + PATH_END_THRESHOLD;
        bool isStopped = Agent.isStopped || Agent.velocity.sqrMagnitude < STOPPED_VELOCITY_THRESHOLD;

        return !Agent.pathPending && closeEnough && isStopped;
    }

    /// <summary>
    /// Checks if a path is valid for this entity to move from its current position to a specified position.
    /// </summary>
    /// <param name="toPosition">If moving to this position results in a valid path.</param>
    /// <returns>True if the calculated path to the new position is valid, false if otherwise.</returns>
    public bool CanReach(Vector3 toPosition)
    {
        return NavMesh.CalculatePath(transform.position, toPosition, NavMeshQueryFilter, navMeshPath);
    }

    /// <summary>
    /// Attempts to find a random valid position on the NavMesh within a given range around a specified center
    /// point.
    /// </summary>
    /// <param name="center">The world space position used as the center of the search area.</param>
    /// <param name="range">The radius of the search area in which a point will be sampled.</param>
    /// <param name="result">When the method returns true, this is a valid <see cref="NavMesh"/> position
    /// within the specified range. Otherwise, this will be <see cref="Vector3.zero"/>.</param>
    /// <returns>True if a valid <see cref="NavMesh"/> position was found within the range, false if otherwise
    /// (note that this method has a maximum allowed number of attempts, so even if it returns false that
    /// doesn't necessarily there are no valid points within the range).</returns>
    public bool GetRandomPointInRange(Vector3 center, float range, out Vector3 result)
    {
        const int MAX_ATTEMPTS = 40;

        // This is the recommended way to get a random point on a navmesh per the Unity API documentation
        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            Vector3 randomPoint = MathUtils.GetRandomPointInSphere(center, range);
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, Agent.height, NavMeshQueryFilter))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
