using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private EndGameUI endGameUI;

    [Header("Settings")]
    [SerializeField] private bool autoStart = true; // For testing, we start immediately

    [SerializeField] private GameObject pauseMenuObject;

    // State
    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameEnded { get; private set; } // Vrai quand le niveau est terminé (écran de fin)

    private InputManager inputManager;

    private void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object alive when changing scenes

            // Écouter les changements de scène pour réinitialiser si nécessaire
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Destroys duplicates if we reload the scene
        }
    }

    private void OnDestroy()
    {
        // Se désabonner de l'événement
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Appelé quand une nouvelle scène est chargée
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si on charge la scène de jeu (index 1), réinitialiser le timer
        if (scene.buildIndex == 1)
        {
            // Réinitialiser l'état
            CurrentTime = 0f;
            IsRunning = false;
            IsPaused = false;
            IsGameEnded = false; // Réinitialiser l'état de fin
            Time.timeScale = 1f;

            // Démarrer automatiquement si autoStart est activé
            if (autoStart)
            {
                StartTimer();
            }

            Debug.Log("🔄 GameManager réinitialisé pour nouvelle partie");
        }
        // Si on retourne au menu (index 0), arrêter le timer
        else if (scene.buildIndex == 0)
        {
            IsRunning = false;
            IsPaused = false;
            IsGameEnded = false;
            Time.timeScale = 1f;
        }
    }

    private void Start()
    {
        inputManager = InputManager.Instance;

        if (autoStart)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        // Écouter la touche Pause (Escape)
        if (inputManager != null && inputManager.PauseTriggered)
        {
            TogglePause();
        }

        if (IsRunning && !IsPaused)
        {
            // Add the time passed since last frame
            CurrentTime += Time.deltaTime;

            // Update the screen
            UpdateTimerUI();
        }
    }

    public void StartTimer()
    {
        IsRunning = true;
        CurrentTime = 0f;
        IsPaused = false;
        IsGameEnded = false; // Réinitialiser l'état de fin
        Time.timeScale = 1f; // S'assurer que le jeu n'est pas figé

        // Verrouiller le curseur pour le jeu
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StopTimer()
    {
        IsRunning = false;
        // Later: This is where we will check for High Scores
        Debug.Log($"🏁 Run Finished! Final Time: {FormatTime(CurrentTime)}");
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(CurrentTime);
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            // Pause the Game
            Time.timeScale = 0f; // Freezes physics and time
            if (pauseMenuObject != null)
            {
                pauseMenuObject.SetActive(true); // Show UI
            }
            Cursor.lockState = CursorLockMode.None; // Unlock mouse
            Cursor.visible = true;
        }
        else
        {
            // Resume the Game
            Time.timeScale = 1f; // Normal speed
            if (pauseMenuObject != null)
            {
                pauseMenuObject.SetActive(false); // Hide UI
            }
            Cursor.lockState = CursorLockMode.Locked; // Lock mouse back
            Cursor.visible = false;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // A utility to make the time look like 00:00.00
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100F) % 100F);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    public void LevelComplete()
    {
        IsRunning = false; // Arrête le timer
        IsGameEnded = true; // Le jeu est terminé - bloque les inputs du joueur

        Debug.Log($"🏁 Niveau terminé ! Temps: {FormatTime(CurrentTime)}");

        // 1. Sauvegarder le meilleur temps personnel
        float currentBest = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (CurrentTime < currentBest)
        {
            PlayerPrefs.SetFloat("BestTime", CurrentTime);
            PlayerPrefs.Save();
            Debug.Log("✅ Nouveau record personnel sauvegardé !");
        }

        // 2. Afficher l'écran de fin avec saisie du nom
        if (endGameUI != null)
        {
            Debug.Log("📋 Affichage de l'écran de fin...");
            endGameUI.ShowEndGame(CurrentTime);
        }
        else
        {
            Debug.LogError("❌ ERREUR : EndGameUI n'est PAS assigné dans le GameManager !");
            Debug.LogError("   SOLUTION :");
            Debug.LogError("   1. Créez un Panel 'EndGamePanel' dans votre Canvas");
            Debug.LogError("   2. Ajoutez-lui le script 'EndGameUI'");
            Debug.LogError("   3. Sélectionnez le GameManager dans la Hierarchy");
            Debug.LogError("   4. Glissez le EndGamePanel dans le champ 'End Game UI' de l'Inspector");
            Debug.LogWarning("⚠️ Retour au menu dans 5 secondes...");
            Invoke("LoadMenu", 5f);
        }
    }

    private void LoadMenu()
    {
        // S'assurer que le temps est réinitialisé avant de charger le menu
        Time.timeScale = 1f;
        IsPaused = false;

        SceneManager.LoadScene(0); // 0 est l'index du Menu Principal
    }
}