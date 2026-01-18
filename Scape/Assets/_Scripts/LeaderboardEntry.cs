using System;

/// <summary>
/// Représente une entrée du leaderboard (un score).
/// Doit être Serializable pour être sauvegardé dans PlayerPrefs.
/// </summary>
[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;

    public LeaderboardEntry(string name, float timeValue)
    {
        playerName = name;
        time = timeValue;
    }
}
