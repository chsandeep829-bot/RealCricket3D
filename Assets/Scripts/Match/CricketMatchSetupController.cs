using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Match-menu bridge for team selection, format selection and a simple toss decision.
/// Connect the public methods to menu buttons and dropdowns.
/// </summary>
public class CricketMatchSetupController : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private string[] availableTeams = { "INDIA", "AUSTRALIA", "ENGLAND", "SOUTH AFRICA", "NEW ZEALAND", "PAKISTAN" };
    [SerializeField] private int firstTeamIndex;
    [SerializeField] private int secondTeamIndex = 1;
    [SerializeField] private CricketMatchFormat selectedFormat = CricketMatchFormat.T20;
    [SerializeField] private Text setupStatus;

    private int tossWinnerIndex = -1;
    private bool tossWinnerBatsFirst;

    public string[] AvailableTeams => availableTeams;
    public int FirstTeamIndex => firstTeamIndex;
    public int SecondTeamIndex => secondTeamIndex;
    public CricketMatchFormat SelectedFormat => selectedFormat;
    public bool IsTossReady => tossWinnerIndex >= 0;

    public void SelectFirstTeam(int index)
    {
        if (!ValidTeamIndex(index) || index == secondTeamIndex) return;
        firstTeamIndex = index;
        UpdateStatus();
    }

    public void SelectSecondTeam(int index)
    {
        if (!ValidTeamIndex(index) || index == firstTeamIndex) return;
        secondTeamIndex = index;
        UpdateStatus();
    }

    public void SelectT20() => selectedFormat = CricketMatchFormat.T20;
    public void SelectODI() => selectedFormat = CricketMatchFormat.ODI;
    public void SelectTest() => selectedFormat = CricketMatchFormat.Test;

    public void PerformToss()
    {
        if (!HasValidSetup()) return;
        tossWinnerIndex = Random.Range(0, 2);
        tossWinnerBatsFirst = false;
        string winner = TeamName(tossWinnerIndex == 0 ? firstTeamIndex : secondTeamIndex);
        if (setupStatus != null) setupStatus.text = winner + " won the toss. Choose bat or bowl.";
    }

    public void TossWinnerChooseBat() => ChooseTossDecision(true);
    public void TossWinnerChooseBowl() => ChooseTossDecision(false);

    private void ChooseTossDecision(bool batsFirst)
    {
        if (tossWinnerIndex < 0) return;
        tossWinnerBatsFirst = batsFirst;
        StartConfiguredMatch();
    }

    public void StartConfiguredMatch()
    {
        if (matchManager == null || !HasValidSetup())
        {
            if (setupStatus != null) setupStatus.text = "Assign MatchManager and select two different teams.";
            return;
        }

        string teamA = TeamName(firstTeamIndex);
        string teamB = TeamName(secondTeamIndex);
        if (tossWinnerIndex >= 0 && !tossWinnerBatsFirst)
        {
            // Toss winner elected to field, so the opposing team bats first.
            if (tossWinnerIndex == 0) Swap(ref teamA, ref teamB);
        }
        else if (tossWinnerIndex == 1 && tossWinnerBatsFirst)
        {
            Swap(ref teamA, ref teamB);
        }

        matchManager.ConfigureFormat(selectedFormat);
        matchManager.SetTeamNames(teamA, teamB);
        matchManager.StartMatch();
        if (setupStatus != null) setupStatus.text = teamA + " batting first — " + selectedFormat;
    }

    private bool ValidTeamIndex(int index) => availableTeams != null && index >= 0 && index < availableTeams.Length;
    private bool HasValidSetup() => ValidTeamIndex(firstTeamIndex) && ValidTeamIndex(secondTeamIndex) && firstTeamIndex != secondTeamIndex;
    private string TeamName(int index) => ValidTeamIndex(index) ? availableTeams[index] : "TEAM";
    private void UpdateStatus()
    {
        if (setupStatus != null) setupStatus.text = TeamName(firstTeamIndex) + " vs " + TeamName(secondTeamIndex);
    }
    private static void Swap(ref string a, ref string b) { string temp = a; a = b; b = temp; }
}
