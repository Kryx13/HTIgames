using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameControls gameControls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool RunHeld { get; private set; }

    public bool InteractTriggered { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionnel : garde l'InputManager entre les scènes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialisation des GameControls
        InitializeControls();
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
    }

    private void OnDestroy()
    {
        if (gameControls != null)
        {
            gameControls.Dispose();
        }
    }
}