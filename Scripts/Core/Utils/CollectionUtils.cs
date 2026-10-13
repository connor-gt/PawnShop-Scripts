using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Utility class for collection-related operations.
/// </summary>
public static class CollectionUtils
{
    private static readonly Random rng = new();

    /// <summary>
    /// Shuffles a list in place using the Fisher-Yates shuffle algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to shuffle.</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Get a random value from the enum type.
    /// </summary>
    /// <typeparam name="T">The enum type to get a random value from.</typeparam>
    /// <returns>A randomly selected enum value.</returns>
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[rng.Next(values.Length)];
    }
}
