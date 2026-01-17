using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private float interactionRange = 4f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private bool showDebugRay = true;

    private Camera mainCam;
    private InputManager input;
    private IInteractable currentInteractable;

    private void Start()
    {
        mainCam = Camera.main;
        input = InputManager.Instance;

        // Vérification de la caméra
        if (mainCam == null)
        {
            Debug.LogError("❌ Aucune Main Camera trouvée ! Assurez-vous qu'une caméra a le tag 'MainCamera'");
        }

        // Vérification de l'InputManager
        if (input == null)
        {
            Debug.LogError("❌ InputManager introuvable ! Assurez-vous qu'un objet GameManager avec InputManager existe dans la scène");
        }

        // Vérification du LayerMask
        if (interactableLayer == 0)
        {
            Debug.LogWarning("⚠️ Aucun layer assigné à 'Interactable Layer' dans l'Inspector ! Assignez le layer 'Interactable' (Layer 6)");
        }
    }

    private void Update()
    {
        CheckForInteractable();

        if (input != null && input.InteractTriggered && currentInteractable != null)
        {
            Debug.Log($"🔑 Interaction avec : {currentInteractable.InteractionPrompt}");
            currentInteractable.Interact(GetComponent<PlayerController>());
        }
    }

    private void CheckForInteractable()
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Debug visuel du raycast
        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionRange,
                currentInteractable != null ? Color.green : Color.red);
        }

        // IMPORTANT : QueryTriggerInteraction.Collide pour détecter les triggers !
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer, QueryTriggerInteraction.Collide))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    Debug.Log($"👀 Vue sur : {interactable.InteractionPrompt}");
                }
                currentInteractable = interactable;
                return;
            }
        }
        currentInteractable = null;
    }
}