/*
    This script was found from this Unity discussion:
    https://discussions.unity.com/t/fps-counter-683389/683389/2

    (I added the documentation to remember what everything does)
*/

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSTracker : MonoBehaviour
{
    [Tooltip("The TextMeshPro component that displays the current FPS.")]
    [SerializeField]
    private TextMeshProUGUI text;

    /// <summary>
    /// This avoids allocating new strings every frame when updating the FPS text by caching the string
    /// representation of numbers up to <see cref="cachedNumberAmounts"/>
    /// </summary>
    private readonly Dictionary<int, string> cachedNumberStrings = new();

    /// <summary>
    /// Circular buffer storing the most recent frame rate samples.
    /// <para>
    /// Each entry is one frame's calculated FPS value, and the array is averaged to produce a smoother, less
    /// jittery FPS readout.
    /// </para>
    /// </summary>
    private int[] frameRateSamples;

    /// <summary>
    /// The maximum number value to pre‑cache as strings.
    /// <para>
    /// e.g., if this is set to 1500 and the game's FPS goes above this amount, the FPS will be displayed as
    /// "> 1500".
    /// </para>
    /// </summary>
    private readonly int cachedNumberAmounts = 1500;

    /// <summary>
    /// How many frame samples to include when calculating the averaged FPS. Higher values produce smoother
    /// results but respond more slowly to changes.
    /// </summary>
    private readonly int averageFromAmount = 150;

    /// <summary>
    /// Current index into the <see cref="frameRateSamples"/> circular buffer. Moves forward each frame and
    /// wraps around using modulo arithmetic.
    /// </summary>
    private int averageCounter = 0;

    /// <summary>
    /// The most recently computed averaged FPS value.
    /// <para>
    /// This is the number that is written to <see cref="text"/> in the UI after smoothing.
    /// </para>
    /// </summary>
    private int currentAveraged;

    private void Awake()
    {
        // Cache strings and create array
        for (int i = 0; i < cachedNumberAmounts; i++)
        {
            cachedNumberStrings[i] = i.ToString();
        }

        frameRateSamples = new int[averageFromAmount];
    }

    private void Update()
    {
        // Sample
        var currentFrame = (int)Mathf.Round(1f / Time.unscaledDeltaTime);
        frameRateSamples[averageCounter] = currentFrame;

        // Average
        var average = 0f;

        foreach (var frameRate in frameRateSamples)
        {
            average += frameRate;
        }

        currentAveraged = (int)Mathf.Round(average / averageFromAmount);
        averageCounter = (averageCounter + 1) % averageFromAmount;

        // Assign to UI
        text.text = currentAveraged switch
        {
            var x when x >= 0 && x < cachedNumberAmounts => cachedNumberStrings[x],
            var x when x >= cachedNumberAmounts => $"> {cachedNumberAmounts}",
            var x when x < 0 => "< 0",
            _ => "?"
        };
    }
}
