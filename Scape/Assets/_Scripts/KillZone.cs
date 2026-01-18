using UnityEngine;

/// <summary>
/// Death zone that respawns the player at a spawn point.
/// Used for fatal falls, traps, etc.
/// </summary>
public class KillZone : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private SpawnPoint customRespawnPoint; // Specific respawn point
    [SerializeField] private bool useDefaultSpawn = true; // Use the default spawn if no custom one

    [Header("Effects")]
    [SerializeField] private bool showRespawnMessage = true;
    [SerializeField] private string respawnMessage = "💀 You fell! Respawning...";

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;

    [Header("Delay")]
    [SerializeField] private float respawnDelay = 0.5f; // Delay before respawn (in seconds)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("💀 Player in KillZone!");

            if (showRespawnMessage)
            {
                Debug.Log(respawnMessage);
            }

            // Death sound
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, other.transform.position);
            }

            // Respawn with delay
            StartCoroutine(RespawnPlayer(other.gameObject));
        }
    }

    /// <summary>
    /// Respawns the player after a short delay
    /// </summary>
    private System.Collections.IEnumerator RespawnPlayer(GameObject player)
    {
        // Wait for the delay
        yield return new WaitForSeconds(respawnDelay);

        // Determine the respawn point
        SpawnPoint respawnPoint = null;

        if (customRespawnPoint != null)
        {
            respawnPoint = customRespawnPoint;
        }
        else if (useDefaultSpawn)
        {
            respawnPoint = SpawnPoint.GetDefaultSpawn();
        }

        // Teleport the player
        if (respawnPoint != null)
        {
            respawnPoint.TeleportPlayerHere();
            Debug.Log("✅ Player respawned!");
        }
        else
        {
            Debug.LogError("❌ No SpawnPoint found for respawn!");

            // Fallback: teleport to (0, 5, 0)
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = new Vector3(0, 5, 0);
                controller.enabled = true;
            }
            else
            {
                player.transform.position = new Vector3(0, 5, 0);
            }
        }
    }

    /// <summary>
    /// Visualization of the kill zone in the editor
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Transparent red

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Show a connection to the respawn point
        if (customRespawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, customRespawnPoint.transform.position);
        }
    }
}
