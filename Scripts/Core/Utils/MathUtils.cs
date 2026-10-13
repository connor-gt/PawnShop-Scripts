using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Utility class for common math related operations.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    /// <param name="value">The value to be remapped</param>
    /// <param name="fromMin">The old range's minimum value</param>
    /// <param name="fromMax">The old range's maximum value</param>
    /// <param name="toMin">the new range's minimum value</param>
    /// <param name="toMax">the new range's maximum value</param>
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // Prevents division by 0
        if (Mathf.Approximately(fromMax, fromMin))
        {
            return toMin;
        }

        float fromAbs = value - fromMin;
        float fromMaxAbs = fromMax - fromMin;

        float normal = fromAbs / fromMaxAbs;

        float toMaxAbs = toMax - toMin;
        float toAbs = toMaxAbs * normal;

        float to = toAbs + toMin;

        return to;
    }

    /// <summary>
    /// Chooses a random element from the specified collection using weighted probabilities.
    /// <para>
    /// Each element's chance of being selected is determined by the integer returned from
    /// <paramref name="weightSelector"/>. Larger weights result in a higher chance of being selected.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of element to select.</typeparam>
    /// <param name="items">The collection of elements to choose from.</param>
    /// <param name="weightSelector">A function that returns the weight associated with a given element.</param>
    /// <returns>A randomly selected element from the collection.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the total weight of all elements is less than or
    /// equal to zero.</exception>
    public static T ChooseWeightedRandom<T>(IEnumerable<T> items, Func<T, int> weightSelector)
    {
        int totalWeight = 0;
        foreach (var item in items)
        {
            totalWeight += weightSelector(item);
        }

        if (totalWeight <= 0)
        {
            throw new InvalidOperationException("Total weight must be greater than zero.");
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;

        // Walk through the collection until the randomly generated value falls within an element's weight range
        // See https://www.youtube.com/watch?v=3RM4jPtZPcQ for reference
        foreach (var item in items)
        {
            currentWeight += weightSelector(item);
            if (randomValue < currentWeight)
            {
                return item;
            }
        }

        throw new InvalidOperationException("No items available for selection.");
    }

    /// <summary>
    /// Calculates a random position within a triangle made of <see cref="Vector3"/> coordinates.
    /// </summary>
    /// <returns>A Vector3 position that is within the specified triangle's vertices.</returns>
    public static Vector3 GetRandomPointInTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Use Barycentric coordinates to get any point in a triangle: P = v0 + r1(v1 - v0) + r2(v2 - v0)
        float r1 = UnityEngine.Random.value;
        float r2 = UnityEngine.Random.value;

        // Make sure the point lies inside the triangle by ensuring r1 >= 0, r2 >= 0, r1 + r2 <= 1
        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        return v0 + r1 * (v1 - v0) + r2 * (v2 - v0);
    }

    /// <summary>
    /// Returns a random integer within the specified range, excluding the provided values.
    /// <para>
    /// This assumes that the integers in <see cref="exclusions"/> are within the specified range.
    /// </para>
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound of the range.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the range.</param>
    /// <param name="exclusions">The values to exclude from the range.</param>
    /// <returns>A randomly selected value within the range that is not excluded.</returns>
    /// <exception cref="ArgumentException">Thrown when all values in the range are excluded, leaving no valid
    /// values to select.</exception>
    public static int GetRandomRangeWithExclusions(int minInclusive, int maxExclusive, HashSet<int> exclusions)
    {
        int validCount = (maxExclusive - minInclusive) - exclusions.Count;
        if (validCount < 1)
        {
            throw new ArgumentException("No valid numbers left in the range after exclusions.");
        }

        // An upper bound is used to collapse the gaps left by the excluded numbers
        int upperBound = minInclusive + validCount;
        int random = UnityEngine.Random.Range(minInclusive, upperBound);

        // The loop maps the compressed range back to the full original range
        var sortedExclusions = exclusions.OrderBy(x => x);
        foreach (int exclusion in sortedExclusions)
        {
            // Shift the random int forward if it's greater than the current exclusion to stretch the range
            if (random >= exclusion)
            {
                random++;
            }
            // Since the exclusions are sorted, if the random int is smaller, no further stretching is necessary
            else
            {
                break;
            }
        }

        return random;
    }

    /// <summary>
    /// Returns a random point inside a sphere of a given radius centered at the specified position.
    /// </summary>
    /// <param name="center">The center of the sphere.</param>
    /// <param name="range">The radius of the sphere.</param>
    /// <returns>A random world space position within the sphere.</returns>
    public static Vector3 GetRandomPointInSphere(Vector3 center, float range)
    {
        return center + UnityEngine.Random.insideUnitSphere * range;
    }
}
