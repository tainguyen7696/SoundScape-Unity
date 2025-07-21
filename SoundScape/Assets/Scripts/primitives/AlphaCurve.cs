using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides time-normalized easing curves for use inside coroutines.
/// Usage example:
/// IEnumerator AnimateHeight(RectTransform rt, float targetHeight, float duration)
/// {
///     float startHeight = rt.sizeDelta.y;
///     foreach (float t in AlphaCurve.Linear(duration))
///     {
///         float newHeight = Mathf.Lerp(startHeight, targetHeight, t);
///         rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
///         yield return null;
///     }
/// }
/// </summary>
public static class AlphaCurve
{
    /// <summary>
    /// Linear progression from 0 to 1 over the given duration (in seconds).
    /// </summary>
    public static IEnumerable<float> Linear(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            yield return Mathf.Clamp01(elapsed / duration);
            elapsed += Time.deltaTime;
        }
        yield return 1f;
    }

    /// <summary>
    /// Quadratic ease-in: slow start, then accelerate.
    /// </summary>
    public static IEnumerable<float> EaseInQuad(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            yield return t * t;
            elapsed += Time.deltaTime;
        }
        yield return 1f;
    }

    /// <summary>
    /// Quadratic ease-out: fast start, then decelerate.
    /// </summary>
    public static IEnumerable<float> EaseOutQuad(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            yield return t * (2f - t);
            elapsed += Time.deltaTime;
        }
        yield return 1f;
    }

    /// <summary>
    /// Quadratic ease-in/out: slow start, fast middle, slow end.
    /// </summary>
    public static IEnumerable<float> EaseInOutQuad(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            if (t < 0.5f)
                yield return 2f * t * t;
            else
                yield return -1f + (4f - 2f * t) * t;
            elapsed += Time.deltaTime;
        }
        yield return 1f;
    }
}
