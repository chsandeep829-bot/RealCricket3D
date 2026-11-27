using UnityEngine;

public class MatchScoringRules : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField, Min(0)] private int noBallPenalty = 1;
    [SerializeField, Min(0)] private int widePenalty = 1;

    public void LegalDelivery(int batterRuns = 0)
    {
        if (matchManager == null) return;
        matchManager.RecordLegalDelivery(Mathf.Max(0, batterRuns));
    }

    public void Wide(int additionalRuns = 0)
    {
        if (matchManager == null) return;
        matchManager.AddRuns(widePenalty + Mathf.Max(0, additionalRuns));
        // A wide does not count as a legal ball.
    }

    public void NoBall(int batterRuns = 0)
    {
        if (matchManager == null) return;
        matchManager.AddRuns(noBallPenalty + Mathf.Max(0, batterRuns));
        // A no-ball does not count as a legal ball.
    }

    public void Bye(int runs)
    {
        if (matchManager == null) return;
        matchManager.RecordBye(Mathf.Max(0, runs));
    }

    public void LegBye(int runs)
    {
        if (matchManager == null) return;
        matchManager.RecordLegBye(Mathf.Max(0, runs));
    }

    public void WicketOnLegalDelivery()
    {
        if (matchManager == null) return;
        matchManager.RecordWicketOnLegalDelivery();
    }
}
