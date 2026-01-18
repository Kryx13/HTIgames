using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.Universal;
#endif

/// <summary>
/// Manages the darkness effect in Stage 6.
/// Makes environment very dark and checks if player has flashlight.
/// </summary>
public class DarknessZone : MonoBehaviour
{
    [Header("Darkness Settings")]
    [SerializeField] private bool enableDarknessEffect = true;
    [SerializeField] private Color ambientColor = new Color(0.05f, 0.05f, 0.08f); // Very dark blue
    [SerializeField] private float fogDensity = 0.08f;
    [SerializeField] private Color fogColor = new Color(0.02f, 0.02f, 0.05f);

    [Header("Flashlight Requirement")]
    [SerializeField] private bool requireFlashlight = true;
    [SerializeField] private string flashlightItemName = "Flashlight";
    [SerializeField] private bool warnIfNoFlashlight = true;

    [Header("Player Light")]
    [SerializeField] private bool addPlayerLight = true;
    [SerializeField] private float playerLightRange = 5f;
    [SerializeField] private float playerLightIntensity = 1.5f;
    [SerializeField] private Color playerLightColor = new Color(1f, 0.9f, 0.7f); // Warm white

    [Header("Exit")]
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private bool deactivateExitOnStart = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Color originalAmbientColor;
    private bool originalFogEnabled;
    private float originalFogDensity;
    private Color originalFogColor;
    private Light playerLight;
    private Transform player;
    private Inventory playerInventory;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
        }

        // Store original lighting settings
        originalAmbientColor = RenderSettings.ambientLight;
        originalFogEnabled = RenderSettings.fog;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogColor = RenderSettings.fogColor;

        // Apply darkness effect
        if (enableDarknessEffect)
        {
            ApplyDarknessEffect();
        }

        // Check flashlight
        if (requireFlashlight)
        {
            CheckFlashlight();
        }

        // Add player light
        if (addPlayerLight && player != null)
        {
            CreatePlayerLight();
        }

        // Hide exit initially
        if (deactivateExitOnStart && exitDoor != null)
        {
            exitDoor.SetActive(false);
        }

        if (showDebugLogs)
        {
            Debug.Log("🌑 Darkness Zone initialized");
        }
    }

    /// <summary>
    /// Applies darkness effect to the scene
    /// </summary>
    private void ApplyDarknessEffect()
    {
        // Set very dark ambient light
        RenderSettings.ambientLight = ambientColor;

        // Enable fog
        RenderSettings.fog = true;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        if (showDebugLogs)
        {
            Debug.Log("  🌑 Darkness effect applied");
        }
    }

    /// <summary>
    /// Checks if player has flashlight
    /// </summary>
    private void CheckFlashlight()
    {
        if (playerInventory == null)
        {
            if (warnIfNoFlashlight && showDebugLogs)
            {
                Debug.LogWarning("⚠️ No Inventory component found on Player!");
            }
            return;
        }

        bool hasFlashlight = playerInventory.HasItem(flashlightItemName);

        if (!hasFlashlight && warnIfNoFlashlight)
        {
            Debug.LogWarning($"⚠️ Player doesn't have {flashlightItemName}! This stage requires it.");
            Debug.LogWarning("   → Make sure you picked it up in Stage 5 (Maze)");
        }
        else if (hasFlashlight && showDebugLogs)
        {
            Debug.Log($"✅ Player has {flashlightItemName}");
        }
    }

    /// <summary>
    /// Creates a light that follows the player
    /// </summary>
    private void CreatePlayerLight()
    {
        GameObject lightObj = new GameObject("PlayerLight");
        lightObj.transform.SetParent(player);
        lightObj.transform.localPosition = new Vector3(0f, 1f, 0.5f); // Head height, slightly forward

        playerLight = lightObj.AddComponent<Light>();
        playerLight.type = LightType.Point;
        playerLight.range = playerLightRange;
        playerLight.intensity = playerLightIntensity;
        playerLight.color = playerLightColor;
        playerLight.shadows = LightShadows.Soft;

        if (showDebugLogs)
        {
            Debug.Log("  💡 Player light created");
        }
    }

    /// <summary>
    /// Called when player reaches the end
    /// </summary>
    public void ActivateExit()
    {
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);

            if (showDebugLogs)
            {
                Debug.Log("✅ Darkness Zone complete! Exit activated.");
            }
        }
    }

    /// <summary>
    /// Restores original lighting when leaving the zone
    /// </summary>
    private void OnDestroy()
    {
        // Restore original lighting settings
        RenderSettings.ambientLight = originalAmbientColor;
        RenderSettings.fog = originalFogEnabled;
        RenderSettings.fogDensity = originalFogDensity;
        RenderSettings.fogColor = originalFogColor;

        if (showDebugLogs)
        {
            Debug.Log("  🌅 Lighting restored to normal");
        }
    }

    /// <summary>
    /// Manually restore lighting (for testing)
    /// </summary>
    [ContextMenu("Restore Normal Lighting")]
    public void RestoreLighting()
    {
        RenderSettings.ambientLight = originalAmbientColor;
        RenderSettings.fog = originalFogEnabled;
        RenderSettings.fogDensity = originalFogDensity;
        RenderSettings.fogColor = originalFogColor;

        Debug.Log("🌅 Lighting restored");
    }

    /// <summary>
    /// Manually apply darkness (for testing)
    /// </summary>
    [ContextMenu("Apply Darkness")]
    public void ApplyDarkness()
    {
        ApplyDarknessEffect();
    }
}
