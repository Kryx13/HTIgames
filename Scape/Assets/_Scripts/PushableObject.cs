using UnityEngine;

/// <summary>
/// Marque un objet comme poussable et configure automatiquement ses composants.
/// L'objet peut être poussé par le joueur quand il marche dessus.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float mass = 10f; // Masse de l'objet (plus lourd = plus difficile à pousser)
    [SerializeField] private bool freezeRotation = true; // Empêcher l'objet de basculer
    [SerializeField] private bool freezeYPosition = true; // Empêcher l'objet de tomber/voler

    [Header("Layer Configuration")]
    [SerializeField] private bool autoSetLayer = true; // Mettre automatiquement sur le layer "Pushable"
    [SerializeField] private string pushableLayerName = "Default"; // Nom du layer pour les objets poussables

    private Rigidbody rb;

    private void Start()
    {
        ConfigurePushable();
    }

    [ContextMenu("Configure as Pushable")]
    public void ConfigurePushable()
    {
        // Récupérer ou ajouter le Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log($"✅ Rigidbody ajouté à {gameObject.name}");
        }

        // Configurer le Rigidbody
        rb.mass = mass;
        rb.isKinematic = false; // Doit être non-kinematic pour être poussé
        rb.useGravity = true;

        // Contraintes de rotation (empêcher de basculer)
        if (freezeRotation)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Contrainte Y (empêcher de monter/descendre)
        if (freezeYPosition)
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionY;
        }

        // Drag pour que l'objet s'arrête rapidement
        rb.linearDamping = 5f; // Résistance au mouvement
        rb.angularDamping = 5f;

        // Configurer le layer si demandé
        if (autoSetLayer)
        {
            int layerIndex = LayerMask.NameToLayer(pushableLayerName);
            if (layerIndex != -1)
            {
                gameObject.layer = layerIndex;
                Debug.Log($"✅ {gameObject.name} mis sur le layer '{pushableLayerName}'");
            }
            else
            {
                Debug.LogWarning($"⚠️ Layer '{pushableLayerName}' introuvable. Utilisez 'Default' ou créez un layer 'Pushable'");
            }
        }

        // Vérifier qu'il y a un collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"❌ {gameObject.name} : Aucun Collider trouvé ! Ajoutez un BoxCollider.");
        }
        else if (col.isTrigger)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} : Le Collider est en mode Trigger. Désactivez 'Is Trigger' pour que l'objet soit poussable.");
        }

        Debug.Log($"✅ {gameObject.name} configuré comme objet poussable (masse: {mass}kg)");
    }

    private void OnValidate()
    {
        // Appliquer les changements en temps réel dans l'éditeur
        if (rb != null && Application.isPlaying)
        {
            rb.mass = mass;

            if (freezeRotation && freezeYPosition)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            }
            else if (freezeRotation)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else if (freezeYPosition)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    // Propriétés publiques
    public float Mass => mass;
    public bool IsConfigured => rb != null && !rb.isKinematic;
}
