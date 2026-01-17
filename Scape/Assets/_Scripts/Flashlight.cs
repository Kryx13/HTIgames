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

    private Light flashlightComponent;
    private bool isOn = false;
    private bool hasFlashlight = false;

    private void Start()
    {
        // Récupérer l'inventaire si non assigné
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        // Utiliser la caméra comme point d'attache par défaut
        if (lightHolder == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                lightHolder = mainCam.transform;
            }
            else
            {
                lightHolder = transform;
            }
        }

        // Créer la lumière automatiquement
        if (autoCreateLight)
        {
            CreateFlashlight();
        }

        // Désactiver la lumière au départ
        if (flashlightComponent != null)
        {
            flashlightComponent.enabled = false;
        }
    }

    private void Update()
    {
        // Vérifier si le joueur possède la lampe dans l'inventaire
        CheckForFlashlight();

        // Détecter la touche F pour toggle la lampe
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
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

        if (isOn)
        {
            Debug.Log("🔦 Lampe ALLUMÉE");
        }
        else
        {
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
