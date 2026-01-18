using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contrôle le menu Pause.
/// Gère les boutons Resume, Settings, Leaderboard, Restart, Quit.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderboardPanel;

    private GameManager gameManager;
    private SettingsUI settingsUI;
    private LeaderboardUI leaderboardUI;

    private void Start()
    {
        gameManager = GameManager.Instance;

        // Récupérer automatiquement les scripts depuis les panels
        if (settingsPanel != null)
        {
            settingsUI = settingsPanel.GetComponent<SettingsUI>();
            if (settingsUI == null)
            {
                Debug.LogWarning("⚠️ SettingsUI non trouvé sur le SettingsPanel !");
            }
        }

        if (leaderboardPanel != null)
        {
            leaderboardUI = leaderboardPanel.GetComponent<LeaderboardUI>();
            if (leaderboardUI == null)
            {
                Debug.LogWarning("⚠️ LeaderboardUI non trouvé sur le LeaderboardPanel !");
            }
        }
    }

    /// <summary>
    /// Reprend le jeu (ferme le menu pause)
    /// </summary>
    public void Resume()
    {
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
        else
        {
            Debug.LogWarning("⚠️ GameManager non trouvé pour Resume");
        }
    }

    /// <summary>
    /// Ouvre le menu Settings
    /// </summary>
    public void OpenSettings()
    {
        if (settingsUI != null)
        {
            // Utiliser le script SettingsUI si disponible
            settingsUI.OpenSettings(pauseMenuPanel);
        }
        else if (settingsPanel != null)
        {
            // Sinon, simplement afficher/cacher les panels
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ SettingsPanel non assigné ! Assignez-le dans l'Inspector du PauseMenuController.");
        }
    }

    /// <summary>
    /// Ouvre le Leaderboard
    /// </summary>
    public void OpenLeaderboard()
    {
        if (leaderboardUI != null)
        {
            // Utiliser le script LeaderboardUI si disponible
            leaderboardUI.OpenLeaderboard(pauseMenuPanel);
        }
        else if (leaderboardPanel != null)
        {
            // Sinon, simplement afficher/cacher les panels
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
            leaderboardPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ LeaderboardPanel non assigné ! Assignez-le dans l'Inspector du PauseMenuController.");
        }
    }

    /// <summary>
    /// Recommence le niveau
    /// </summary>
    public void Restart()
    {
        // Réinitialiser le temps
        Time.timeScale = 1f;

        // Recharger la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("🔄 Niveau redémarré");
    }

    /// <summary>
    /// Retourne au menu principal
    /// </summary>
    public void QuitToMenu()
    {
        // Réinitialiser le temps
        Time.timeScale = 1f;

        // Charger le menu principal (scène 0)
        SceneManager.LoadScene(0);

        Debug.Log("📤 Retour au menu principal");
    }

    /// <summary>
    /// Quitte le jeu (fonctionne uniquement en build, pas dans l'éditeur)
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("🚪 Quitter le jeu");
        Application.Quit();

#if UNITY_EDITOR
        // Arrêter le play mode dans l'éditeur
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
