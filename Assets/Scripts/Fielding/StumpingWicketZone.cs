using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class StumpingWicketZone : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private RunningBetweenWicketsController runningController;
    [SerializeField] private UnityEvent onStumping;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (matchManager == null || runningController == null ||
            matchManager.IsInningsCompleted || matchManager.IsMatchCompleted ||
            !runningController.IsStrikerOutOfCrease) return;

        CricketBall ball = other.GetComponentInParent<CricketBall>();
        if (ball == null || ball.HasBeenHit || ball.DismissalRecorded) return;

        ball.MarkDismissal();
        matchManager.RecordWicketOnLegalDelivery();
        onStumping?.Invoke();
    }
}
