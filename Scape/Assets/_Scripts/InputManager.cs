using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameControls gameControls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool RunHeld { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool PauseTriggered { get; private set; }

    private void Awake()
    {
        Debug.Log($"🎮 InputManager.Awake() - Instance actuelle: {(Instance != null ? Instance.gameObject.name : "NULL")}");

        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"✅ InputManager initialisé sur {gameObject.name} (DontDestroyOnLoad)");
        }
        else
        {
            Debug.Log($"⚠️ InputManager déjà existant - destruction de {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        // Initialisation des GameControls
        InitializeControls();

        // S'abonner aux changements de scène pour réactiver les contrôles
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🎮 InputManager.OnSceneLoaded - Scène: {scene.name}, Réactivation des contrôles...");

        // Réactiver les contrôles après chargement de scène
        if (gameControls != null)
        {
            gameControls.Player.Enable();
            Debug.Log("✅ Contrôles réactivés");
        }
        else
        {
            Debug.LogWarning("⚠️ gameControls est null, réinitialisation...");
            InitializeControls();
        }
    }

    private void InitializeControls()
    {
        if (gameControls == null)
        {
            gameControls = new GameControls();
            Debug.Log("✅ InputManager : GameControls initialisé");
        }

        // Activer les inputs
        gameControls.Player.Enable();
    }

    private void OnEnable()
    {
        if (gameControls != null)
        {
            gameControls.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (gameControls != null)
        {
            gameControls.Player.Disable();
        }
    }

    private void Update()
    {
        // Protection contre les accès null
        if (gameControls == null)
        {
            Debug.LogError("❌ InputManager : gameControls est null ! Réinitialisation...");
            InitializeControls();
            return;
        }

        MoveInput = gameControls.Player.Move.ReadValue<Vector2>();
        LookInput = gameControls.Player.Look.ReadValue<Vector2>();
        JumpTriggered = gameControls.Player.Jump.WasPressedThisFrame();
        RunHeld = gameControls.Player.Run.IsPressed();
        InteractTriggered = gameControls.Player.Interact.WasPressedThisFrame();
        PauseTriggered = gameControls.Player.Pause.WasPressedThisFrame();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (gameControls != null)
        {
            gameControls.Dispose();
        }
    }
}