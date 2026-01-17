using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Système de tir avec le pistolet.
/// Tire un raycast depuis le centre de la caméra pour toucher des cibles.
/// Nécessite d'avoir le "Pistolet" dans l'inventaire.
/// </summary>
public class Gun : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Camera playerCamera;

    [Header("Gun Settings")]
    [SerializeField] private float fireRate = 0.5f; // Temps entre deux tirs (en secondes)
    [SerializeField] private float range = 100f; // Portée du tir
    [SerializeField] private int damage = 1; // Dégâts par tir
    [SerializeField] private bool useLayerMask = false; // Filtrer par layer ?
    [SerializeField] private LayerMask shootableLayers; // Layers que la balle peut toucher (si useLayerMask = true)

    [Header("Visual Effects")]
    [SerializeField] private GameObject impactEffectPrefab; // Effet d'impact (particules)
    [SerializeField] private float impactEffectDuration = 0.5f;
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private bool showBulletTrail = true; // Afficher la traînée de balle
    [SerializeField] private float bulletTrailDuration = 0.1f; // Durée de la traînée

    [Header("Gun Visual Model")]
    [SerializeField] private GameObject gunModel; // Modèle 3D du pistolet
    [SerializeField] private Transform gunHolder; // Point d'attache (main droite du joueur)
    [SerializeField] private bool autoCreateGunModel = true;
    [SerializeField] private float gunHideDelay = 5f; // Temps avant de cacher le pistolet (secondes)
    [SerializeField] private Vector3 gunPosition = new Vector3(0.4f, 0.5f, 0.3f); // Position relative au joueur (côté droit, main)
    [SerializeField] private Vector3 gunRotation = new Vector3(0, 90, 0); // Rotation
    [SerializeField] private Vector3 gunScale = new Vector3(0.08f, 0.08f, 0.2f); // Échelle

    [Header("Auto Effects")]
    [SerializeField] private bool autoCreateImpactEffect = true;

    private float nextFireTime = 0f;
    private float lastShotTime = 0f;
    private bool hasGun = false;
    private LineRenderer bulletTrailRenderer;
    private Transform barrelTip; // Point de sortie de la balle (bout du canon)

    private void Start()
    {
        // Récupérer les références si non assignées
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Si pas de point d'attache spécifié, utiliser le transform du joueur
        if (gunHolder == null)
        {
            gunHolder = transform; // Utilise le Player comme parent
        }

        // Créer un effet d'impact simple si nécessaire
        if (autoCreateImpactEffect && impactEffectPrefab == null)
        {
            CreateSimpleImpactEffect();
        }

        // Créer le LineRenderer pour la traînée de balle
        if (showBulletTrail)
        {
            CreateBulletTrail();
        }

        // Créer le modèle 3D du pistolet
        if (autoCreateGunModel && gunModel == null)
        {
            CreateGunModel();
        }

        // Cacher le pistolet au départ
        if (gunModel != null)
        {
            gunModel.SetActive(false);
        }
    }

    private void Update()
    {
        // Vérifier si le joueur possède le pistolet
        CheckForGun();

        // Détection du tir (Left Click)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryShoot();
        }

        // Gérer la visibilité du pistolet (cacher après inactivité)
        if (hasGun && gunModel != null)
        {
            if (Time.time - lastShotTime > gunHideDelay)
            {
                gunModel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Vérifie si le joueur a le pistolet dans son inventaire
    /// </summary>
    private void CheckForGun()
    {
        if (inventory != null)
        {
            bool previousState = hasGun;
            hasGun = inventory.HasItem("Pistolet");

            // Si on vient de ramasser le pistolet
            if (hasGun && !previousState)
            {
                Debug.Log("🔫 Pistolet équipé ! Clic gauche pour tirer");
            }
        }
    }

    /// <summary>
    /// Tente de tirer
    /// </summary>
    private void TryShoot()
    {
        // Vérifier si on a le pistolet
        if (!hasGun)
        {
            Debug.Log("⚠️ Vous n'avez pas de pistolet !");
            return;
        }

        // Vérifier le cooldown (cadence de tir)
        if (Time.time < nextFireTime)
        {
            return;
        }

        // Tirer !
        Shoot();

        // Mise à jour du prochain tir possible
        nextFireTime = Time.time + fireRate;
    }

    /// <summary>
    /// Tire un raycast depuis le centre de la caméra
    /// </summary>
    private void Shoot()
    {
        if (playerCamera == null)
        {
            Debug.LogError("❌ Pas de caméra assignée pour le tir !");
            return;
        }

        // Afficher le pistolet et mettre à jour le timer
        if (gunModel != null)
        {
            gunModel.SetActive(true);
        }
        lastShotTime = Time.time;

        // Raycast depuis le centre de l'écran
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Debug visuel
        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 0.5f);
        }

        Debug.Log("🔫 BANG ! Tir effectué");

        bool hitSomething = false;
        Vector3 endPoint = ray.origin + ray.direction * range;

        // Raycast avec ou sans LayerMask
        if (useLayerMask)
        {
            hitSomething = Physics.Raycast(ray, out hit, range, shootableLayers, QueryTriggerInteraction.Ignore);
        }
        else
        {
            // Tirer sur tout (meilleure option par défaut)
            hitSomething = Physics.Raycast(ray, out hit, range, ~0, QueryTriggerInteraction.Ignore);
        }

        // Vérifier si on touche quelque chose
        if (hitSomething)
        {
            endPoint = hit.point;
            Debug.Log($"🎯 Touché : {hit.collider.name} à {hit.distance:F1}m");

            // Vérifier si c'est une cible
            Target target = hit.collider.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // Créer un effet d'impact
            CreateImpact(hit.point, hit.normal);
        }
        else
        {
            Debug.Log("❌ Raté !");
        }

        // Afficher la traînée de balle (du canon vers la cible)
        if (showBulletTrail && bulletTrailRenderer != null)
        {
            Vector3 bulletStart = barrelTip != null ? barrelTip.position : ray.origin;
            StartCoroutine(ShowBulletTrail(bulletStart, endPoint));
        }
    }

    /// <summary>
    /// Crée un effet d'impact au point de collision
    /// </summary>
    private void CreateImpact(Vector3 position, Vector3 normal)
    {
        if (impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(impactEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(impact, impactEffectDuration);
        }
    }

    /// <summary>
    /// Crée un effet d'impact simple (sphère rouge)
    /// </summary>
    private void CreateSimpleImpactEffect()
    {
        impactEffectPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impactEffectPrefab.name = "ImpactEffect";
        impactEffectPrefab.transform.localScale = Vector3.one * 0.2f;

        // Couleur rouge
        Renderer renderer = impactEffectPrefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        // Supprimer le collider
        Destroy(impactEffectPrefab.GetComponent<Collider>());

        // Désactiver pour l'instant (sera instancié lors des tirs)
        impactEffectPrefab.SetActive(false);

        Debug.Log("✅ Effet d'impact simple créé");
    }

    /// <summary>
    /// Crée un LineRenderer pour visualiser la traînée de balle
    /// </summary>
    private void CreateBulletTrail()
    {
        GameObject trailObj = new GameObject("BulletTrail");
        trailObj.transform.SetParent(transform);

        bulletTrailRenderer = trailObj.AddComponent<LineRenderer>();
        bulletTrailRenderer.startWidth = 0.02f;
        bulletTrailRenderer.endWidth = 0.02f;
        bulletTrailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bulletTrailRenderer.startColor = Color.yellow;
        bulletTrailRenderer.endColor = Color.red;
        bulletTrailRenderer.positionCount = 2;
        bulletTrailRenderer.enabled = false;

        Debug.Log("✅ Traînée de balle créée");
    }

    /// <summary>
    /// Affiche la traînée de balle pendant un court instant
    /// </summary>
    private System.Collections.IEnumerator ShowBulletTrail(Vector3 start, Vector3 end)
    {
        bulletTrailRenderer.SetPosition(0, start);
        bulletTrailRenderer.SetPosition(1, end);
        bulletTrailRenderer.enabled = true;

        yield return new WaitForSeconds(bulletTrailDuration);

        bulletTrailRenderer.enabled = false;
    }

    /// <summary>
    /// Crée un modèle 3D simple du pistolet
    /// </summary>
    private void CreateGunModel()
    {
        gunModel = new GameObject("GunModel");
        gunModel.transform.SetParent(gunHolder);
        gunModel.transform.localPosition = gunPosition;
        gunModel.transform.localRotation = Quaternion.Euler(gunRotation);
        gunModel.transform.localScale = gunScale;

        // Corps du pistolet (cube principal)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(gunModel.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = Vector3.one;
        Destroy(body.GetComponent<Collider>());

        Renderer bodyRenderer = body.GetComponent<Renderer>();
        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = new Color(0.2f, 0.2f, 0.2f); // Gris foncé
        }

        // Canon du pistolet
        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Barrel";
        barrel.transform.SetParent(gunModel.transform);
        barrel.transform.localPosition = new Vector3(0, 0, 0.7f);
        barrel.transform.localRotation = Quaternion.Euler(90, 0, 0);
        barrel.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
        Destroy(barrel.GetComponent<Collider>());

        Renderer barrelRenderer = barrel.GetComponent<Renderer>();
        if (barrelRenderer != null)
        {
            barrelRenderer.material.color = new Color(0.1f, 0.1f, 0.1f); // Noir
        }

        // Créer un point de sortie de balle au bout du canon
        GameObject tipObject = new GameObject("BarrelTip");
        tipObject.transform.SetParent(barrel.transform);
        tipObject.transform.localPosition = new Vector3(0, 0.6f, 0); // Au bout du cylindre
        barrelTip = tipObject.transform;

        Debug.Log($"✅ Modèle 3D du pistolet créé (attaché à {gunHolder.name})");
    }

    /// <summary>
    /// Permet d'ajuster la position du pistolet en temps réel dans l'éditeur
    /// </summary>
    private void OnValidate()
    {
        if (gunModel != null && Application.isPlaying)
        {
            gunModel.transform.localPosition = gunPosition;
            gunModel.transform.localRotation = Quaternion.Euler(gunRotation);
            gunModel.transform.localScale = gunScale;
        }
    }

    // Propriétés publiques
    public bool HasGun => hasGun;
    public float FireRate => fireRate;
}
