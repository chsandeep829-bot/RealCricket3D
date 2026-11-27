using System;
using UnityEngine;

[Serializable]
public class CricketCareerProfile
{
    public string playerName = "PLAYER";
    public int matches;
    public int wins;
    public int losses;
    public int draws;
    public int totalRuns;
    public int totalWickets;
    public int highestScore;
    public int trophies;
}

/// <summary>
/// Local career persistence using Unity PlayerPrefs. This is device-local save data,
/// not cloud sync or a secure online account system.
/// </summary>
public class CricketCareerSaveSystem : MonoBehaviour
{
    private const string SaveKey = "RealCricket3D.CareerProfile.v1";
    [SerializeField] private CricketCareerProfile profile = new CricketCareerProfile();

    public CricketCareerProfile Profile => profile;

    private void Awake() => LoadCareer();

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        profile.playerName = value.Trim();
        SaveCareer();
    }

    public void RecordMatch(bool won, bool drawn, int runsScored, int wicketsTaken)
    {
        profile.matches++;
        if (drawn) profile.draws++;
        else if (won) profile.wins++;
        else profile.losses++;

        profile.totalRuns += Mathf.Max(0, runsScored);
        profile.totalWickets += Mathf.Max(0, wicketsTaken);
        profile.highestScore = Mathf.Max(profile.highestScore, Mathf.Max(0, runsScored));
        SaveCareer();
    }

    public void AwardTrophy()
    {
        profile.trophies++;
        SaveCareer();
    }

    public void SaveCareer()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(profile));
        PlayerPrefs.Save();
    }

    public void LoadCareer()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;
        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (string.IsNullOrEmpty(json)) return;
        CricketCareerProfile loaded = JsonUtility.FromJson<CricketCareerProfile>(json);
        if (loaded != null) profile = loaded;
    }

    public void ResetCareer()
    {
        profile = new CricketCareerProfile();
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
