using UnityEngine;
using System.Collections;

/// <summary>
/// Provides extension methods to fade CanvasGroup alpha over time.
/// Usage:
///   canvasGroup.FadeTo(1f, 0.5f, true, true);
/// </summary>
public static class CanvasGroupHelper
{
    /// <summary>
    /// Fades the CanvasGroup's alpha to the target value over duration seconds.
    /// Optionally sets interactable and blocksRaycasts at the end of the fade.
    /// </summary>
    /// <param name="canvasGroup">The CanvasGroup to fade.</param>
    /// <param name="targetAlpha">Target alpha (0 to 1).</param>
    /// <param name="duration">Duration in seconds.</param>
    /// <param name="interactableAfter">Whether canvasGroup.interactable = true at end.</param>
    /// <param name="blocksRaycastsAfter">Whether canvasGroup.blocksRaycasts = true at end.</param>
    public static void FadeTo(this CanvasGroup canvasGroup, float targetAlpha, float duration,
                              bool interactableAfter = true, bool blocksRaycastsAfter = true)
    {
        // Ensure there's only one fader per CanvasGroup
        var existing = canvasGroup.gameObject.GetComponent<CanvasGroupFadeBehaviour>();
        if (existing != null)
            existing.StopAllCoroutines();

        var fader = existing ?? canvasGroup.gameObject.AddComponent<CanvasGroupFadeBehaviour>();
        fader.Setup(canvasGroup, targetAlpha, duration, interactableAfter, blocksRaycastsAfter);
    }
}

/// <summary>
/// Internal MonoBehaviour that runs the fade coroutine and destroys itself when done.
/// </summary>
public class CanvasGroupFadeBehaviour : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private float _startAlpha;
    private float _targetAlpha;
    private float _duration;
    private bool _interactableAfter;
    private bool _blocksRaycastsAfter;

    /// <summary>
    /// Initialize or reconfigure the fader.
    /// </summary>
    public void Setup(CanvasGroup canvasGroup, float targetAlpha, float duration,
                      bool interactableAfter, bool blocksRaycastsAfter)
    {
        _canvasGroup = canvasGroup;
        _startAlpha = canvasGroup.alpha;
        _targetAlpha = Mathf.Clamp01(targetAlpha);
        _duration = Mathf.Max(duration, 0f);
        _interactableAfter = interactableAfter;
        _blocksRaycastsAfter = blocksRaycastsAfter;

        // Start (or restart) coroutine
        StopAllCoroutines();
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(_startAlpha, _targetAlpha, elapsed / _duration);
            yield return null;
        }

        // Ensure final alpha
        _canvasGroup.alpha = _targetAlpha;
        _canvasGroup.interactable = _interactableAfter;
        _canvasGroup.blocksRaycasts = _blocksRaycastsAfter;

        // Cleanup
        Destroy(this);
    }
}
