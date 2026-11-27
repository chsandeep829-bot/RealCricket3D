using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Converts a drag gesture in the gameplay area into a cricket batting shot.
/// UI buttons placed above this image continue to receive their own clicks.
/// </summary>
[RequireComponent(typeof(Image))]
public class SwipeBattingInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
{
    [SerializeField] private ShotSelectionController shotSelection;
    [SerializeField, Min(10f)] private float minimumSwipePixels = 65f;
    [SerializeField, Range(0.5f, 2f)] private float swipeSensitivity = 1f;

    private const string SensitivityKey = "RealCricket3D.Controls.SwipeSensitivity";
    private Vector2 pointerStart;
    private bool tracking;

    public float SwipeSensitivity => swipeSensitivity;
    public float SensitivitySliderValue => Mathf.InverseLerp(0.5f, 2f, swipeSensitivity);

    private void Awake()
    {
        if (PlayerPrefs.HasKey(SensitivityKey))
            swipeSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat(SensitivityKey, 1f), 0.5f, 2f);
    }

    /// <summary>Connect a 0–1 UI Slider to configure and persist swipe sensitivity.</summary>
    public void SetSensitivityFromSlider(float normalizedValue)
    {
        swipeSensitivity = Mathf.Lerp(0.5f, 2f, Mathf.Clamp01(normalizedValue));
        PlayerPrefs.SetFloat(SensitivityKey, swipeSensitivity);
        PlayerPrefs.Save();
    }

    public void ResetSensitivity()
    {
        swipeSensitivity = 1f;
        PlayerPrefs.SetFloat(SensitivityKey, swipeSensitivity);
        PlayerPrefs.Save();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerStart = eventData.position;
        tracking = true;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!tracking) return;
        tracking = false;

        Vector2 delta = eventData.position - pointerStart;
        if (shotSelection == null) return;

        float requiredPixels = minimumSwipePixels / Mathf.Max(0.5f, swipeSensitivity);
        if (delta.magnitude < requiredPixels) return;

        Vector2 swipe = delta.normalized;
        swipe.x = Mathf.Clamp(swipe.x * swipeSensitivity, -1f, 1f);
        shotSelection.PlaySwipeShot(swipe);
    }

    public void SetShotSelection(ShotSelectionController controller)
    {
        shotSelection = controller;
    }
}
