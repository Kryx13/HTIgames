using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Écran de fin de jeu avec saisie du nom du joueur.
/// S'affiche après avoir terminé le niveau.
/// </summary>
public class EndGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button tryAgainButton;

    [Header("Messages")]
    [SerializeField] private string newRecordMessage = "🏆 NOUVEAU RECORD !";
    [SerializeField] private string topTenMessage = "✨ Top 10 !";
    [SerializeField] private string completedMessage = "Temple Échappé !";

    private float finalTime;
    private bool isTopScore = false;
    private LeaderboardManager leaderboardManager;

    private void Start()
    {
        leaderboardManager = LeaderboardManager.Instance;

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitClicked);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipClicked);
        }

        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        }

        // Cacher le panel au départ
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        // Limite de caractères pour le nom
        if (nameInputField != null)
        {
            nameInputField.characterLimit = 15;
        }
    }

    /// <summary>
    /// Affiche l'écran de fin avec le temps final
    /// </summary>
    public void ShowEndGame(float time)
    {
        Debug.Log($"🎬 EndGameUI.ShowEndGame() appelé avec temps: {FormatTime(time)}");

        finalTime = time;

        // Afficher le panel
        if (endGamePanel != null)
        {
            Debug.Log($"✅ Activation du panel: {endGamePanel.name}");
            Debug.Log($"   État avant: {endGamePanel.activeSelf}");

            endGamePanel.SetActive(true);

            Debug.Log($"   État après: {endGamePanel.activeSelf}");
            Debug.Log($"   Parent actif: {(endGamePanel.transform.parent != null ? endGamePanel.transform.parent.gameObject.activeInHierarchy : true)}");

            // Vérifier le Canvas
            Canvas canvas = endGamePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"   Canvas trouvé: {canvas.gameObject.name} (Actif: {canvas.gameObject.activeInHierarchy})");
                Debug.Log($"   Render Mode: {canvas.renderMode}");
                Debug.Log($"   Sort Order: {canvas.sortingOrder}");
            }
            else
            {
                Debug.LogError("   ❌ Aucun Canvas parent trouvé !");
            }

            // Vérifier RectTransform et taille
            RectTransform rect = endGamePanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                Debug.Log($"   Taille du panel: {rect.rect.width} x {rect.rect.height}");
                Debug.Log($"   Position locale: {rect.localPosition}");
                Debug.Log($"   Ancres: Min={rect.anchorMin}, Max={rect.anchorMax}");
                Debug.Log($"   Scale: {rect.localScale}");
            }

            // Forcer en premier plan
            endGamePanel.transform.SetAsLastSibling();
            Debug.Log($"   ✅ Panel déplacé en dernier enfant (au-dessus de tout)");
        }
        else
        {
            Debug.LogError("❌ endGamePanel est NULL ! Assignez-le dans l'Inspector du EndGameUI !");
            Debug.LogError($"   Script EndGameUI attaché à: {gameObject.name}");
            return;
        }

        // Afficher le temps
        if (finalTimeText != null)
        {
            finalTimeText.text = $"Temps: {FormatTime(finalTime)}";
        }

        // Vérifier si c'est un top score
        if (leaderboardManager != null)
        {
            isTopScore = leaderboardManager.IsTopScore(finalTime);

            // Vérifier si c'est le meilleur temps absolu
            bool isNewRecord = false;
            if (PlayerPrefs.HasKey("BestTime"))
            {
                float bestTime = PlayerPrefs.GetFloat("BestTime");
                isNewRecord = finalTime < bestTime;
            }
            else
            {
                isNewRecord = true; // Premier temps
            }

            // Afficher le message approprié
            if (congratsText != null)
            {
                if (isNewRecord)
                {
                    congratsText.text = newRecordMessage;
                }
                else if (isTopScore)
                {
                    congratsText.text = topTenMessage;
                }
                else
                {
                    congratsText.text = completedMessage;
                }
            }
        }

        // Focus sur le champ de texte
        if (nameInputField != null)
        {
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }

        // Déverrouiller le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Soumet le score avec le nom
    /// </summary>
    private void OnSubmitClicked()
    {
        string playerName = "Anonyme";

        if (nameInputField != null && !string.IsNullOrWhiteSpace(nameInputField.text))
        {
            playerName = nameInputField.text.Trim();
        }

        // Ajouter au leaderboard si top score
        if (isTopScore && leaderboardManager != null)
        {
            leaderboardManager.AddScore(playerName, finalTime);
        }

        // Retourner au menu
        ReturnToMenu();
    }

    /// <summary>
    /// Passe l'écran sans sauvegarder
    /// </summary>
    private void OnSkipClicked()
    {
        ReturnToMenu();
    }

    /// <summary>
    /// Recommence le niveau immédiatement
    /// </summary>
    private void OnTryAgainClicked()
    {
        // Optionnel: Sauvegarder le score si le joueur a entré un nom
        if (nameInputField != null && !string.IsNullOrWhiteSpace(nameInputField.text) && isTopScore && leaderboardManager != null)
        {
            string playerName = nameInputField.text.Trim();
            leaderboardManager.AddScore(playerName, finalTime);
        }

        // Réinitialiser Time.timeScale
        Time.timeScale = 1f;

        // Recharger la scène de jeu (scène 1)
        SceneManager.LoadScene(1);

        Debug.Log("🔄 Recommencer le niveau");
    }

    /// <summary>
    /// Retourne  au menu principal
    /// </summary>
    private void ReturnToMenu()
    {
        // Réinitialiser Time.timeScale
        Time.timeScale = 1f;

        // Charger le menu principal (scène 0)
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Formate un temps en MM:SS.MS
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        int m = Mathf.FloorToInt(timeInSeconds / 60F);
        int s = Mathf.FloorToInt(timeInSeconds % 60F);
        int ms = Mathf.FloorToInt((timeInSeconds * 100F) % 100F);
        return string.Format("{0:00}:{1:00}.{2:00}", m, s, ms);
    }
}
