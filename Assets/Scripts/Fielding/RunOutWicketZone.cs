using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class RunOutWicketZone : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private RunningBetweenWicketsController runningController;
    [SerializeField] private UnityEvent onRunOut;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (matchManager == null || runningController == null ||
            !runningController.IsRunning || matchManager.IsInningsCompleted ||
            matchManager.IsMatchCompleted) return;

        CricketBall ball = other.GetComponentInParent<CricketBall>();
        if (ball == null || !ball.HasBeenHit || ball.DismissalRecorded) return;

        ball.MarkDismissal();
        runningController.CancelQueuedRuns();
        matchManager.RecordRunOutOnLegalDelivery();
        onRunOut?.Invoke();
    }
}
