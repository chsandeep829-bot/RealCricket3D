using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assigns each newly hit ball to the nearest available fielder.
/// Add this component to a scene manager object and assign fielders,
/// or enable automatic scene discovery.
/// </summary>
public class FielderTeamController : MonoBehaviour
{
    [Header("Team")]
    [SerializeField] private FielderAI[] fielders;
    [SerializeField] private bool discoverFieldersOnStart = true;

    [Header("Ball Scanning")]
    [SerializeField, Min(0.05f)] private float scanInterval = 0.2f;
    [SerializeField] private bool onlyChaseHitBalls = true;

    private readonly HashSet<CricketBall> assignedBalls = new HashSet<CricketBall>();
    private float nextScanTime;

    private void Start()
    {
        if (discoverFieldersOnStart || fielders == null || fielders.Length == 0)
            fielders = FindObjectsOfType<FielderAI>();
    }

    private void Update()
    {
        if (Time.time < nextScanTime) return;
        nextScanTime = Time.time + scanInterval;

        assignedBalls.RemoveWhere(ball => ball == null);

        CricketBall[] balls = FindObjectsOfType<CricketBall>();
        foreach (CricketBall ball in balls)
        {
            if (ball == null || assignedBalls.Contains(ball)) continue;
            if (onlyChaseHitBalls && !ball.HasBeenHit) continue;

            FielderAI nearest = FindNearestAvailableFielder(ball.transform.position);
            if (nearest == null) continue;

            nearest.Chase(ball);
            assignedBalls.Add(ball);
        }
    }

    private FielderAI FindNearestAvailableFielder(Vector3 ballPosition)
    {
        if (fielders == null) return null;

        FielderAI nearest = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (FielderAI fielder in fielders)
        {
            if (fielder == null || fielder.IsBusy) continue;

            float distance = (fielder.transform.position - ballPosition).sqrMagnitude;
            if (distance >= nearestDistance) continue;

            nearestDistance = distance;
            nearest = fielder;
        }

        return nearest;
    }

    public void SetFielders(FielderAI[] teamFielders)
    {
        fielders = teamFielders;
    }

    public void RefreshFielders()
    {
        fielders = FindObjectsOfType<FielderAI>();
    }
}
