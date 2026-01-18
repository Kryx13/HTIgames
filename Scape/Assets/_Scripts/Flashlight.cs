using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Système de lampe torche.
/// La lampe s'active/désactive avec F uniquement si le joueur possède l'objet "Lampe" dans son inventaire.
/// </summary>
public class Flashlight : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform lightHolder; // Point d'attache de la lumière (caméra ou main)

    [Header("Light Settings")]
    [SerializeField] private float lightIntensity = 2f;
    [SerializeField] private float lightRange = 15f;
    [SerializeField] private float spotAngle = 60f;
    [SerializeField] private Color lightColor = Color.white;

    [Header("Visual Settings")]
    [SerializeField] private bool autoCreateLight = true;
    [SerializeField] private Vector3 lightOffset = new Vector3(0.3f, -0.2f, 0.5f); // Position relative à la caméra
    [SerializeField] private GameObject flashlightModel; // Modèle 3D de la lampe
    [SerializeField] private Transform flashlightHolder; // Point d'attache du modèle (joueur)
    [SerializeField] private bool autoCreateFlashlightModel = true;
    [SerializeField] private Vector3 flashlightPosition = new Vector3(-0.35f, 1.0f, 0.3f); // Position sur le côté gauche du personnage
    [SerializeField] private Vector3 flashlightRotation = new Vector3(15, 0, 0);
    [SerializeField] private Vector3 flashlightScale = new Vector3(0.08f, 0.08f, 0.25f); // Plus grande et visible

    [Header("Animation")]
    [SerializeField] private bool pulseWhenOn = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinIntensity = 1.5f;
    [SerializeField] private float pulseMaxIntensity = 2.5f;

    private Light flashlightComponent;
    private bool isOn = false;
    private bool hasFlashlight = false;
    private ItemVisibilityManager visibilityManager;
    private GameManager gameManager;
    private float baseIntensity;

    private void Start()
    {
        // Récupérer l'inventaire si non assigné
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        // Utiliser la caméra comme point d'attache par défaut
        Camera mainCam = Camera.main;
        if (lightHolder == null)
        {
            if (mainCam != null)
            {
                lightHolder = mainCam.transform;
            }
            else
            {
                lightHolder = transform;
            }
        }

        // Pour le modèle visuel, utiliser le transform du joueur
        if (flashlightHolder == null)
        {
            flashlightHolder = transform; // Attaché au joueur pour vue 3ème personne
        }

        // Créer la lumière automatiquement
        if (autoCreateLight)
        {
            CreateFlashlight();
        }

        // Créer le modèle visuel de la lampe
        if (autoCreateFlashlightModel && flashlightModel == null)
        {
            CreateFlashlightModel();
        }

        // Enregistrer le modèle avec le visibility manager
        visibilityManager = ItemVisibilityManager.Instance;
        if (visibilityManager != null && flashlightModel != null)
        {
            visibilityManager.RegisterItemModel(ItemVisibilityManager.ItemType.Flashlight, flashlightModel);
        }

        // Récupérer le GameManager
        gameManager = GameManager.Instance;

        // Sauvegarder l'intensité de base pour l'animation
        if (flashlightComponent != null)
        {
            baseIntensity = lightIntensity;
            flashlightComponent.enabled = false;
        }

        // Cacher le modèle au départ
        if (flashlightModel != null)
        {
            flashlightModel.SetActive(false);
        }
    }

    private void Update()
    {
        // Ne rien faire si le jeu est en pause ou terminé
        if (gameManager != null && (gameManager.IsPaused || gameManager.IsGameEnded))
        {
            return;
        }

        // Vérifier si le joueur possède la lampe dans l'inventaire
        CheckForFlashlight();

        // Détecter la touche F pour toggle la lampe
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }

        // Animation de pulsation quand la lampe est allumée
        if (isOn && pulseWhenOn && flashlightComponent != null)
        {
            float pulse = Mathf.Lerp(pulseMinIntensity, pulseMaxIntensity, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            flashlightComponent.intensity = pulse;
        }
    }

    /// <summary>
    /// Vérifie si le joueur a la lampe dans son inventaire
    /// </summary>
    private void CheckForFlashlight()
    {
        if (inventory != null)
        {
            bool previousState = hasFlashlight;
            hasFlashlight = inventory.HasItem("Lampe");

            // Si on vient de ramasser la lampe
            if (hasFlashlight && !previousState)
            {
                Debug.Log("🔦 Lampe équipée ! Appuyez sur F pour l'allumer/éteindre");
            }
        }
    }

    /// <summary>
    /// Active ou désactive la lampe
    /// </summary>
    private void ToggleFlashlight()
    {
        if (!hasFlashlight)
        {
            Debug.Log("⚠️ Vous n'avez pas de lampe ! Trouvez-la d'abord.");
            return;
        }

        if (flashlightComponent == null)
        {
            Debug.LogError("❌ Aucun composant Light trouvé pour la lampe !");
            return;
        }

        isOn = !isOn;
        flashlightComponent.enabled = isOn;

        // Afficher/cacher le modèle via le visibility manager
        if (isOn)
        {
            if (visibilityManager != null && flashlightModel != null)
            {
                visibilityManager.ShowItem(ItemVisibilityManager.ItemType.Flashlight);
            }
            Debug.Log("🔦 Lampe ALLUMÉE");
        }
        else
        {
            if (visibilityManager != null)
            {
                visibilityManager.HideItem(ItemVisibilityManager.ItemType.Flashlight);
            }
            Debug.Log("🔦 Lampe ÉTEINTE");
        }
    }

    /// <summary>
    /// Crée automatiquement une lumière spotlight
    /// </summary>
    private void CreateFlashlight()
    {
        GameObject lightObject = new GameObject("Flashlight");
        lightObject.transform.SetParent(lightHolder);
        lightObject.transform.localPosition = lightOffset;
        lightObject.transform.localRotation = Quaternion.identity;

        flashlightComponent = lightObject.AddComponent<Light>();
        flashlightComponent.type = LightType.Spot;
        flashlightComponent.intensity = lightIntensity;
        flashlightComponent.range = lightRange;
        flashlightComponent.spotAngle = spotAngle;
        flashlightComponent.color = lightColor;
        flashlightComponent.shadows = LightShadows.Soft; // Ombres douces pour plus de réalisme

        // Paramètres URP (si applicable)
        flashlightComponent.renderMode = LightRenderMode.ForcePixel;

        Debug.Log("✅ Lampe torche créée automatiquement");
    }

    /// <summary>
    /// Crée un modèle 3D simple de la lampe
    /// </summary>
    private void CreateFlashlightModel()
    {
        flashlightModel = new GameObject("FlashlightModel");
        flashlightModel.transform.SetParent(flashlightHolder);
        flashlightModel.transform.localPosition = flashlightPosition;
        flashlightModel.transform.localRotation = Quaternion.Euler(flashlightRotation);
        flashlightModel.transform.localScale = flashlightScale;

        // Corps de la lampe (cylindre)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.name = "Body";
        body.transform.SetParent(flashlightModel.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.Euler(90, 0, 0);
        body.transform.localScale = Vector3.one;
        Destroy(body.GetComponent<Collider>());

        Renderer bodyRenderer = body.GetComponent<Renderer>();
        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = new Color(0.2f, 0.2f, 0.2f); // Gris foncé
        }

        // Tête de la lampe (partie avant, plus large)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        head.name = "Head";
        head.transform.SetParent(flashlightModel.transform);
        head.transform.localPosition = new Vector3(0, 0.6f, 0);
        head.transform.localRotation = Quaternion.Euler(90, 0, 0);
        head.transform.localScale = new Vector3(1.3f, 0.3f, 1.3f);
        Destroy(head.GetComponent<Collider>());

        Renderer headRenderer = head.GetComponent<Renderer>();
        if (headRenderer != null)
        {
            headRenderer.material.color = new Color(0.8f, 0.8f, 0.2f); // Jaune (verre)
        }

        Debug.Log($"✅ Modèle 3D de la lampe créé (attaché à {flashlightHolder.name})");
    }

    /// <summary>
    /// Force l'allumage de la lampe (utilisé pour les zones sombres obligatoires)
    /// </summary>
    public void ForceOn()
    {
        if (hasFlashlight && flashlightComponent != null)
        {
            isOn = true;
            flashlightComponent.enabled = true;
        }
    }

    /// <summary>
    /// Force l'extinction de la lampe
    /// </summary>
    public void ForceOff()
    {
        if (flashlightComponent != null)
        {
            isOn = false;
            flashlightComponent.enabled = false;
        }
    }

    // Propriétés publiques
    public bool IsOn => isOn;
    public bool HasFlashlight => hasFlashlight;
}
