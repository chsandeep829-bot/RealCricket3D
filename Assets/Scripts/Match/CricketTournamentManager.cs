using System;
using UnityEngine;

[Serializable]
public class CricketTournamentState
{
    public string[] teams = new string[4];
    public string[] winners = new string[3];
    public int completedMatches;
}

/// <summary>
/// Simple four-team knockout bracket. Call RecordCurrentMatchWinner after each
/// completed match; connect this to the match-result screen.
/// </summary>
public class CricketTournamentManager : MonoBehaviour
{
    private const string TournamentKey = "RealCricket3D.Tournament.v1";
    [SerializeField] private CricketTournamentState state = new CricketTournamentState();

    public CricketTournamentState State => state;
    public bool IsComplete => state != null && state.completedMatches >= 3;

    public void CreateTournament(string team1, string team2, string team3, string team4)
    {
        state = new CricketTournamentState
        {
            teams = new[] { Clean(team1), Clean(team2), Clean(team3), Clean(team4) },
            winners = new string[3],
            completedMatches = 0
        };
        SaveTournament();
    }

    public string GetCurrentTeamA()
    {
        if (state == null) return string.Empty;
        if (state.completedMatches == 0) return state.teams[0];
        if (state.completedMatches == 1) return state.teams[2];
        if (state.completedMatches == 2) return state.winners[0];
        return string.Empty;
    }

    public string GetCurrentTeamB()
    {
        if (state == null) return string.Empty;
        if (state.completedMatches == 0) return state.teams[1];
        if (state.completedMatches == 1) return state.teams[3];
        if (state.completedMatches == 2) return state.winners[1];
        return string.Empty;
    }

    public void RecordCurrentMatchWinner(string winnerName)
    {
        if (state == null || IsComplete || string.IsNullOrWhiteSpace(winnerName)) return;
        string teamA = GetCurrentTeamA();
        string teamB = GetCurrentTeamB();
        string winner = winnerName.Trim();
        if (!string.Equals(winner, teamA, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(winner, teamB, StringComparison.OrdinalIgnoreCase)) return;

        state.winners[state.completedMatches] = winner;
        state.completedMatches++;
        SaveTournament();
    }

    public string GetChampion() => IsComplete ? state.winners[2] : string.Empty;

    public void SaveTournament()
    {
        if (state != null) PlayerPrefs.SetString(TournamentKey, JsonUtility.ToJson(state));
        PlayerPrefs.Save();
    }

    public void LoadTournament()
    {
        if (!PlayerPrefs.HasKey(TournamentKey)) return;
        CricketTournamentState loaded = JsonUtility.FromJson<CricketTournamentState>(PlayerPrefs.GetString(TournamentKey));
        if (loaded != null && loaded.teams != null && loaded.teams.Length == 4 &&
            loaded.winners != null && loaded.winners.Length == 3) state = loaded;
    }

    public void ClearTournament()
    {
        state = new CricketTournamentState();
        PlayerPrefs.DeleteKey(TournamentKey);
        PlayerPrefs.Save();
    }

    private void Awake() => LoadTournament();
    private static string Clean(string value) => string.IsNullOrWhiteSpace(value) ? "TEAM" : value.Trim();
}
