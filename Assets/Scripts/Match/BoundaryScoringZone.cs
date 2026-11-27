using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoundaryScoringZone : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField, Range(4, 6)] private int boundaryRuns = 4;
    [SerializeField] private bool scoreOnlyOncePerBall = true;

    private CricketBall lastScoredBall;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CricketBall ball = other.GetComponentInParent<CricketBall>();
        if (ball == null || !ball.HasBeenHit || matchManager == null) return;
        if (scoreOnlyOncePerBall && lastScoredBall == ball) return;

        lastScoredBall = ball;
        if (!ball.MarkDeliveryResolved()) return;
        matchManager.RecordBoundaryDelivery(boundaryRuns);
    }
}
