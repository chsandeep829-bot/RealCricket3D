using UnityEngine;

/// <summary>
/// Connects completed offline matches to the local career profile and knockout bracket.
/// The player is treated as Team A for this prototype; a future team-selection menu
/// can call SetPlayerTeamName with the selected side.
/// </summary>
public class CricketMatchProgressionBridge : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private CricketCareerSaveSystem careerSave;
    [SerializeField] private CricketTournamentManager tournament;
    [SerializeField] private string playerTeamName = "TEAM A";

    private bool resultProcessed;
    private bool trophyAwarded;

    public void SetPlayerTeamName(string teamName)
    {
        if (!string.IsNullOrWhiteSpace(teamName))
            playerTeamName = teamName.Trim();
    }

    private void Update()
    {
        ProcessCompletedMatch();
    }

    // Public and idempotent so the result screen can force progression to be
    // saved before it renders the career totals and tournament status.
    public void ProcessCompletedMatch()
    {
        if (matchManager == null) return;

        // Keep the default player side aligned if the match setup changes Team A.
        if (!matchManager.IsMatchCompleted && !string.IsNullOrWhiteSpace(matchManager.TeamAName))
            playerTeamName = matchManager.TeamAName;

        if (!matchManager.IsMatchCompleted)
        {
            resultProcessed = false;
            return;
        }

        if (resultProcessed) return;
        resultProcessed = true;

        if (careerSave != null)
            careerSave.RecordCompletedMatch(matchManager, playerTeamName);

        if (tournament != null)
        {
            tournament.RecordWinnerFromMatch(matchManager);
            if (tournament.IsComplete && !trophyAwarded && careerSave != null)
            {
                careerSave.AwardTrophy();
                trophyAwarded = true;
            }
        }
    }
}
