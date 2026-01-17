using UnityEngine;

/// <summary>
/// Vérifie que le Player a tous les composants nécessaires pour le ramassage automatique.
/// Le système OnTriggerEnter nécessite :
/// - Un Rigidbody sur au moins un des deux objets
/// - Un Collider normal (non-trigger) sur le Player
/// - Un Collider trigger sur les pickups
/// </summary>
public class PlayerSetupChecker : MonoBehaviour
{
    private void Start()
    {
        CheckPlayerSetup();
    }

    [ContextMenu("Check Player Setup")]
    private void CheckPlayerSetup()
    {
        Debug.Log("=== 🔍 VÉRIFICATION CONFIGURATION PLAYER ===\n");

        // 1. Vérifier le Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ ERREUR : Le Player n'a pas de Rigidbody !");
            Debug.LogError("   → Ajoutez un Rigidbody pour que OnTriggerEnter fonctionne");
            Debug.LogError("   → Cochez 'Is Kinematic' si vous utilisez un CharacterController");
        }
        else
        {
            Debug.Log($"✅ Rigidbody présent (Kinematic: {rb.isKinematic})");
            if (!rb.isKinematic)
            {
                Debug.LogWarning("⚠️ Le Rigidbody n'est pas Kinematic. Si vous utilisez CharacterController, cochez 'Is Kinematic'");
            }
        }

        // 2. Vérifier le Collider
        Collider col = GetComponent<Collider>();
        CharacterController charController = GetComponent<CharacterController>();

        if (col == null && charController == null)
        {
            Debug.LogError("❌ ERREUR : Le Player n'a ni Collider ni CharacterController !");
            Debug.LogError("   → Ajoutez un CharacterController ou un Collider");
        }
        else if (charController != null)
        {
            Debug.Log($"✅ CharacterController présent (Radius: {charController.radius}, Height: {charController.height})");
            Debug.Log("   Note : CharacterController agit comme un collider pour OnTriggerEnter");
        }
        else if (col != null)
        {
            Debug.Log($"✅ Collider présent : {col.GetType().Name}");
            if (col.isTrigger)
            {
                Debug.LogWarning("⚠️ Le Collider du Player est en mode Trigger. Cela devrait être désactivé normalement.");
            }
        }

        // 3. Vérifier l'Inventory
        Inventory inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("❌ ERREUR : Le Player n'a pas de composant Inventory !");
        }
        else
        {
            Debug.Log($"✅ Inventory présent");
        }

        // 4. Vérifier le Layer
        Debug.Log($"Layer du Player : {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)})");
        if (gameObject.layer == 6)
        {
            Debug.LogError("❌ ERREUR : Le Player est sur le layer 'Interactable' (6) !");
            Debug.LogError("   → Changez le layer en 'Player' (3)");
        }

        Debug.Log("\n=== FIN VÉRIFICATION ===");
    }
}
