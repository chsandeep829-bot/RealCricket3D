using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WicketTrigger : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private bool disableAfterWicket = true;
    private bool wicketRecorded;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CricketBall ball = other.GetComponentInParent<CricketBall>();
        if (ball == null || ball.HasBeenHit || ball.DismissalRecorded || wicketRecorded || matchManager == null) return;

        wicketRecorded = true;
        ball.MarkDismissal();
        matchManager.RecordWicketOnLegalDelivery();
        if (disableAfterWicket) enabled = false;
    }

    public void ResetWicket()
    {
        wicketRecorded = false;
        enabled = true;
    }
}
