using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MatchManager : MonoBehaviour
{
    [Header("Match State")]
    [SerializeField] private int runs;
    [SerializeField] private int wickets;
    [SerializeField] private int legalBalls;
    [SerializeField, Min(1)] private int maxOvers = 2;

    [Header("Optional UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private UnityEvent onInningsCompleted;

    private bool inningsCompleted;

    public int Runs => runs;
    public int Wickets => wickets;
    public int LegalBalls => legalBalls;
    public int Overs => legalBalls / 6;
    public int BallsInOver => legalBalls % 6;

    private void Start() => RefreshScoreboard();

    public void AddRuns(int amount)
    {
        if (inningsCompleted || amount < 0) return;
        runs += amount;
        RefreshScoreboard();
    }

    public void RecordLegalDelivery()
    {
        if (inningsCompleted) return;
        legalBalls++;
        RefreshScoreboard();
        CheckInningsEnd();
    }

    public void RecordWicket()
    {
        if (inningsCompleted) return;
        wickets++;
        RefreshScoreboard();
        CheckInningsEnd();
    }

    public void ResetInnings()
    {
        runs = 0;
        wickets = 0;
        legalBalls = 0;
        inningsCompleted = false;
        RefreshScoreboard();
    }

    private void CheckInningsEnd()
    {
        if (legalBalls >= maxOvers * 6 || wickets >= 10)
        {
            inningsCompleted = true;
            onInningsCompleted?.Invoke();
        }
    }

    private void RefreshScoreboard()
    {
        if (scoreText == null) return;
        scoreText.text = $"RUNS  {runs}/{wickets}\nOVERS  {Overs}.{BallsInOver}";
    }
}
