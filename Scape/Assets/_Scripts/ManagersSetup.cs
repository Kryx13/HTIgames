using UnityEngine;

/// <summary>
/// Script de setup automatique pour créer tous les managers nécessaires au jeu.
/// Attachez ce script à un GameObject vide dans les deux scènes (Menu et Jeu).
/// </summary>
public class ManagersSetup : MonoBehaviour
{
    [Header("Auto-Create Managers")]
    [SerializeField] private bool createLeaderboardManager = true;
    [SerializeField] private bool createSettingsManager = true;

    private void Awake()
    {
        // Créer LeaderboardManager si nécessaire
        if (createLeaderboardManager && LeaderboardManager.Instance == null)
        {
            GameObject leaderboardObj = new GameObject("LeaderboardManager");
            leaderboardObj.AddComponent<LeaderboardManager>();
            Debug.Log("✅ LeaderboardManager créé automatiquement");
        }

        // Créer SettingsManager si nécessaire
        if (createSettingsManager && SettingsManager.Instance == null)
        {
            GameObject settingsObj = new GameObject("SettingsManager");
            settingsObj.AddComponent<SettingsManager>();
            Debug.Log("✅ SettingsManager créé automatiquement");
        }
    }
}
