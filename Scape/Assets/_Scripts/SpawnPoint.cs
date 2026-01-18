using UnityEngine;

/// <summary>
/// Spawn point for the player.
/// Marks the location where the player appears at the start of the level or after a respawn.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private bool isDefaultSpawn = true; // Default spawn point
    [SerializeField] private string spawnID = "Default"; // Unique spawn ID

    [Header("Player Setup")]
    [SerializeField] private GameObject playerPrefab; // Player prefab (optional)
    [SerializeField] private bool spawnOnStart = false; // Spawn the player at Start()

    [Header("Visual")]
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoSize = 1f;

    private static SpawnPoint defaultSpawn;

    private void Awake()
    {
        // Register the default spawn
        if (isDefaultSpawn && defaultSpawn == null)
        {
            defaultSpawn = this;
        }
    }

    private void Start()
    {
        // Automatic spawn if requested
        if (spawnOnStart)
        {
            SpawnPlayer();
        }
    }

    /// <summary>
    /// Teleports the existing player to this spawn point
    /// </summary>
    public void TeleportPlayerHere()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;
                controller.enabled = true;
            }
            else
            {
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;
            }

            Debug.Log($"📍 Player teleported to spawn: {spawnID}");
        }
        else
        {
            Debug.LogWarning("⚠️ No player found with tag 'Player'");
        }
    }

    /// <summary>
    /// Instantiates a new player at this spawn point (if prefab is assigned)
    /// </summary>
    public GameObject SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, transform.position, transform.rotation);
            Debug.Log($"✅ Player spawned at point: {spawnID}");
            return player;
        }
        else
        {
            Debug.LogWarning("⚠️ No Player Prefab assigned to SpawnPoint");
            return null;
        }
    }

    /// <summary>
    /// Gets the default spawn point
    /// </summary>
    public static SpawnPoint GetDefaultSpawn()
    {
        return defaultSpawn;
    }

    /// <summary>
    /// Finds a spawn point by its ID
    /// </summary>
    public static SpawnPoint FindSpawnByID(string id)
    {
        SpawnPoint[] spawns = FindObjectsOfType<SpawnPoint>();
        foreach (SpawnPoint spawn in spawns)
        {
            if (spawn.spawnID == id)
            {
                return spawn;
            }
        }
        return null;
    }

    /// <summary>
    /// Gizmo to visualize the spawn point in the editor
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Draw an arrow pointing upward
        Vector3 position = transform.position;
        Gizmos.DrawWireSphere(position, gizmoSize * 0.5f);

        // Direction where the player will look
        Vector3 forward = transform.forward * gizmoSize;
        Gizmos.DrawRay(position, forward);

        // Arrow
        Vector3 right = transform.right * gizmoSize * 0.3f;
        Gizmos.DrawRay(position + forward, -forward * 0.3f + right);
        Gizmos.DrawRay(position + forward, -forward * 0.3f - right);

        // Text (requires Handles in UnityEditor)
#if UNITY_EDITOR
        UnityEditor.Handles.color = gizmoColor;
        UnityEditor.Handles.Label(position + Vector3.up * gizmoSize, $"SPAWN: {spawnID}");
#endif
    }

    private void OnDrawGizmosSelected()
    {
        // Show a more visible area when selected
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawSphere(transform.position, gizmoSize);
    }
}
