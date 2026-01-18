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

    [Header("Canvas Setup")]
    [SerializeField] private GameObject canvasPrefab; // Assigner le Canvas prefab ici

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
        Debug.Log($"🎬 OnSceneLoaded appelé - Scène: {scene.name} (index {scene.buildIndex})");

        // Si on charge une scène de jeu (index >= 1)
        if (scene.buildIndex >= 1)
        {
            // Trouver les références UI automatiquement
            FindUIReferences();

            // Réinitialiser l'état de pause/fin
            IsPaused = false;
            IsGameEnded = false;
            Time.timeScale = 1f;

            // Ne réinitialiser le timer QUE si on vient du menu (timer pas en cours)
            // Si le timer est déjà en cours (mort, restart, changement de stage), on continue
            Debug.Log($"🔍 État actuel: IsRunning={IsRunning}, IsPaused={IsPaused}, IsGameEnded={IsGameEnded}, autoStart={autoStart}");

            if (!IsRunning)
            {
                CurrentTime = 0f;

                // Effacer l'inventaire sauvegardé pour une nouvelle partie (venant du menu)
                Inventory.ClearSavedInventory();
                Debug.Log("🎒 Inventaire effacé pour nouvelle partie");

                if (autoStart)
                {
                    Debug.Log("⏱️ Appel de StartTimer()...");
                    StartTimer();
                    Debug.Log($"⏱️ Après StartTimer(): IsRunning={IsRunning}");
                }
                else
                {
                    Debug.LogWarning("⚠️ autoStart est désactivé - le timer ne démarre pas automatiquement");
                }
                Debug.Log("🔄 Nouvelle partie - Timer démarré");
            }
            else
            {
                // Le timer continue (restart, mort, changement de stage)
                Debug.Log($"➡️ Stage {scene.buildIndex} chargé - Timer continue: {FormatTime(CurrentTime)}");
            }
        }
        // Si on retourne au menu (index 0), arrêter et reset le timer
        else if (scene.buildIndex == 0)
        {
            IsRunning = false;
            IsPaused = false;
            IsGameEnded = false;
            CurrentTime = 0f; // Reset pour la prochaine partie
            Time.timeScale = 1f;

            // IMPORTANT: Déverrouiller le curseur pour le menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("🏠 Retour au menu - Timer réinitialisé, curseur déverrouillé");
        }
    }

    /// <summary>
    /// Trouve automatiquement les références UI dans la scène
    /// </summary>
    private void FindUIReferences()
    {
        // D'abord, s'assurer qu'un Canvas valide existe
        EnsureCanvasExists();

        // Trouver le TimerText
        GameObject timerObj = GameObject.Find("TimerText");
        if (timerObj != null)
        {
            timerText = timerObj.GetComponent<TextMeshProUGUI>();
            if (timerText != null)
                Debug.Log("✅ TimerText trouvé automatiquement");
        }
        else
        {
            Debug.LogWarning("⚠️ TimerText non trouvé dans la scène");
        }

        // Trouver le PauseMenuPanel
        pauseMenuObject = GameObject.Find("PauseMenuPanel");
        if (pauseMenuObject != null)
        {
            pauseMenuObject.SetActive(false); // S'assurer qu'il est caché au départ
            Debug.Log("✅ PauseMenuPanel trouvé automatiquement");
        }
        else
        {
            Debug.LogWarning("⚠️ PauseMenuPanel non trouvé dans la scène");
        }

        // Trouver le EndGameUI
        endGameUI = FindObjectOfType<EndGameUI>(true); // true pour inclure les objets inactifs
        if (endGameUI != null)
        {
            Debug.Log("✅ EndGameUI trouvé automatiquement");
        }
        else
        {
            Debug.LogWarning("⚠️ EndGameUI non trouvé dans la scène");
        }

        // S'assurer qu'un EventSystem existe
        EnsureEventSystemExists();
    }

    /// <summary>
    /// Vérifie et crée le Canvas si nécessaire
    /// </summary>
    private void EnsureCanvasExists()
    {
        // Chercher un Canvas avec les éléments nécessaires
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        bool validCanvasFound = false;

        foreach (Canvas canvas in canvases)
        {
            // Vérifier si ce Canvas contient PauseMenuPanel ou TimerText
            if (canvas.transform.Find("PauseMenuPanel") != null ||
                canvas.transform.Find("TimerText") != null ||
                FindChildRecursive(canvas.transform, "PauseMenuPanel") != null ||
                FindChildRecursive(canvas.transform, "TimerText") != null)
            {
                validCanvasFound = true;
                Debug.Log($"✅ Canvas valide trouvé: {canvas.gameObject.name}");
                break;
            }
        }

        if (!validCanvasFound)
        {
            Debug.LogWarning("⚠️ Aucun Canvas valide trouvé - tentative de création...");

            if (canvasPrefab != null)
            {
                GameObject newCanvas = Instantiate(canvasPrefab);
                newCanvas.name = "Canvas";
                Debug.Log("✅ Canvas créé à partir du prefab!");

                // Initialiser les panels
                Transform pausePanel = FindChildRecursive(newCanvas.transform, "PauseMenuPanel");
                if (pausePanel != null) pausePanel.gameObject.SetActive(false);

                Transform endPanel = FindChildRecursive(newCanvas.transform, "EndGamePanel");
                if (endPanel != null) endPanel.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("❌ Canvas prefab non assigné dans GameManager!");
                Debug.LogError("   SOLUTION: Dans Unity, sélectionne le GameManager prefab");
                Debug.LogError("   et assigne le Canvas prefab dans le champ 'Canvas Prefab'");
            }
        }
    }

    /// <summary>
    /// S'assure qu'un EventSystem existe
    /// </summary>
    private void EnsureEventSystemExists()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            Debug.Log("✅ EventSystem créé automatiquement");
        }
    }

    /// <summary>
    /// Recherche récursive d'un enfant par nom
    /// </summary>
    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;

        // Trouver les références UI si on démarre directement dans une scène de jeu
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            FindUIReferences();
        }

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
        // Ne pas réinitialiser le temps si le timer est déjà en cours (changement de stage)
        if (!IsRunning)
        {
            CurrentTime = 0f;
        }

        IsRunning = true;
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