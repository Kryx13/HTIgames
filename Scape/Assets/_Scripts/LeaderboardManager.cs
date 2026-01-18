using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gère le système de classement (Top 10).
/// Utilise PlayerPrefs avec JSON pour sauvegarder les scores.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private const string LEADERBOARD_KEY = "Leaderboard";
    private const int MAX_ENTRIES = 10;

    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadLeaderboard();
    }

    /// <summary>
    /// Ajoute un nouveau score au leaderboard
    /// </summary>
    public void AddScore(string playerName, float time)
    {
        // Créer une nouvelle entrée
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, time);
        entries.Add(newEntry);

        // Trier par temps (le plus rapide en premier)
        entries = entries.OrderBy(e => e.time).ToList();

        // Garder seulement les 10 meilleurs
        if (entries.Count > MAX_ENTRIES)
        {
            entries = entries.Take(MAX_ENTRIES).ToList();
        }

        // Sauvegarder
        SaveLeaderboard();

        Debug.Log($"✅ Score ajouté : {playerName} - {FormatTime(time)}");
    }

    /// <summary>
    /// Retourne la liste complète des scores
    /// </summary>
    public List<LeaderboardEntry> GetLeaderboard()
    {
        return new List<LeaderboardEntry>(entries); // Copie pour éviter modifications externes
    }

    /// <summary>
    /// Vérifie si un temps est assez bon pour entrer dans le Top 10
    /// </summary>
    public bool IsTopScore(float time)
    {
        // Si moins de 10 entrées, toujours accepter
        if (entries.Count < MAX_ENTRIES)
        {
            return true;
        }

        // Vérifier si meilleur que le 10ème
        return time < entries[MAX_ENTRIES - 1].time;
    }

    /// <summary>
    /// Sauvegarde le leaderboard dans PlayerPrefs
    /// </summary>
    private void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData();
        data.entries = entries;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"💾 Leaderboard sauvegardé ({entries.Count} entrées)");
    }

    /// <summary>
    /// Charge le leaderboard depuis PlayerPrefs
    /// </summary>
    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            
            // --- SÉCURITÉ AJOUTÉE ---
            if (data != null && data.entries != null)
            {
                entries = data.entries;
            }
            else
            {
                entries = new List<LeaderboardEntry>();
            }
            // ------------------------

            Debug.Log($"📊 Leaderboard chargé ({entries.Count} entrées)");
        }
        else
        {
            Debug.Log("📊 Aucun leaderboard existant, création d'un nouveau");
            entries = new List<LeaderboardEntry>();
        }
    }

    /// <summary>
    /// Efface tout le leaderboard (utile pour debug)
    /// </summary>
    public void ClearLeaderboard()
    {
        entries.Clear();
        PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
        PlayerPrefs.Save();
        Debug.Log("🗑️ Leaderboard effacé");
    }

    /// <summary>
    /// Formate un temps en MM:SS.MS
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        int m = Mathf.FloorToInt(timeInSeconds / 60F);
        int s = Mathf.FloorToInt(timeInSeconds % 60F);
        int ms = Mathf.FloorToInt((timeInSeconds * 100F) % 100F);
        return string.Format("{0:00}:{1:00}.{2:00}", m, s, ms);
    }
}

/// <summary>
/// Wrapper pour sérialiser la liste d'entrées en JSON
/// </summary>
[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;
}
