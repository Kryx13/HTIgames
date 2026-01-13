using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private GameControls gameControls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; } // Nouveau
    public bool JumpTriggered { get; private set; }
    public bool RunHeld { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameControls = new GameControls();
        
        // On s'assure que tout est activé
        gameControls.Player.Enable();
    }

    private void OnEnable()
    {
        gameControls.Player.Enable();
    }

    private void OnDisable()
    {
        gameControls.Player.Disable();
    }

    private void Update()
    {
        MoveInput = gameControls.Player.Move.ReadValue<Vector2>();
        LookInput = gameControls.Player.Look.ReadValue<Vector2>(); // Nouveau
        JumpTriggered = gameControls.Player.Jump.WasPressedThisFrame();
        RunHeld = gameControls.Player.Run.IsPressed();
    }
}