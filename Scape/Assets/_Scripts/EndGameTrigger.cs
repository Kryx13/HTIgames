using UnityEngine;

/// <summary>
/// Zone de victoire - Déclenche la fin du niveau quand le joueur entre.
/// À attacher sur un GameObject avec un Collider en mode Trigger.
/// </summary>
public class EndGameTrigger : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void Start()
    {
        // Vérifier que le collider est en trigger mode
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("❌ EndGameTrigger: Aucun Collider trouvé ! Ajoutez un Collider à cet objet.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("⚠️ EndGameTrigger: Le Collider n'est PAS en mode Trigger ! Activez 'Is Trigger' dans l'Inspector.");
            col.isTrigger = true; // Auto-correction
            Debug.Log("✅ Collider automatiquement configuré en Trigger");
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log($"✅ EndGameTrigger activé sur {gameObject.name}");
            }
        }

        // Vérifier que le GameManager existe
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ EndGameTrigger: GameManager.Instance est NULL ! Assurez-vous qu'un GameManager existe dans la scène.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (showDebugLogs)
        {
            Debug.Log($"🔔 EndGameTrigger: Collision détectée avec {other.gameObject.name} (Tag: {other.tag})");
        }

        // Si c'est le joueur qui touche la zone
        if (other.CompareTag("Player"))
        {
            Debug.Log("🏁 JOUEUR DÉTECTÉ DANS LA WIN ZONE !");

            // Vérifier que le GameManager existe
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LevelComplete();
            }
            else
            {
                Debug.LogError("❌ GameManager.Instance est NULL ! Impossible de terminer le niveau.");
            }
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log($"⚠️ L'objet {other.gameObject.name} n'a pas le tag 'Player'");
            }
        }
    }

    // Afficher la zone dans l'éditeur
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Vert transparent
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.DrawCube(transform.position, col.bounds.size);
        }
    }
}