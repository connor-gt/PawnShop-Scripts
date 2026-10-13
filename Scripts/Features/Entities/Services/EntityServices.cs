using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides utility methods for <see cref="Entity"/>s.
/// </summary>
public static class EntityServices
{
    private static readonly HashSet<int> navmeshPriorities = new();

    /// <summary>
    /// Gets a randomly selected avoidance priority that is not currently registered, then registers it as being
    /// in use.
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound of the range.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the range.</param>
    /// <returns>A randomly selected, currently unregistered avoidance priority.</returns>
    /// <exception cref="ArgumentException">Thrown if all priorities within the range are already registered.
    /// </exception>
    public static int GetUniqueAvoidancePriority(int minInclusive, int maxExclusive)
    {
        int uniquePriority = MathUtils.GetRandomRangeWithExclusions(minInclusive, maxExclusive, navmeshPriorities);
        RegisterAvoidancePriority(uniquePriority);
        return uniquePriority;
    }

    /// <summary>
    /// Registers an avoidance priority as currently being in use.
    /// </summary>
    /// <param name="priority">The avoidance priority to register.</param>
    /// <returns>True if the priority is not already registered and was successfully registered, false if otherwise.
    /// </returns>
    public static bool RegisterAvoidancePriority(int priority)
    {
        if (navmeshPriorities.Contains(priority))
        {
            return false;
        }

        navmeshPriorities.Add(priority);
        return true;
    }

    /// <summary>
    /// Releases a previously registered avoidance priority, allowing it to be used again.
    /// </summary>
    /// <param name="priority">The avoidance priority to release.</param>
    /// <returns>True if the priority was registered and released, false if the priority was not currently
    /// registered at all.</returns>
    public static bool ReleaseAvoidancePriority(int priority)
    {
        if (!navmeshPriorities.Contains(priority))
        {
            return false;
        }

        navmeshPriorities.Remove(priority);
        return true;
    }
}
