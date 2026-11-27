using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class FielderCatchZone : MonoBehaviour
{
    [Header("Catch Rules")]
    [SerializeField] private MatchManager matchManager;
    [SerializeField, Min(0f)] private float minimumCatchHeight = 0.8f;
    [SerializeField, Min(0f)] private float maximumCatchHeight = 3.5f;
    [SerializeField] private bool requireDescendingBall = true;
    [SerializeField] private UnityEvent onCatch;

    private bool catchCompleted;

    private void Reset()
    {
        Collider zone = GetComponent<Collider>();
        zone.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (catchCompleted || matchManager == null) return;

        CricketBall ball = other.GetComponentInParent<CricketBall>();
        if (ball == null || !ball.HasBeenHit || ball.DismissalRecorded || ball.Body == null) return;

        float height = ball.transform.position.y;
        if (height < minimumCatchHeight || height > maximumCatchHeight) return;
        if (requireDescendingBall && ball.Body.velocity.y > 0.25f) return;

        catchCompleted = true;
        ball.MarkDismissal();
        matchManager.RecordWicketOnLegalDelivery();
        onCatch?.Invoke();
    }

    public void ResetCatchZone()
    {
        catchCompleted = false;
    }
}
