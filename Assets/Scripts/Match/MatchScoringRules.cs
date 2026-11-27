using UnityEngine;

public class MatchScoringRules : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField, Min(0)] private int noBallPenalty = 1;
    [SerializeField, Min(0)] private int widePenalty = 1;

    public void LegalDelivery(int batterRuns = 0)
    {
        if (matchManager == null) return;
        if (batterRuns > 0) matchManager.AddRuns(batterRuns);
        matchManager.RecordLegalDelivery();
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
        matchManager.AddRuns(Mathf.Max(0, runs));
        matchManager.RecordLegalDelivery();
    }

    public void WicketOnLegalDelivery()
    {
        if (matchManager == null) return;
        // Count the delivery before the wicket so the final wicket still records the ball.
        matchManager.RecordLegalDelivery();
        matchManager.RecordWicket();
    }
}
