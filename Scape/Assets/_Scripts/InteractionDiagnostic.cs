using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script de diagnostic pour le système d'interaction.
/// Attachez-le au joueur temporairement pour identifier les problèmes.
/// </summary>
public class InteractionDiagnostic : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float checkRange = 4f;

    private void Update()
    {
        // Utilise le nouveau Input System (touche F1)
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            RunDiagnostic();
        }
    }

    [ContextMenu("Run Full Diagnostic")]
    public void RunDiagnostic()
    {
        Debug.Log("=== 🔍 DIAGNOSTIC SYSTÈME D'INTERACTION ===\n");

        // 1. Vérifier la caméra
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("❌ ERREUR CRITIQUE : Aucune Main Camera trouvée !");
            Debug.LogError("   → Assurez-vous qu'une caméra a le tag 'MainCamera'");
            return;
        }
        else
        {
            Debug.Log($"✅ Main Camera trouvée : {cam.name}");
        }

        // 2. Vérifier InputManager
        InputManager input = InputManager.Instance;
        if (input == null)
        {
            Debug.LogError("❌ ERREUR CRITIQUE : InputManager.Instance est null !");
            Debug.LogError("   → Vérifiez qu'un GameObject avec InputManager existe dans la scène");
            return;
        }
        else
        {
            Debug.Log($"✅ InputManager trouvé");
        }

        // 3. Vérifier le composant Interactor
        Interactor interactor = GetComponent<Interactor>();
        if (interactor == null)
        {
            Debug.LogError("❌ ERREUR : Pas de composant Interactor sur le joueur !");
            return;
        }
        else
        {
            Debug.Log($"✅ Composant Interactor présent");
        }

        // 4. Vérifier l'inventaire
        Inventory inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("❌ ERREUR : Pas de composant Inventory sur le joueur !");
            return;
        }
        else
        {
            Debug.Log($"✅ Composant Inventory présent ({inventory.items.Count}/{inventory.items.Capacity} items)");
        }

        // 5. Test de raycast manuel
        Debug.Log("\n--- Test Raycast ---");
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Test SANS LayerMask (AVEC détection des triggers)
        if (Physics.Raycast(ray, out hit, checkRange, ~0, QueryTriggerInteraction.Collide))
        {
            Debug.Log($"✅ Raycast touche quelque chose : {hit.collider.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");

            // Vérifier si c'est un interactable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log($"   ✅ C'est un IInteractable : {interactable.InteractionPrompt}");
            }
            else
            {
                Debug.LogWarning($"   ⚠️ Pas de composant IInteractable sur {hit.collider.name}");
            }

            // Vérifier le layer
            if (hit.collider.gameObject.layer == 6)
            {
                Debug.Log($"   ✅ L'objet est sur le Layer 6 (Interactable)");
            }
            else
            {
                Debug.LogWarning($"   ⚠️ L'objet est sur le Layer {hit.collider.gameObject.layer} ({LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                Debug.LogWarning($"   → Changez le layer en 'Interactable' (6)");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun objet détecté devant la caméra (portée: " + checkRange + "m)");
            Debug.LogWarning("   → Approchez-vous d'un objet pickup et relancez le diagnostic");
        }

        // 6. Test avec LayerMask pour Layer 6 (AVEC détection des triggers)
        Debug.Log("\n--- Test Raycast avec LayerMask (Layer 6) ---");
        LayerMask interactableLayer = 1 << 6; // Layer 6
        if (Physics.Raycast(ray, out hit, checkRange, interactableLayer, QueryTriggerInteraction.Collide))
        {
            Debug.Log($"✅ Raycast avec LayerMask détecte : {hit.collider.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ Raycast avec LayerMask ne détecte rien");
            Debug.LogWarning("   → Vérifiez que l'objet est sur le Layer 6 (Interactable)");
        }

        // 7. Lister tous les ItemPickup dans la scène
        Debug.Log("\n--- Objets ItemPickup dans la scène ---");
        ItemPickup[] pickups = FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);
        if (pickups.Length == 0)
        {
            Debug.LogWarning("⚠️ Aucun ItemPickup trouvé dans la scène !");
        }
        else
        {
            Debug.Log($"Trouvé {pickups.Length} ItemPickup(s) :");
            foreach (var pickup in pickups)
            {
                string status = "";

                // Vérifier collider
                Collider col = pickup.GetComponent<Collider>();
                if (col == null)
                    status += "❌ PAS DE COLLIDER ";
                else if (!col.isTrigger)
                    status += "⚠️ TRIGGER DÉSACTIVÉ ";
                else
                    status += "✅ Collider OK ";

                // Vérifier layer
                if (pickup.gameObject.layer != 6)
                    status += $"❌ Layer {pickup.gameObject.layer} ";
                else
                    status += "✅ Layer 6 ";

                Debug.Log($"   {pickup.name} - {status}");
            }
        }

        Debug.Log("\n=== FIN DU DIAGNOSTIC ===");
        Debug.Log("Appuyez sur E face à un objet et vérifiez la console pour les messages d'interaction");
    }

    private void OnDrawGizmos()
    {
        // Dessiner un rayon devant le joueur
        Camera cam = Camera.main;
        if (cam != null)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ray.origin, ray.direction * checkRange);
        }
    }
}
