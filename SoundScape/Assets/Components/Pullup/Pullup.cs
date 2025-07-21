using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Handles showing/hiding a pull-up panel by animating its RectTransform vertically,
/// fading a blocker CanvasGroup alongside it, and supports drag-to-close.
/// </summary>
public class Pullup : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI References")]
    [SerializeField] private RectTransform pullupRectTransform;
    [SerializeField] private CanvasGroup blockerCanvasGroup;

    [Header("Animation Settings")]
    [Tooltip("Duration of the pull-up animation in seconds.")]
    [SerializeField] private float animationDuration = 0.5f;
    [Tooltip("Open height of the panel in pixels.")]
    [SerializeField] private float targetHeight = 300f;
    [Tooltip("Drag release threshold (fraction of height) below which it closes.")]
    [Range(0f, 1f)]
    [SerializeField] private float closeThreshold = 0.5f;

    private bool isOpen = false;
    private bool isDragging = false;
    private Vector2 dragStartLocalPos;
    private float startHeight;

    private void Awake()
    {
        // Start closed
        SetHeight(0f);
        blockerCanvasGroup.alpha = 0f;
        blockerCanvasGroup.blocksRaycasts = false;
        blockerCanvasGroup.interactable = false;
    }

    /// <summary>
    /// Opens or closes the panel with animation.
    /// </summary>
    public void SetActivePullup(bool open)
    {
        isOpen = open;
        StopAllCoroutines();
        StartCoroutine(AnimatePullup(open));
        SetActiveBlocker(open);
    }

    /// <summary>
    /// Fades the blocker overlay.
    /// </summary>
    public void SetActiveBlocker(bool isOn)
    {
        float alpha = isOn ? 1f : 0f;
        blockerCanvasGroup.FadeTo(alpha, animationDuration, isOn, isOn);
    }

    private IEnumerator AnimatePullup(bool open)
    {
        float from = pullupRectTransform.sizeDelta.y;
        float to = open ? targetHeight : 0f;
        foreach (float t in AlphaCurve.Linear(animationDuration))
        {
            float h = Mathf.Lerp(from, to, t);
            SetHeight(h);
            yield return null;
        }
        SetHeight(to);
    }

    private void SetHeight(float height)
    {
        Vector2 sd = pullupRectTransform.sizeDelta;
        sd.y = height;
        pullupRectTransform.sizeDelta = sd;
    }

    // Called when user starts touching the panel
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isOpen) return;
        isDragging = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            pullupRectTransform, eventData.position, eventData.pressEventCamera, out dragStartLocalPos);
        startHeight = pullupRectTransform.sizeDelta.y;
    }

    // Called while dragging
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            pullupRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        float delta = localPoint.y - dragStartLocalPos.y;
        float newHeight = Mathf.Clamp(startHeight + delta, 0f, targetHeight);
        SetHeight(newHeight);
        // Update blocker alpha proportionally
        blockerCanvasGroup.alpha = newHeight / targetHeight;
    }

    // Called when user releases
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        float currentHeight = pullupRectTransform.sizeDelta.y;
        bool shouldOpen = currentHeight >= targetHeight * closeThreshold;
        SetActivePullup(shouldOpen);
    }
}
