using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gère l'interface du menu Paramètres.
/// Attachez ce script au Canvas/Panel des Paramètres.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeValueText;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    [Header("Parent Menu")]
    [SerializeField] private GameObject settingsPanel; // Le panel des paramètres
    [SerializeField] private GameObject previousMenu; // Le menu d'où on vient (MainMenu ou PauseMenu)

    private SettingsManager settingsManager;

    private void Start()
    {
        settingsManager = SettingsManager.Instance;

        if (settingsManager == null)
        {
            Debug.LogError("❌ SettingsManager non trouvé !");
            return;
        }

        // Charger les valeurs actuelles
        LoadCurrentSettings();

        // Ajouter les listeners aux sliders
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        // Ajouter les listeners aux boutons
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(OnApplyClicked);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        // Peupler le dropdown de qualité
        PopulateQualityDropdown();
    }

    /// <summary>
    /// Charge les paramètres actuels depuis le SettingsManager
    /// </summary>
    private void LoadCurrentSettings()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = settingsManager.MouseSensitivity;
            UpdateSensitivityText(settingsManager.MouseSensitivity);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = settingsManager.MasterVolume;
            UpdateVolumeText(settingsManager.MasterVolume);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.value = settingsManager.QualityLevel;
        }
    }

    /// <summary>
    /// Remplit le dropdown avec les niveaux de qualité disponibles
    /// </summary>
    private void PopulateQualityDropdown()
    {
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
            qualityDropdown.value = settingsManager.QualityLevel;
        }
    }

    /// <summary>
    /// Appelé quand le slider de sensibilité change
    /// </summary>
    private void OnSensitivityChanged(float value)
    {
        UpdateSensitivityText(value);
    }

    /// <summary>
    /// Appelé quand le slider de volume change
    /// </summary>
    private void OnVolumeChanged(float value)
    {
        UpdateVolumeText(value);
    }

    /// <summary>
    /// Appelé quand le dropdown de qualité change
    /// </summary>
    private void OnQualityChanged(int index)
    {
        // Pas besoin de mettre à jour de texte
    }

    /// <summary>
    /// Met à jour le texte de la sensibilité
    /// </summary>
    private void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = value.ToString("F1");
        }
    }

    /// <summary>
    /// Met à jour le texte du volume
    /// </summary>
    private void UpdateVolumeText(float value)
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    /// <summary>
    /// Applique les paramètres
    /// </summary>
    private void OnApplyClicked()
    {
        if (sensitivitySlider != null)
        {
            settingsManager.SetMouseSensitivity(sensitivitySlider.value);
        }

        if (volumeSlider != null)
        {
            settingsManager.SetMasterVolume(volumeSlider.value);
        }

        if (qualityDropdown != null)
        {
            settingsManager.SetQualityLevel(qualityDropdown.value);
        }

        Debug.Log("✅ Paramètres appliqués");

        // Fermer le menu des paramètres après application
        OnBackClicked();
    }

    /// <summary>
    /// Réinitialise aux valeurs par défaut
    /// </summary>
    private void OnResetClicked()
    {
        settingsManager.ResetToDefaults();
        LoadCurrentSettings();
    }

    /// <summary>
    /// Retour au menu précédent
    /// </summary>
    private void OnBackClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (previousMenu != null)
        {
            previousMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Ouvre le menu des paramètres
    /// </summary>
    public void OpenSettings(GameObject fromMenu)
    {
        previousMenu = fromMenu;

        if (previousMenu != null)
        {
            previousMenu.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        LoadCurrentSettings();
    }
}
