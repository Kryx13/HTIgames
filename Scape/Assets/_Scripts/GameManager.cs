using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Nécessaire pour manipuler les objets UI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject pauseMenuObject;
    [SerializeField] private GameObject parametersMenuObject; // Nouveau
    [SerializeField] private GameObject leaderBoardObject;    // Nouveau
    [SerializeField] private GameObject endGamePanelObject;   // L'objet Panel lui-même
    private EndGameUI endGameScript;                          // Le script attaché au panel

    [Header("Settings")]
    [SerializeField] private bool autoStart = true;

    // State
    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameEnded { get; private set; }

    private InputManager inputManager;

    private void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // S'abonner au chargement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Se désabonner
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;

        // Si on lance le jeu direct dans le niveau 1
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            FindUIReferences();
            if (autoStart) StartTimer();
        }
    }

    private void Update()
    {
        // Sécurité : Si les références sont perdues (cas rares), on recheck
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            if (timerText == null || pauseMenuObject == null)
            {
                FindUIReferences();
            }
        }

        // Gestion de la Pause (Touche Echap)
        if (inputManager != null && inputManager.PauseTriggered)
        {
            // On interdit la pause si le jeu est fini
            if (!IsGameEnded)
            {
                TogglePause();
            }
        }

        // Gestion du Timer
        if (IsRunning && !IsPaused)
        {
            CurrentTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    /// <summary>
    /// Appelé automatiquement quand une scène change
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🎬 Scène chargée : {scene.name}");

        if (scene.buildIndex >= 1) // Scène de JEU
        {
            FindUIReferences();
            
            IsPaused = false;
            IsGameEnded = false;
            Time.timeScale = 1f;

            // Si c'est une nouvelle partie (venant du menu), on reset
            if (!IsRunning)
            {
                CurrentTime = 0f;
                // Inventory.ClearSavedInventory(); // Décommente si tu as l'inventaire
                if (autoStart) StartTimer();
            }
        }
        else // MENU PRINCIPAL (Index 0)
        {
            IsRunning = false;
            IsPaused = false;
            IsGameEnded = false;
            Time.timeScale = 1f;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// La fonction qui répare les liens brisés avec l'UI
    /// </summary>
    private void FindUIReferences()
    {
        // 1. Chercher le Canvas
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            Transform t = canvas.transform;

            // 2. Chercher les enfants (Même désactivés)
            // Note: Transform.Find cherche les enfants directs. 
            // Si tes panels sont dans des sous-dossiers, il faudra adapter.
            Transform pauseRef = t.Find("PauseMenuPanel");
            Transform paramRef = t.Find("ParametersMenuPanel");
            Transform leaderRef = t.Find("LeaderBoardPanel");
            Transform endRef = t.Find("EndGamePanel");
            Transform timerRef = t.Find("TimerText"); 

            // 3. Assigner les variables
            if (pauseRef) pauseMenuObject = pauseRef.gameObject;
            if (paramRef) parametersMenuObject = paramRef.gameObject;
            if (leaderRef) leaderBoardObject = leaderRef.gameObject;
            
            if (endRef) 
            {
                endGamePanelObject = endRef.gameObject;
                endGameScript = endGamePanelObject.GetComponent<EndGameUI>();
            }

            if (timerRef) timerText = timerRef.GetComponent<TextMeshProUGUI>();

            // 4. Cacher les menus au démarrage
            if (pauseMenuObject) pauseMenuObject.SetActive(false);
            if (parametersMenuObject) parametersMenuObject.SetActive(false);
            if (leaderBoardObject) leaderBoardObject.SetActive(false);
            if (endGamePanelObject) endGamePanelObject.SetActive(false);

            Debug.Log("✅ UI reconnectée avec succès !");
        }
        else
        {
            // Pas de Canvas trouvé (peut arriver dans le MainMenu, ce n'est pas grave)
            if(SceneManager.GetActiveScene().buildIndex >= 1)
                Debug.LogWarning("⚠️ Attention : Pas d'objet 'Canvas' trouvé dans cette scène.");
        }
    }

    public void StartTimer()
    {
        if (!IsRunning) CurrentTime = 0f;
        IsRunning = true;
        IsPaused = false;
        IsGameEnded = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = FormatTime(CurrentTime);
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuObject != null) pauseMenuObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            
            // On cache tous les menus potentiellement ouverts
            if (pauseMenuObject) pauseMenuObject.SetActive(false);
            if (parametersMenuObject) parametersMenuObject.SetActive(false);
            if (leaderBoardObject) leaderBoardObject.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void LevelComplete()
    {
        IsRunning = false;
        IsGameEnded = true;

        // Sauvegarde du score
        float currentBest = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (CurrentTime < currentBest)
        {
            PlayerPrefs.SetFloat("BestTime", CurrentTime);
            PlayerPrefs.Save();
        }

        // Afficher l'écran de fin
        if (endGameScript != null)
        {
            // On active l'objet parent
            if(endGamePanelObject) endGamePanelObject.SetActive(true);
            
            // On appelle la fonction du script
            endGameScript.ShowEndGame(CurrentTime);
            
            // On libère la souris
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogError("❌ Pas de script EndGameUI trouvé ! Retour menu force.");
            Invoke("LoadMenu", 3f);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    private void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private string FormatTime(float timeInSeconds)
    {
        int m = Mathf.FloorToInt(timeInSeconds / 60F);
        int s = Mathf.FloorToInt(timeInSeconds % 60F);
        int ms = Mathf.FloorToInt((timeInSeconds * 100F) % 100F);
        return string.Format("{0:00}:{1:00}.{2:00}", m, s, ms);
    }
}