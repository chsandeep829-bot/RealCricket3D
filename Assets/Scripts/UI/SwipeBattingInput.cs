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

    private Vector2 pointerStart;
    private bool tracking;

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
        if (delta.magnitude < minimumSwipePixels || shotSelection == null) return;

        Vector2 swipe = delta.normalized;
        shotSelection.PlaySwipeShot(swipe);
    }

    public void SetShotSelection(ShotSelectionController controller)
    {
        shotSelection = controller;
    }
}
