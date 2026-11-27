using UnityEngine;

/// <summary>
/// Fits a full-screen UI RectTransform inside the device safe area.
/// Put this on the root gameplay/menu panel under a screen-space Canvas.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class CricketSafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Update()
    {
        if (lastSafeArea != Screen.safeArea ||
            lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
            ApplySafeArea();
    }

    public void ApplySafeArea()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        Rect safe = Screen.safeArea;
        lastSafeArea = safe;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (Screen.width <= 0 || Screen.height <= 0) return;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;
        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;

        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}