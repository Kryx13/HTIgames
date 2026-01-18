using UnityEngine;

/// <summary>
/// Gère les paramètres du jeu (sensibilité souris, volume, qualité graphique, etc.).
/// Utilise PlayerPrefs pour sauvegarder les préférences.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Default Values")]
    [SerializeField] private float defaultMouseSensitivity = 2f;
    [SerializeField] private float defaultMasterVolume = 0.8f;
    [SerializeField] private int defaultQualityLevel = 2; // Medium

    // Paramètres actuels
    public float MouseSensitivity { get; private set; }
    public float MasterVolume { get; private set; }
    public int QualityLevel { get; private set; }

    // Clés PlayerPrefs
    private const string SENSITIVITY_KEY = "MouseSensitivity";
    private const string VOLUME_KEY = "MasterVolume";
    private const string QUALITY_KEY = "QualityLevel";

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadSettings();
    }

    /// <summary>
    /// Charge les paramètres depuis PlayerPrefs
    /// </summary>
    private void LoadSettings()
    {
        MouseSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, defaultMouseSensitivity);
        MasterVolume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultMasterVolume);
        QualityLevel = PlayerPrefs.GetInt(QUALITY_KEY, defaultQualityLevel);

        ApplySettings();

        Debug.Log($"⚙️ Paramètres chargés - Sensibilité: {MouseSensitivity}, Volume: {MasterVolume}, Qualité: {QualityLevel}");
    }

    /// <summary>
    /// Applique les paramètres au jeu
    /// </summary>
    private void ApplySettings()
    {
        // Appliquer la qualité graphique
        QualitySettings.SetQualityLevel(QualityLevel);

        // Appliquer le volume (sera utilisé par les systèmes audio)
        AudioListener.volume = MasterVolume;
    }

    /// <summary>
    /// Modifie la sensibilité de la souris
    /// </summary>
    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = Mathf.Clamp(value, 0.1f, 10f); // Entre 0.1 et 10
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, MouseSensitivity);
        PlayerPrefs.Save();

        Debug.Log($"🖱️ Sensibilité souris: {MouseSensitivity}");
    }

    /// <summary>
    /// Modifie le volume général
    /// </summary>
    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value); // Entre 0 et 1
        AudioListener.volume = MasterVolume;
        PlayerPrefs.SetFloat(VOLUME_KEY, MasterVolume);
        PlayerPrefs.Save();

        Debug.Log($"🔊 Volume: {MasterVolume * 100}%");
    }

    /// <summary>
    /// Modifie la qualité graphique
    /// </summary>
    public void SetQualityLevel(int level)
    {
        QualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(QualityLevel);
        PlayerPrefs.SetInt(QUALITY_KEY, QualityLevel);
        PlayerPrefs.Save();

        Debug.Log($"🎨 Qualité graphique: {QualitySettings.names[QualityLevel]}");
    }

    /// <summary>
    /// Réinitialise tous les paramètres aux valeurs par défaut
    /// </summary>
    public void ResetToDefaults()
    {
        SetMouseSensitivity(defaultMouseSensitivity);
        SetMasterVolume(defaultMasterVolume);
        SetQualityLevel(defaultQualityLevel);

        Debug.Log("🔄 Paramètres réinitialisés");
    }
}
