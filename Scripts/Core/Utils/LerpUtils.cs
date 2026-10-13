using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Utility class that contains methods for lerping.
/// </summary>
public static class LerpUtils
{
    /// <summary>
    /// Interpolates a float value over time and invokes a callback each frame with the updated value.
    /// </summary>
    /// <param name="from">The starting float value.</param>
    /// <param name="to">The target float value.</param>
    /// <param name="duration">The total duration of the lerp in seconds.</param>
    /// <param name="onValueChanged">A callback invoked every frame with the newly interpolated value. This is
    /// where the caller applies the value to a component (e.g. CanvasGroup.alpha).</param>
    public static IEnumerator LerpFloat(float from, float to, float duration, Action<float> onValueChanged,
        bool useUnscaledTime = false)
    {
        float time = 0f;

        while (time < duration)
        {
            time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float lerp = Mathf.Lerp(from, to, time / duration);

            onValueChanged(lerp);
            yield return null;
        }

        onValueChanged(to);
    }
    
    /// <summary>
    /// Interpolates a color over time and invokes a callback each frame with the updated color.
    /// </summary>
    /// <param name="from">The starting color.</param>
    /// <param name="to">The target color.</param>
    /// <param name="duration">The total duration of the lerp in seconds.</param>
    /// <param name="onColorChanged">A callback invoked every frame with the newly interpolated color. This is
    /// where the caller applies the value to a component (e.g. Image.color).</param>
    public static IEnumerator LerpColor(Color from, Color to, float duration, Action<Color> onColorChanged, 
        bool useUnscaledTime = false)
    {
        float time = 0f;

        while (time < duration)
        {
            time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            Color lerp = Color.Lerp(from, to, time / duration);

            onColorChanged(lerp);
            yield return null;
        }

        onColorChanged(to);
    }

    /// <summary>
    /// Interpolates a quaternion rotation over time and invokes a callback each frame with the updated rotation.
    /// </summary>
    /// <param name="from">The starting rotation.</param>
    /// <param name="to">The target rotation.</param>
    /// <param name="duration">The total duration of the lerp in seconds.</param>
    /// <param name="onRotationChanged">A callback invoked every frame with the newly interpolated rotation. This
    /// is where the caller applies the value to a component (e.g. transform.rotation).</param>
    public static IEnumerator LerpRotation(Quaternion from, Quaternion to, float duration,
        Action<Quaternion> onRotationChanged, bool useUnscaledTime = false)
    {
        float time = 0f;

        while (time < duration)
        {
            time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            Quaternion lerp = Quaternion.Lerp(from, to, time / duration);

            onRotationChanged(lerp);
            yield return null;
        }

        onRotationChanged(to);
    }

    /// <summary>
    /// Interpolate a position over time and invokes a callback each frame with the updated position.
    /// </summary>
    /// <param name="from">The starting position.</param>
    /// <param name="to">The target position.</param>
    /// <param name="duration">The total duration of the lerp in seconds.</param>
    /// <param name="onPositionChanged">A callback invoked every frame with the newly interpolated position. This
    /// is where the caller applies the value to a component (e.g. transform.position).</param>
    /// <param name="useUnscaledTime">Whether to use unscaled time for the lerp.</param>
    public static IEnumerator LerpPosition(Vector3 from, Vector3 to, float duration,
        Action<Vector3> onPositionChanged, bool useUnscaledTime = false)
    {
        float time = 0f;

        while (time < duration)
        {
            time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            Vector3 lerp = Vector3.Lerp(from, to, time / duration);

            onPositionChanged(lerp);
            yield return null;
        }

        onPositionChanged(to);
    }
}
