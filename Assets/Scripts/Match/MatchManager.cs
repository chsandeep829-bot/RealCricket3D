using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum CricketMatchFormat
{
    T20,
    ODI,
    Test
}

public class MatchManager : MonoBehaviour
{
    [Header("Match Setup")]
    [SerializeField] private CricketMatchFormat format = CricketMatchFormat.T20;
    [SerializeField] private string teamAName = "TEAM A";
    [SerializeField] private string teamBName = "TEAM B";
    [SerializeField, Min(1)] private int t20Overs = 20;
    [SerializeField, Min(1)] private int odiOvers = 50;
    [SerializeField, Min(1)] private int testOversPerInnings = 90;
    [SerializeField] private bool startMatchOnPlay = true;

    [Header("Match State (read only at runtime)")]
    [SerializeField] private int runs;
    [SerializeField] private int wickets;
    [SerializeField] private int legalBalls;
    [SerializeField] private int inningsNumber = 1;
    [SerializeField] private int maxOvers = 20;

    [Header("Optional UI and Events")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private UnityEvent onInningsCompleted;
    [SerializeField] private UnityEvent onMatchCompleted;

    private readonly int[] inningsRuns = new int[4];
    private readonly int[] inningsWickets = new int[4];
    private bool inningsCompleted;
    private bool matchCompleted;
    private bool matchDraw;
    private int target;
    private bool strikerAtBowlerEnd;

    public int Runs => runs;
    public int Wickets => wickets;
    public int LegalBalls => legalBalls;
    public int Overs => legalBalls / 6;
    public int BallsInOver => legalBalls % 6;
    public int InningsNumber => inningsNumber;
    public int Target => target;
    public bool StrikerAtBowlerEnd => strikerAtBowlerEnd;
    public bool IsInningsCompleted => inningsCompleted;
    public bool IsMatchCompleted => matchCompleted;
    public CricketMatchFormat Format => format;
    public int MaxOvers => maxOvers;
    public string BattingTeamName => (inningsNumber == 1 || inningsNumber == 3) ? teamAName : teamBName;
    public string BowlingTeamName => (inningsNumber == 1 || inningsNumber == 3) ? teamBName : teamAName;
    public int InningsCount => format == CricketMatchFormat.Test ? 4 : 2;

    private void Start()
    {
        ApplyFormatLimits();
        if (startMatchOnPlay) StartMatch();
        else RefreshScoreboard();
    }

    public void ConfigureFormat(CricketMatchFormat selectedFormat)
    {
        format = selectedFormat;
        ApplyFormatLimits();
        RefreshScoreboard();
    }

    public void SetTeamNames(string firstTeam, string secondTeam)
    {
        if (!string.IsNullOrWhiteSpace(firstTeam)) teamAName = firstTeam.Trim();
        if (!string.IsNullOrWhiteSpace(secondTeam)) teamBName = secondTeam.Trim();
        RefreshScoreboard();
    }

    public void StartMatch()
    {
        Array.Clear(inningsRuns, 0, inningsRuns.Length);
        Array.Clear(inningsWickets, 0, inningsWickets.Length);
        inningsNumber = 1;
        target = 0;
        matchCompleted = false;
        matchDraw = false;
        strikerAtBowlerEnd = false;
        BeginCurrentInnings();
    }

    public int GetInningsRuns(int oneBasedInnings)
    {
        if (oneBasedInnings < 1 || oneBasedInnings > InningsCount) return 0;
        return inningsRuns[oneBasedInnings - 1];
    }

    public int GetInningsWickets(int oneBasedInnings)
    {
        if (oneBasedInnings < 1 || oneBasedInnings > InningsCount) return 0;
        return inningsWickets[oneBasedInnings - 1];
    }

    public void AddRuns(int amount)
    {
        if (!CanScore() || amount < 0) return;
        runs += amount;
        SaveCurrentInnings();
        if (target > 0 && runs >= target) CompleteMatch();
        else RefreshScoreboard();
    }

    public void RecordLegalDelivery(int batterRuns)
    {
        if (!CanScore() || batterRuns < 0) return;
        runs += batterRuns;
        RotateStrikeForRuns(batterRuns);
        legalBalls++;
        SaveCurrentInnings();
        CheckDeliveryEnd();
    }

    public void RecordLegalDelivery()
    {
        if (!CanScore()) return;
        legalBalls++;
        SaveCurrentInnings();
        CheckDeliveryEnd();
    }

    public void RecordWicket()
    {
        if (!CanScore()) return;
        wickets = Mathf.Min(10, wickets + 1);
        SaveCurrentInnings();
        if (wickets >= 10) CompleteInnings();
        else RefreshScoreboard();
    }

    // Use this for a wicket on a legal ball so the final wicket cannot
    // complete the innings before the delivery has been recorded.
    public void RecordRunOutOnLegalDelivery()
    {
        RecordWicketOnLegalDelivery();
    }

    public void RecordWicketOnLegalDelivery()
    {
        if (!CanScore()) return;
        legalBalls++;
        wickets = Mathf.Min(10, wickets + 1);
        if (legalBalls % 6 == 0) strikerAtBowlerEnd = !strikerAtBowlerEnd;
        SaveCurrentInnings();

        if (target > 0 && runs >= target) CompleteMatch();
        else if (wickets >= 10 || legalBalls >= maxOvers * 6) CompleteInnings();
        else RefreshScoreboard();
    }

    public void RecordWide(int penaltyRuns = 1, int additionalRuns = 0)
    {
        if (!CanScore()) return;
        int total = Mathf.Max(0, penaltyRuns) + Mathf.Max(0, additionalRuns);
        runs += total;
        RotateStrikeForRuns(Mathf.Max(0, additionalRuns));
        SaveCurrentInnings();
        if (target > 0 && runs >= target) CompleteMatch();
        else RefreshScoreboard();
    }

    public void RecordNoBall(int penaltyRuns = 1, int batterRuns = 0)
    {
        if (!CanScore()) return;
        int total = Mathf.Max(0, penaltyRuns) + Mathf.Max(0, batterRuns);
        runs += total;
        RotateStrikeForRuns(Mathf.Max(0, batterRuns));
        SaveCurrentInnings();
        if (target > 0 && runs >= target) CompleteMatch();
        else RefreshScoreboard();
    }

    // Called once for each completed run while batters physically cross.
    public void RecordRunningRuns(int completedRuns)
    {
        if (!CanScore() || completedRuns <= 0) return;
        runs += completedRuns;
        RotateStrikeForRuns(completedRuns);
        SaveCurrentInnings();
        if (target > 0 && runs >= target) CompleteMatch();
        else RefreshScoreboard();
    }

    public void RecordBye(int byeRuns)
    {
        if (!CanScore() || byeRuns < 0) return;
        runs += byeRuns;
        RotateStrikeForRuns(byeRuns);
        legalBalls++;
        SaveCurrentInnings();
        CheckDeliveryEnd();
    }

    public void RecordLegBye(int legByeRuns)
    {
        RecordBye(legByeRuns);
    }

    // Retained for compatibility with the original prototype controls.
    public void ResetInnings()
    {
        if (matchCompleted) return;
        BeginCurrentInnings();
    }

    private bool CanScore() => !inningsCompleted && !matchCompleted;

    private void ApplyFormatLimits()
    {
        switch (format)
        {
            case CricketMatchFormat.ODI:
                maxOvers = odiOvers;
                break;
            case CricketMatchFormat.Test:
                maxOvers = testOversPerInnings;
                break;
            default:
                maxOvers = t20Overs;
                break;
        }
        maxOvers = Mathf.Max(1, maxOvers);
    }

    private void BeginCurrentInnings()
    {
        runs = 0;
        wickets = 0;
        legalBalls = 0;
        strikerAtBowlerEnd = false;
        inningsCompleted = false;
        ApplyFormatLimits();

        // Only the final innings of a Test match has a chase target.
        if (format == CricketMatchFormat.Test && inningsNumber == 4)
            target = inningsRuns[0] + inningsRuns[2] - inningsRuns[1] + 1;
        else if (format != CricketMatchFormat.Test && inningsNumber == 2)
            target = inningsRuns[0] + 1;
        else
            target = 0;

        RefreshScoreboard();
    }

    private void CheckDeliveryEnd()
    {
        if (target > 0 && runs >= target) CompleteMatch();
        else if (wickets >= 10 || legalBalls >= maxOvers * 6) CompleteInnings();
        else
        {
            // At the end of each six-ball over, the batters change ends.
            if (legalBalls > 0 && legalBalls % 6 == 0) strikerAtBowlerEnd = !strikerAtBowlerEnd;
            RefreshScoreboard();
        }
    }

    private void RotateStrikeForRuns(int completedRuns)
    {
        if (completedRuns % 2 != 0) strikerAtBowlerEnd = !strikerAtBowlerEnd;
    }

    private void SaveCurrentInnings()
    {
        int index = Mathf.Clamp(inningsNumber - 1, 0, inningsRuns.Length - 1);
        inningsRuns[index] = runs;
        inningsWickets[index] = wickets;
    }

    private void CompleteInnings()
    {
        if (inningsCompleted || matchCompleted) return;
        inningsCompleted = true;
        SaveCurrentInnings();
        RefreshScoreboard();
        onInningsCompleted?.Invoke();

        if (inningsNumber >= InningsCount)
        {
            if (format == CricketMatchFormat.Test && target > runs) matchDraw = true;
            CompleteMatch();
            return;
        }

        inningsNumber++;
        BeginCurrentInnings();
    }

    private void CompleteMatch()
    {
        if (matchCompleted) return;
        SaveCurrentInnings();
        inningsCompleted = true;
        matchCompleted = true;
        RefreshScoreboard();
        onMatchCompleted?.Invoke();
    }

    private string GetResultText()
    {
        if (!matchCompleted) return string.Empty;
        if (matchDraw) return "MATCH DRAWN";

        int teamAScore = inningsRuns[0] + (format == CricketMatchFormat.Test ? inningsRuns[2] : 0);
        int teamBScore = inningsRuns[1] + (format == CricketMatchFormat.Test ? inningsRuns[3] : 0);
        if (teamAScore == teamBScore) return "MATCH TIED";
        return teamAScore > teamBScore ? teamAName + " WIN" : teamBName + " WIN";
    }

    private void RefreshScoreboard()
    {
        if (scoreText == null) return;

        string oversText = Overs + "." + BallsInOver;
        string targetText = target > 0 ? "\\nTARGET  " + target : string.Empty;
        string statusText = matchCompleted ? "\\n" + GetResultText() :
            inningsCompleted ? "\\nINNINGS COMPLETE" : string.Empty;

        scoreText.text =
            BattingTeamName + "  " + runs + "/" + wickets +
            "\\nINNINGS  " + inningsNumber + "/" + InningsCount +
            "\\nOVERS  " + oversText + " / " + maxOvers +
            "\\nSTRIKER END  " + (strikerAtBowlerEnd ? "BOWLER END" : "SCORE END") +
            targetText + statusText;
    }
}
