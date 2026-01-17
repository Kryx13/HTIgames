using UnityEngine;

/// <summary>
/// Script utilitaire pour auto-configurer les objets ItemPickup.
/// Ajoute automatiquement un collider si absent et configure le layer.
/// </summary>
[RequireComponent(typeof(ItemPickup))]
public class InteractionSystemHelper : MonoBehaviour
{
    [Header("Configuration Auto")]
    [SerializeField] private bool autoConfigureOnAwake = true;
    [SerializeField] private ColliderType colliderType = ColliderType.Sphere;

    public enum ColliderType
    {
        Box,
        Sphere,
        Capsule
    }

    private void Awake()
    {
        if (autoConfigureOnAwake)
        {
            AutoConfigure();
        }
    }

    [ContextMenu("Auto-Configure Pickup")]
    public void AutoConfigure()
    {
        // 1. Vérifier et ajouter un Collider si nécessaire
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            switch (colliderType)
            {
                case ColliderType.Box:
                    col = gameObject.AddComponent<BoxCollider>();
                    Debug.Log($"✅ BoxCollider ajouté à {gameObject.name}");
                    break;
                case ColliderType.Sphere:
                    col = gameObject.AddComponent<SphereCollider>();
                    Debug.Log($"✅ SphereCollider ajouté à {gameObject.name}");
                    break;
                case ColliderType.Capsule:
                    col = gameObject.AddComponent<CapsuleCollider>();
                    Debug.Log($"✅ CapsuleCollider ajouté à {gameObject.name}");
                    break;
            }
        }

        // 2. Activer le mode Trigger
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.Log($"✅ Collider configuré en mode Trigger sur {gameObject.name}");
        }

        // 3. Assigner au layer 6 (Interactable)
        if (gameObject.layer != 6)
        {
            gameObject.layer = 6;
            Debug.Log($"✅ Layer changé en 'Interactable' (6) sur {gameObject.name}");
        }
    }

    private void OnValidate()
    {
        // Vérification visuelle dans l'éditeur
        ItemPickup pickup = GetComponent<ItemPickup>();
        if (pickup == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} : InteractionSystemHelper nécessite un composant ItemPickup !");
        }
    }
}
