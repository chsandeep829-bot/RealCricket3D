using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Completes ordinary deliveries when the ball has settled. Wicket and boundary
/// triggers resolve their own delivery first, so this component does not double-count.
/// </summary>
public class CricketBallDeliveryResolver : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField, Min(0.05f)] private float scanInterval = 0.15f;
    [SerializeField, Min(0.1f)] private float minimumFlightSeconds = 0.65f;
    [SerializeField, Min(0.05f)] private float settleSpeed = 0.45f;
    [SerializeField, Min(0.1f)] private float settleDuration = 0.55f;
    [SerializeField, Min(1f)] private float maximumDeliverySeconds = 10f;

    private readonly Dictionary<CricketBall, float> stationarySince =
        new Dictionary<CricketBall, float>();
    private float nextScanTime;

    private void Update()
    {
        if (Time.time < nextScanTime) return;
        nextScanTime = Time.time + scanInterval;
        if (matchManager == null || matchManager.IsMatchCompleted ||
            matchManager.IsInningsCompleted) return;

        CricketBall[] balls = FindObjectsOfType<CricketBall>();
        HashSet<CricketBall> active = new HashSet<CricketBall>(balls);

        List<CricketBall> stale = new List<CricketBall>();
        foreach (CricketBall tracked in stationarySince.Keys)
            if (tracked == null || !active.Contains(tracked)) stale.Add(tracked);
        foreach (CricketBall tracked in stale) stationarySince.Remove(tracked);

        foreach (CricketBall ball in balls)
        {
            if (ball == null || ball.DeliveryResolved || ball.DismissalRecorded ||
                ball.Body == null || ball.Body.isKinematic) continue;

            float age = Time.time - ball.SpawnedAt;
            if (age < minimumFlightSeconds) continue;

            if (ball.Body.velocity.sqrMagnitude <= settleSpeed * settleSpeed)
            {
                if (!stationarySince.ContainsKey(ball))
                    stationarySince.Add(ball, Time.time);

                if (Time.time - stationarySince[ball] >= settleDuration)
                    ResolveAsLegalDotBall(ball);
            }
            else
            {
                stationarySince.Remove(ball);
            }

            if (!ball.DeliveryResolved && age >= maximumDeliverySeconds)
                ResolveAsLegalDotBall(ball);
        }
    }

    private void ResolveAsLegalDotBall(CricketBall ball)
    {
        if (ball == null || !ball.MarkDeliveryResolved()) return;
        stationarySince.Remove(ball);
        matchManager.RecordLegalDelivery();
    }

    public void SetMatchManager(MatchManager manager)
    {
        matchManager = manager;
    }
}