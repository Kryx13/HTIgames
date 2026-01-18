using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderboardPanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitToMenuButton;
    [SerializeField] private Button quitGameButton;

    private SettingsUI settingsUI;
    private LeaderboardUI leaderboardUI;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsUI = settingsPanel.GetComponent<SettingsUI>();
            settingsPanel.SetActive(false);
        }

        if (leaderboardPanel != null)
        {
            leaderboardUI = leaderboardPanel.GetComponent<LeaderboardUI>();
            leaderboardPanel.SetActive(false);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(Resume);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OpenSettings);
        }

        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.RemoveAllListeners();
            leaderboardButton.onClick.AddListener(OpenLeaderboard);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(Restart);
        }

        if (quitToMenuButton != null)
        {
            quitToMenuButton.onClick.RemoveAllListeners();
            quitToMenuButton.onClick.AddListener(QuitToMenu);
        }

        if (quitGameButton != null)
        {
            quitGameButton.onClick.RemoveAllListeners();
            quitGameButton.onClick.AddListener(QuitGame);
        }
    }

    public void Resume()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }

    public void OpenSettings()
    {
        if (settingsUI != null)
        {
            settingsUI.OpenSettings(pauseMenuPanel);
        }
        else if (settingsPanel != null)
        {
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }
    }

    public void OpenLeaderboard()
    {
        if (leaderboardUI != null)
        {
            leaderboardUI.OpenLeaderboard(pauseMenuPanel);
        }
        else if (leaderboardPanel != null)
        {
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            leaderboardPanel.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}