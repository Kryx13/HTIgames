using UnityEngine;
using TMPro;

/// <summary>
/// Final door slot that requires the Ancient Amulet to open.
/// Used in Stage 7: Final Room.
/// When Amulet is placed, the game is won!
/// </summary>
public class AmuletDoorSlot : MonoBehaviour
{
    [Header("Item Requirement")]
    [SerializeField] private string requiredItemName = "Ancient Amulet";

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string interactionPrompt = "Press E to place Amulet";

    [Header("Door")]
    [SerializeField] private GameObject door; // The door that opens
    [SerializeField] private float doorOpenHeight = 5f;
    [SerializeField] private float doorOpenSpeed = 2f;

    [Header("Visual Feedback")]
    [SerializeField] private Color emptySlotColor = new Color(0.3f, 0.3f, 0.4f);
    [SerializeField] private Color filledSlotColor = Color.gold;
    [SerializeField] private GameObject amuletVisual; // Visual amulet that appears

    [Header("Audio")]
    [SerializeField] private AudioClip amuletPlaceSound;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip victorySound;

    [Header("UI")]
    [SerializeField] private bool showInteractionPrompt = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Transform player;
    private Inventory playerInventory;
    private GameManager gameManager;
    private bool amuletPlaced = false;
    private bool doorOpening = false;
    private bool isPlayerNearby = false;
    private Vector3 doorStartPosition;
    private TextMeshPro promptText;
    private Renderer slotRenderer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gameManager = GameManager.Instance;

        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
        }

        slotRenderer = GetComponent<Renderer>();

        // Store door start position
        if (door != null)
        {
            doorStartPosition = door.transform.position;
        }

        // Hide amulet visual initially
        if (amuletVisual != null)
        {
            amuletVisual.SetActive(false);
        }

        // Create interaction prompt
        if (showInteractionPrompt)
        {
            CreatePromptText();
        }

        // Set slot color
        UpdateSlotVisual();

        if (showDebugLogs)
        {
            Debug.Log("🚪 Amulet Door Slot initialized");
        }
    }

    private void Update()
    {
        CheckPlayerProximity();

        // Show prompt when nearby
        if (isPlayerNearby && !amuletPlaced && promptText != null)
        {
            promptText.gameObject.SetActive(true);

            // Check for interaction
            if (Input.GetKeyDown(interactKey))
            {
                TryPlaceAmulet();
            }
        }
        else if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }

        // Animate door opening
        if (doorOpening && door != null)
        {
            AnimateDoorOpening();
        }
    }

    /// <summary>
    /// Creates interaction prompt text
    /// </summary>
    private void CreatePromptText()
    {
        GameObject textObj = new GameObject("InteractionPrompt");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = Vector3.up * 3f;

        promptText = textObj.AddComponent<TextMeshPro>();
        promptText.text = interactionPrompt;
        promptText.fontSize = 2.5f;
        promptText.color = Color.yellow;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.rectTransform.sizeDelta = new Vector2(5f, 2f);

        promptText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Checks if player is within interaction range
    /// </summary>
    private void CheckPlayerProximity()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distance <= interactionRange;
    }

    /// <summary>
    /// Attempts to place the Amulet
    /// </summary>
    private void TryPlaceAmulet()
    {
        // Check if player has inventory
        if (playerInventory == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("⚠️ Player doesn't have Inventory component!");
            }
            return;
        }

        // Check if player has the Amulet
        if (!playerInventory.HasItem(requiredItemName))
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"⚠️ Missing required item: {requiredItemName}");
                Debug.LogWarning("   → You need to find the Amulet in Stage 0 (Tutorial Room)");
            }
            return;
        }

        // Place the Amulet!
        PlaceAmulet();
    }

    /// <summary>
    /// Places the Amulet and wins the game
    /// </summary>
    private void PlaceAmulet()
    {
        amuletPlaced = true;

        if (showDebugLogs)
        {
            Debug.Log("✅ AMULET PLACED! Opening final door...");
        }

        // Remove Amulet from inventory
        if (playerInventory != null)
        {
            playerInventory.RemoveItem(requiredItemName);
        }

        // Show amulet visual
        if (amuletVisual != null)
        {
            amuletVisual.SetActive(true);
        }

        // Update slot visual
        UpdateSlotVisual();

        // Play placement sound
        if (amuletPlaceSound != null)
        {
            AudioSource.PlayClipAtPoint(amuletPlaceSound, transform.position);
        }

        // Start opening door
        StartOpeningDoor();

        // Hide prompt
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Starts the door opening animation
    /// </summary>
    private void StartOpeningDoor()
    {
        doorOpening = true;

        // Play door opening sound
        if (doorOpenSound != null)
        {
            AudioSource.PlayClipAtPoint(doorOpenSound, door.transform.position);
        }

        if (showDebugLogs)
        {
            Debug.Log("🚪 Door opening...");
        }
    }

    /// <summary>
    /// Animates the door opening (moving upward)
    /// </summary>
    private void AnimateDoorOpening()
    {
        Vector3 targetPosition = doorStartPosition + Vector3.up * doorOpenHeight;
        door.transform.position = Vector3.MoveTowards(
            door.transform.position,
            targetPosition,
            doorOpenSpeed * Time.deltaTime
        );

        // Check if door is fully open
        if (Vector3.Distance(door.transform.position, targetPosition) < 0.1f)
        {
            doorOpening = false;
            OnDoorFullyOpen();
        }
    }

    /// <summary>
    /// Called when door is fully open
    /// </summary>
    private void OnDoorFullyOpen()
    {
        if (showDebugLogs)
        {
            Debug.Log("🎉 DOOR FULLY OPEN! GAME WON!");
        }

        // Play victory sound
        if (victorySound != null)
        {
            AudioSource.PlayClipAtPoint(victorySound, transform.position);
        }

        // Trigger game completion
        if (gameManager != null)
        {
            gameManager.LevelComplete();
        }
        else
        {
            Debug.LogWarning("GameManager not found! Cannot complete game.");
        }
    }

    /// <summary>
    /// Updates the slot's visual appearance
    /// </summary>
    private void UpdateSlotVisual()
    {
        if (slotRenderer != null)
        {
            slotRenderer.material.color = amuletPlaced ? filledSlotColor : emptySlotColor;
        }
    }

    /// <summary>
    /// Gizmo to show interaction range
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = amuletPlaced ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.gold;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 4f,
            $"AMULET DOOR SLOT\n{(amuletPlaced ? "FILLED" : "EMPTY")}");

        // Draw line to door
        if (door != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, door.transform.position);
        }
#endif
    }

    /// <summary>
    /// Manual test: place amulet without checking inventory
    /// </summary>
    [ContextMenu("Force Place Amulet")]
    public void ForcePlaceAmulet()
    {
        PlaceAmulet();
    }
}
