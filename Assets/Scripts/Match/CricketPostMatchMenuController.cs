using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows a simple full-time result and local career summary, and starts the next
/// saved knockout fixture when one exists.
/// </summary>
public class CricketPostMatchMenuController : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private CricketCareerSaveSystem careerSave;
    [SerializeField] private CricketTournamentManager tournament;
    [SerializeField] private CricketMatchProgressionBridge progression;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultText;
    [SerializeField] private Text careerText;

    private bool resultShown;

    private void Start()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void Update()
    {
        if (matchManager == null || !matchManager.IsMatchCompleted || resultShown) return;
        resultShown = true;
        if (progression != null) progression.ProcessCompletedMatch();
        RefreshResultPanel();
        if (resultPanel != null) resultPanel.SetActive(true);
    }

    public void RefreshResultPanel()
    {
        if (matchManager != null && resultText != null)
        {
            resultText.text = matchManager.MatchResultText + "\n" +
                matchManager.TeamAName + "  " + matchManager.GetTeamTotalRuns(matchManager.TeamAName) +
                "  -  " + matchManager.TeamBName + "  " +
                matchManager.GetTeamTotalRuns(matchManager.TeamBName);
        }

        if (careerSave != null && careerText != null)
        {
            CricketCareerProfile profile = careerSave.Profile;
            careerText.text = profile.playerName + "  |  MATCHES " + profile.matches +
                "  W " + profile.wins + "  L " + profile.losses +
                "  D " + profile.draws + "\nRUNS " + profile.totalRuns +
                "  WICKETS " + profile.totalWickets +
                "  TROPHIES " + profile.trophies;
        }
    }

    public void StartNextTournamentFixture()
    {
        if (matchManager == null || tournament == null ||
            !tournament.HasTournament || tournament.IsComplete) return;

        string teamA = tournament.GetCurrentTeamA();
        string teamB = tournament.GetCurrentTeamB();
        if (string.IsNullOrWhiteSpace(teamA) || string.IsNullOrWhiteSpace(teamB)) return;

        matchManager.ConfigureFormat(CricketMatchFormat.T20);
        matchManager.SetTeamNames(teamA, teamB);
        matchManager.StartMatch();
        resultShown = false;
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    public void CloseResultPanel()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
    }
}
