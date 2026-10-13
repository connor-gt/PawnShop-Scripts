using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Utility class that contains methods for managing timing of methods and MonoBehaviours.
/// </summary>
public static class TimingUtils
{
    /// <summary>
    /// Invokes a method or lambda expression after a delay.
    /// </summary>
    public static void Invoke(this MonoBehaviour mb, Action function, float delay)
    {
        if (delay < 0f)
        {
            delay = 0f;
        }

        mb.StartCoroutine(InvokeRoutine(function, delay));
    }

    private static IEnumerator InvokeRoutine(Action f, float d)
    {
        yield return new WaitForSecondsRealtime(d);
        f?.Invoke();
    }

    /// <summary>
    /// Invokes a method or lambda expression after a certain number of frames.
    /// </summary>
    public static void InvokeAfterFrames(this MonoBehaviour mb, Action function, int frames)
    {
        if (frames < 1)
        {
            frames = 1;
        }
        
        mb.StartCoroutine(InvokeAfterFramesRoutine(function, frames));
    }

    private static IEnumerator InvokeAfterFramesRoutine(Action f, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        f?.Invoke();
    }
}
