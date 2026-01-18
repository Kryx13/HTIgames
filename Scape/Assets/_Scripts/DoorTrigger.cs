using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Door that opens automatically when the player enters the trigger zone.
/// Can load a new scene, teleport the player, or simply open visually.
/// </summary>
public class DoorTrigger : MonoBehaviour
{
    [Header("Door Type")]
    [SerializeField] private DoorType doorType = DoorType.SceneTransition;

    [Header("Scene Transition")]
    [SerializeField] private int targetSceneIndex = 1; // Scene index to load
    [SerializeField] private string targetSceneName = ""; // Optional: scene name

    [Header("Teleport")]
    [SerializeField] private Transform teleportDestination; // Teleport destination

    [Header("Visual Door")]
    [SerializeField] private GameObject doorModel; // 3D door model (for animation)
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private Vector3 openOffset = new Vector3(0, 3f, 0); // Movement when opened

    [Header("Conditions")]
    [SerializeField] private bool requireItem = false;
    [SerializeField] private string requiredItemName = "Amulet"; // Required item to open

    [Header("Audio")]
    [SerializeField] private AudioClip doorOpenSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool isOpening = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Inventory playerInventory;

    public enum DoorType
    {
        SceneTransition,    // Change scene
        Teleport,           // Teleport player
        VisualOnly          // Just door animation
    }

    private void Start()
    {
        // Save positions for animation
        if (doorModel != null)
        {
            closedPosition = doorModel.transform.position;
            openPosition = closedPosition + openOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (showDebugLogs)
            {
                Debug.Log($"🚪 Player touched door: {gameObject.name}");
            }

            // Check if item is required
            if (requireItem)
            {
                if (playerInventory == null)
                {
                    playerInventory = other.GetComponent<Inventory>();
                }

                if (playerInventory == null || !playerInventory.HasItem(requiredItemName))
                {
                    Debug.Log($"⚠️ Missing required item: {requiredItemName}");
                    return;
                }
            }

            // Open door according to type
            OpenDoor(other.gameObject);
        }
    }

    /// <summary>
    /// Opens the door according to configured type
    /// </summary>
    private void OpenDoor(GameObject player)
    {
        if (showDebugLogs)
        {
            Debug.Log($"✅ Door opened! Type: {doorType}");
        }

        // Door sound
        if (doorOpenSound != null)
        {
            AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
        }

        switch (doorType)
        {
            case DoorType.SceneTransition:
                LoadNextScene();
                break;

            case DoorType.Teleport:
                TeleportPlayer(player);
                break;

            case DoorType.VisualOnly:
                if (!isOpening && doorModel != null)
                {
                    StartCoroutine(AnimateDoor());
                }
                break;
        }
    }

    /// <summary>
    /// Loads the next scene
    /// </summary>
    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"🎬 Loading scene: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.Log($"🎬 Loading scene index: {targetSceneIndex}");
            SceneManager.LoadScene(targetSceneIndex);
        }
    }

    /// <summary>
    /// Teleports the player to destination
    /// </summary>
    private void TeleportPlayer(GameObject player)
    {
        if (teleportDestination != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false; // Disable to teleport
                player.transform.position = teleportDestination.position;
                player.transform.rotation = teleportDestination.rotation;
                controller.enabled = true;
                Debug.Log($"📍 Player teleported to {teleportDestination.name}");
            }
            else
            {
                player.transform.position = teleportDestination.position;
                player.transform.rotation = teleportDestination.rotation;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No teleport destination assigned!");
        }
    }

    /// <summary>
    /// Animates door opening (moves upward)
    /// </summary>
    private System.Collections.IEnumerator AnimateDoor()
    {
        isOpening = true;
        float elapsed = 0f;
        float duration = 1f / openSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            doorModel.transform.position = Vector3.Lerp(closedPosition, openPosition, t);
            yield return null;
        }

        doorModel.transform.position = openPosition;
        Debug.Log("✅ Door fully opened");
    }

    /// <summary>
    /// Gizmo to visualize trigger zone
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(transform.position + box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
            }
        }

        // Show destination if teleport
        if (doorType == DoorType.Teleport && teleportDestination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, teleportDestination.position);
            Gizmos.DrawWireSphere(teleportDestination.position, 0.5f);
        }
    }
}
