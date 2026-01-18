using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Vérifie et instancie automatiquement le Canvas UI si absent dans la scène.
/// Ajoute ce script au GameManager ou à un objet persistant.
/// </summary>
public class CanvasSetup : MonoBehaviour
{
    [Header("Canvas Prefab")]
    [SerializeField] private GameObject canvasPrefab;

    [Header("Settings")]
    [SerializeField] private bool autoSetupInGameScenes = true;
    [SerializeField] private int menuSceneIndex = 0; // Scène du menu principal

    private static CanvasSetup instance;
    private GameObject currentCanvasInstance;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Vérifier le Canvas au démarrage
        if (SceneManager.GetActiveScene().buildIndex != menuSceneIndex)
        {
            EnsureCanvasExists();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ne pas toucher au menu principal (il a son propre Canvas)
        if (scene.buildIndex == menuSceneIndex)
        {
            Debug.Log("🎨 CanvasSetup: Scène menu - Canvas non modifié");
            return;
        }

        if (autoSetupInGameScenes)
        {
            EnsureCanvasExists();
        }
    }

    /// <summary>
    /// Vérifie si un Canvas valide existe, sinon en crée un
    /// </summary>
    public void EnsureCanvasExists()
    {
        // Chercher un Canvas existant avec les éléments nécessaires
        Canvas existingCanvas = FindValidCanvas();

        if (existingCanvas != null)
        {
            Debug.Log($"✅ CanvasSetup: Canvas valide trouvé ({existingCanvas.gameObject.name})");
            currentCanvasInstance = existingCanvas.gameObject;
            return;
        }

        // Pas de Canvas valide, en créer un
        CreateCanvas();
    }

    /// <summary>
    /// Cherche un Canvas qui contient les éléments UI nécessaires
    /// </summary>
    private Canvas FindValidCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            // Vérifier si ce Canvas contient les éléments nécessaires
            Transform pausePanel = canvas.transform.Find("PauseMenuPanel");
            Transform timerText = canvas.transform.Find("TimerText");

            // Un Canvas est valide s'il a au moins le PauseMenuPanel ou le TimerText
            if (pausePanel != null || timerText != null)
            {
                return canvas;
            }

            // Chercher aussi dans les enfants (structure imbriquée)
            if (FindChildRecursive(canvas.transform, "PauseMenuPanel") != null ||
                FindChildRecursive(canvas.transform, "TimerText") != null)
            {
                return canvas;
            }
        }

        return null;
    }

    /// <summary>
    /// Crée le Canvas à partir du prefab ou manuellement
    /// </summary>
    private void CreateCanvas()
    {
        if (canvasPrefab != null)
        {
            // Créer à partir du prefab
            currentCanvasInstance = Instantiate(canvasPrefab);
            currentCanvasInstance.name = "Canvas";
            Debug.Log("✅ CanvasSetup: Canvas créé à partir du prefab");
        }
        else
        {
            // Essayer de charger le prefab depuis Resources
            GameObject prefab = Resources.Load<GameObject>("Canvas");
            if (prefab != null)
            {
                currentCanvasInstance = Instantiate(prefab);
                currentCanvasInstance.name = "Canvas";
                Debug.Log("✅ CanvasSetup: Canvas créé depuis Resources");
            }
            else
            {
                Debug.LogError("❌ CanvasSetup: Aucun Canvas prefab assigné et aucun trouvé dans Resources!");
                Debug.LogError("   SOLUTION: Assigne le Canvas prefab au CanvasSetup dans le GameManager");
                return;
            }
        }

        // S'assurer que les panels sont dans le bon état initial
        if (currentCanvasInstance != null)
        {
            InitializeCanvasPanels();
        }
    }

    /// <summary>
    /// Initialise l'état des panels du Canvas
    /// </summary>
    private void InitializeCanvasPanels()
    {
        // Cacher le PauseMenuPanel
        Transform pausePanel = FindChildRecursive(currentCanvasInstance.transform, "PauseMenuPanel");
        if (pausePanel != null)
        {
            pausePanel.gameObject.SetActive(false);
            Debug.Log("   - PauseMenuPanel: caché");
        }

        // Cacher le EndGamePanel
        Transform endGamePanel = FindChildRecursive(currentCanvasInstance.transform, "EndGamePanel");
        if (endGamePanel != null)
        {
            endGamePanel.gameObject.SetActive(false);
            Debug.Log("   - EndGamePanel: caché");
        }

        // S'assurer que le TimerText est visible
        Transform timerText = FindChildRecursive(currentCanvasInstance.transform, "TimerText");
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            Debug.Log("   - TimerText: visible");
        }

        // Vérifier l'EventSystem
        EnsureEventSystemExists();
    }

    /// <summary>
    /// S'assure qu'un EventSystem existe dans la scène
    /// </summary>
    private void EnsureEventSystemExists()
    {
        UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            Debug.Log("✅ CanvasSetup: EventSystem créé");
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
}
