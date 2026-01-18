using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Mouvements")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float gravityValue = -15f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Caméra (Souris)")]
    [SerializeField] private Transform cameraRoot; // L'objet vide qu'on a créé sur le joueur
    [SerializeField] private float topClamp = -40f; // Limite haut
    [SerializeField] private float bottomClamp = 70f; // Limite bas

    private CharacterController controller;
    private InputManager input;
    private SettingsManager settings;
    private GameManager gameManager;
    private Transform mainCamera;
    private float currentSensitivity = 2f; // Valeur par défaut si SettingsManager absent
    
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float targetRotation; // Pour l'orientation du joueur
    private float rotationVelocity;
    
    // Variables pour la rotation caméra
    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;

    private void Start()
    {
        // Récupération des références
        controller = GetComponent<CharacterController>();
        input = InputManager.Instance;
        settings = SettingsManager.Instance;
        gameManager = GameManager.Instance;
        mainCamera = Camera.main.transform;

        // Récupérer la sensibilité depuis les paramètres
        if (settings != null)
        {
            currentSensitivity = settings.MouseSensitivity;
        }

        // Si tu as oublié d'assigner le CameraRoot dans l'inspecteur, on cherche un enfant
        if (cameraRoot == null)
            cameraRoot = transform.Find("CameraRoot");

        // Cache la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Ne rien faire si le jeu est en pause OU terminé
        if (gameManager != null && (gameManager.IsPaused || gameManager.IsGameEnded))
        {
            return;
        }

        // 1. Rotation de la caméra (Souris)
        HandleCameraRotation();

        // 2. Mouvement du joueur (Clavier)
        HandleMovement();
    }

    private void HandleCameraRotation()
    {
        // Récupérer la sensibilité actuelle depuis les paramètres
        if (settings != null)
        {
            currentSensitivity = settings.MouseSensitivity;
        }

        // On vérifie que l'input existe (si la souris bouge)
        if (input.LookInput.sqrMagnitude >= 0.01f)
        {
            // On multiplie par la sensibilité (maintenant dynamique)
            cinemachineTargetYaw += input.LookInput.x * currentSensitivity;
            cinemachineTargetPitch += input.LookInput.y * currentSensitivity;
            // Note : Si c'est inversé haut/bas, mets un "-" devant input.LookInput.y
        }

        // On limite l'angle haut/bas pour ne pas se tordre le cou
        cinemachineTargetPitch = Mathf.Clamp(cinemachineTargetPitch, topClamp, bottomClamp);

        // On applique la rotation au CameraRoot (et au joueur pour la gauche/droite si on veut)
        // Ici, on tourne le Root pour la hauteur et l'orientation
        cameraRoot.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
    }

    private void HandleMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        Vector2 movement = input.MoveInput;
        
        // Si on appuie sur une touche
        if (movement.sqrMagnitude >= 0.1f)
        {
            // On calcule l'angle vers lequel on veut aller par rapport à la caméra
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            
            // Lissage de la rotation du personnage
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            float targetSpeed = input.RunHeld ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * targetSpeed * Time.deltaTime);
        }

        // Gravité / Saut
        if (input.JumpTriggered && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(1.5f * -2f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}