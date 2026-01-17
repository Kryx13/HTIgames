using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private float interactionRange = 4f;
    [SerializeField] private LayerMask interactableLayer;

    private Camera mainCam;
    private InputManager input;
    private IInteractable currentInteractable;

    private void Start()
    {
        mainCam = Camera.main;
        input = InputManager.Instance;
    }

    private void Update()
    {
        CheckForInteractable();

        if (input.InteractTriggered && currentInteractable != null)
        {
            currentInteractable.Interact(GetComponent<PlayerController>());
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                // Debug.Log($"👀 Vue sur : {interactable.InteractionPrompt}"); 
                return;
            }
        }
        currentInteractable = null;
    }
}