using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

/// <summary>
/// Système de pioche pour détruire les murs fragiles.
/// Utilise Right Click pour frapper et casser les murs destructibles.
/// Nécessite d'avoir la "Pioche" dans l'inventaire.
/// </summary>
public class Pickaxe : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Camera playerCamera;

    [Header("Pickaxe Settings")]
    [SerializeField] private float range = 5f; // Portée de la pioche (augmentée pour toucher plus loin)
    [SerializeField] private float hitRate = 0.5f; // Temps entre deux coups
    [SerializeField] private int damagePerHit = 1; // Dégâts par coup

    [Header("Visual Settings")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private GameObject hitEffectPrefab; // Effet quand on frappe un mur
    [SerializeField] private bool autoCreateHitEffect = true;
    [SerializeField] private GameObject pickaxeModel; // Modèle 3D de la pioche
    [SerializeField] private Transform pickaxeHolder; // Point d'attache (joueur pour vue 3ème personne)
    [SerializeField] private bool autoCreatePickaxeModel = true;
    [SerializeField] private Vector3 pickaxePosition = new Vector3(0.4f, 1.0f, 0.3f); // Côté droit du personnage, plus bas
    [SerializeField] private Vector3 pickaxeRotation = new Vector3(0, 0, -45f);
    [SerializeField] private Vector3 pickaxeScale = new Vector3(0.15f, 0.4f, 0.15f); // Pioche plus grande et visible

    [Header("Animation")]
    [SerializeField] private bool animateSwing = true;
    [SerializeField] private float swingDuration = 0.3f;
    [SerializeField] private float swingAngle = 90f; // Angle de rotation lors du swing

    private float nextHitTime = 0f;
    private bool hasPickaxe = false;
    private ItemVisibilityManager visibilityManager;
    private bool isSwinging = false;
    private Quaternion restRotation;
    private Quaternion swingRotation;

    private void Start()
    {
        // Récupérer les références
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Si pas de point d'attache, utiliser le transform du joueur
        if (pickaxeHolder == null)
        {
            pickaxeHolder = transform; // Attaché au joueur pour vue 3ème personne
        }

        // Créer le modèle de la pioche
        if (autoCreatePickaxeModel && pickaxeModel == null)
        {
            CreatePickaxeModel();
        }

        // Enregistrer le modèle avec le visibility manager
        visibilityManager = ItemVisibilityManager.Instance;
        if (visibilityManager != null && pickaxeModel != null)
        {
            visibilityManager.RegisterItemModel(ItemVisibilityManager.ItemType.Pickaxe, pickaxeModel);
        }

        // Sauvegarder les rotations pour l'animation
        if (pickaxeModel != null)
        {
            restRotation = pickaxeModel.transform.localRotation;
            swingRotation = restRotation * Quaternion.Euler(-swingAngle, 0, 0);
            pickaxeModel.SetActive(false);
        }

        // Créer l'effet de coup simple si nécessaire
        if (autoCreateHitEffect && hitEffectPrefab == null)
        {
            CreateSimpleHitEffect();
        }
    }

    private void Update()
    {
        // Vérifier si le joueur possède la pioche
        CheckForPickaxe();

        // Détection du coup de pioche (Right Click)
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryHit();
        }
    }

    /// <summary>
    /// Vérifie si le joueur a la pioche dans son inventaire
    /// </summary>
    private void CheckForPickaxe()
    {
        if (inventory != null)
        {
            bool previousState = hasPickaxe;
            hasPickaxe = inventory.HasItem("Pioche");

            // Si on vient de ramasser la pioche
            if (hasPickaxe && !previousState)
            {
                Debug.Log("⛏️ Pioche équipée ! Clic droit pour casser les murs fragiles");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Inventory est null dans Pickaxe.cs");
        }
    }

    /// <summary>
    /// Tente de frapper avec la pioche
    /// </summary>
    private void TryHit()
    {
        // Vérifier si on a la pioche
        if (!hasPickaxe)
        {
            Debug.Log("⚠️ Vous n'avez pas de pioche !");
            return;
        }

        // Vérifier le cooldown
        if (Time.time < nextHitTime)
        {
            return;
        }

        // Frapper !
        Hit();

        // Mise à jour du prochain coup possible
        nextHitTime = Time.time + hitRate;
    }

    /// <summary>
    /// Frappe avec la pioche et détecte les murs destructibles
    /// </summary>
    private void Hit()
    {
        if (playerCamera == null)
        {
            Debug.LogError("❌ Pas de caméra pour le raycast !");
            return;
        }

        // Afficher la pioche via le visibility manager
        if (visibilityManager != null && pickaxeModel != null)
        {
            visibilityManager.ShowItem(ItemVisibilityManager.ItemType.Pickaxe);
        }

        // Déclencher l'animation de swing
        if (animateSwing && !isSwinging)
        {
            StartCoroutine(SwingAnimation());
        }

        // Raycast depuis le centre de l'écran
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Debug visuel - Rayon cyan pour voir où on vise
        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.cyan, 1f);
            Debug.Log($"⛏️ Raycast depuis {ray.origin} dans la direction {ray.direction}, portée {range}m");
        }

        Debug.Log("⛏️ *Coup de pioche*");

        // Vérifier si on touche quelque chose
        if (Physics.Raycast(ray, out hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"⛏️ Frappé : {hit.collider.name} (Distance: {hit.distance:F2}m)");

            // Vérifier si c'est un mur destructible
            DestructibleWall wall = hit.collider.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                Debug.Log($"✅ Mur destructible détecté ! Application de {damagePerHit} dégâts");
                wall.TakeDamage(damagePerHit);

                // Effet de frappe au point d'impact
                CreateHitEffect(hit.point, hit.normal);
            }
            else
            {
                Debug.Log($"⚠️ L'objet '{hit.collider.name}' n'est pas un mur destructible !");
                Debug.Log($"Components sur {hit.collider.name}: {string.Join(", ", hit.collider.GetComponents<Component>().Select(c => c.GetType().Name))}");
            }
        }
        else
        {
            Debug.Log($"⛏️ Coup dans le vide ! (Portée: {range}m)");
        }
    }

    /// <summary>
    /// Crée un effet visuel au point d'impact
    /// </summary>
    private void CreateHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(effect, 0.5f);
        }
    }

    /// <summary>
    /// Crée un modèle 3D simple de la pioche
    /// </summary>
    private void CreatePickaxeModel()
    {
        pickaxeModel = new GameObject("PickaxeModel");
        pickaxeModel.transform.SetParent(pickaxeHolder);
        pickaxeModel.transform.localPosition = pickaxePosition;
        pickaxeModel.transform.localRotation = Quaternion.Euler(pickaxeRotation);
        pickaxeModel.transform.localScale = pickaxeScale;

        // Manche de la pioche (cylindre brun)
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handle.name = "Handle";
        handle.transform.SetParent(pickaxeModel.transform);
        handle.transform.localPosition = Vector3.zero;
        handle.transform.localRotation = Quaternion.Euler(90, 0, 0);
        handle.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
        Destroy(handle.GetComponent<Collider>());

        Renderer handleRenderer = handle.GetComponent<Renderer>();
        if (handleRenderer != null)
        {
            handleRenderer.material.color = new Color(0.4f, 0.2f, 0.1f); // Brun
        }

        // Tête de la pioche (cube gris métallique) - Plus grande
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(pickaxeModel.transform);
        head.transform.localPosition = new Vector3(0, 1.3f, 0);
        head.transform.localScale = new Vector3(2.5f, 0.6f, 0.6f); // Plus grande
        Destroy(head.GetComponent<Collider>());

        Renderer headRenderer = head.GetComponent<Renderer>();
        if (headRenderer != null)
        {
            headRenderer.material.color = new Color(0.6f, 0.6f, 0.7f); // Gris métallique
        }

        Debug.Log($"✅ Modèle 3D de la pioche créé (attaché à {pickaxeHolder.name})");
    }

    /// <summary>
    /// Animation de swing pour la pioche
    /// </summary>
    private System.Collections.IEnumerator SwingAnimation()
    {
        if (pickaxeModel == null) yield break;

        isSwinging = true;
        float elapsed = 0f;

        // Swing vers le bas
        while (elapsed < swingDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (swingDuration / 2f);
            pickaxeModel.transform.localRotation = Quaternion.Lerp(restRotation, swingRotation, t);
            yield return null;
        }

        // Retour à la position initiale
        elapsed = 0f;
        while (elapsed < swingDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (swingDuration / 2f);
            pickaxeModel.transform.localRotation = Quaternion.Lerp(swingRotation, restRotation, t);
            yield return null;
        }

        pickaxeModel.transform.localRotation = restRotation;
        isSwinging = false;
    }

    /// <summary>
    /// Crée un effet simple de frappe (cubes gris)
    /// </summary>
    private void CreateSimpleHitEffect()
    {
        hitEffectPrefab = new GameObject("HitEffect");

        // Créer quelques petits cubes pour simuler des débris
        for (int i = 0; i < 3; i++)
        {
            GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debris.name = "Debris";
            debris.transform.SetParent(hitEffectPrefab.transform);
            debris.transform.localPosition = Random.insideUnitSphere * 0.2f;
            debris.transform.localScale = Vector3.one * Random.Range(0.05f, 0.1f);

            Renderer renderer = debris.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.5f, 0.5f, 0.5f); // Gris
            }

            Destroy(debris.GetComponent<Collider>());
        }

        // Désactiver pour l'instant
        hitEffectPrefab.SetActive(false);

        Debug.Log("✅ Effet de frappe simple créé");
    }

    // Propriétés publiques
    public bool HasPickaxe => hasPickaxe;
    public float Range => range;
}
