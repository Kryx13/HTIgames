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

        // IMPORTANT: Désactiver le PauseMenuPanel s'il existe
        GameObject pauseMenu = GameObject.Find("PauseMenuPanel");
        if (pauseMenu != null && pauseMenu.activeSelf)
        {
            Debug.Log("⚠️ PauseMenuPanel était actif - désactivation");
            pauseMenu.SetActive(false);
        }

        // Désactiver tous les autres panels qui pourraient bloquer
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            foreach (Transform child in canvas.transform)
            {
                if (child.name.Contains("Panel") && child.name != "EndGamePanel" && child.gameObject.activeSelf)
                {
                    Debug.Log($"⚠️ Désactivation de {child.name} pour afficher EndGamePanel");
                    child.gameObject.SetActive(false);
                }
            }
        }

        // Afficher le panel
        if (endGamePanel != null)
        {
            Debug.Log($"✅ Activation du panel: {endGamePanel.name}");
            Debug.Log($"   État avant: {endGamePanel.activeSelf}");

            endGamePanel.SetActive(true);

            Debug.Log($"   État après: {endGamePanel.activeSelf}");
            Debug.Log($"   Parent actif: {(endGamePanel.transform.parent != null ? endGamePanel.transform.parent.gameObject.activeInHierarchy : true)}");

            // Vérifier le Canvas (réutilise la variable canvas déclarée plus haut)
            if (canvas == null)
            {
                canvas = endGamePanel.GetComponentInParent<Canvas>();
            }
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

            // Vérifier l'Image de fond
            Image backgroundImage = endGamePanel.GetComponent<Image>();
            if (backgroundImage != null)
            {
                Debug.Log($"   Image de fond: Color={backgroundImage.color}, Enabled={backgroundImage.enabled}");
                if (backgroundImage.color.a < 0.1f)
                {
                    Debug.LogWarning("   ⚠️ L'image de fond est presque transparente !");
                    backgroundImage.color = new Color(0, 0, 0, 0.9f); // Forcer un fond visible
                }
            }
            else
            {
                Debug.LogWarning("   ⚠️ Pas d'Image component sur EndGamePanel - ajout d'un fond noir");
                backgroundImage = endGamePanel.AddComponent<Image>();
                backgroundImage.color = new Color(0, 0, 0, 0.9f);
            }

            // Forcer le Canvas en premier plan
            if (canvas != null && canvas.sortingOrder < 100)
            {
                Debug.Log($"   🔄 Augmentation du Sort Order de {canvas.sortingOrder} à 100");
                canvas.sortingOrder = 100;
            }

            // Vérifier les enfants (textes, boutons)
            Debug.Log($"   Nombre d'enfants: {endGamePanel.transform.childCount}");
            foreach (Transform child in endGamePanel.transform)
            {
                Debug.Log($"     - {child.name}: Active={child.gameObject.activeSelf}");
            }

            // Vérifier CanvasGroup (peut rendre invisible avec alpha=0)
            CanvasGroup canvasGroup = endGamePanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                Debug.Log($"   CanvasGroup trouvé: Alpha={canvasGroup.alpha}, Interactable={canvasGroup.interactable}, BlocksRaycasts={canvasGroup.blocksRaycasts}");
                if (canvasGroup.alpha < 0.1f)
                {
                    Debug.LogWarning("   ⚠️ CanvasGroup alpha trop bas - correction à 1");
                    canvasGroup.alpha = 1f;
                }
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            // Si le panel n'a pas d'enfants visibles, créer une UI de secours
            if (endGamePanel.transform.childCount == 0 || !HasVisibleChildren(endGamePanel))
            {
                Debug.LogWarning("⚠️ EndGamePanel vide ou invisible - création d'une UI de secours");
                CreateFallbackEndGameUI(endGamePanel, time);
            }
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

    /// <summary>
    /// Vérifie si le panel a des enfants visibles
    /// </summary>
    private bool HasVisibleChildren(GameObject panel)
    {
        foreach (Transform child in panel.transform)
        {
            if (child.gameObject.activeSelf)
            {
                // Vérifier si l'enfant a un composant visible (Image ou Text)
                if (child.GetComponent<Image>() != null || child.GetComponent<TextMeshProUGUI>() != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Crée une UI de fin de jeu de secours si le panel est vide
    /// </summary>
    private void CreateFallbackEndGameUI(GameObject panel, float time)
    {
        Debug.Log("🔧 Création de l'UI de fin de secours...");

        // Nettoyer les anciens enfants invisibles
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }

        // Ajouter un fond semi-transparent
        Image background = panel.GetComponent<Image>();
        if (background == null)
        {
            background = panel.AddComponent<Image>();
        }
        background.color = new Color(0, 0, 0, 0.9f);

        // Container vertical pour centrer le contenu
        GameObject container = new GameObject("Container");
        container.transform.SetParent(panel.transform);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = new Vector2(500, 400);

        VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 20f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;

        // Titre "NIVEAU TERMINÉ"
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(container.transform);
        congratsText = titleObj.AddComponent<TextMeshProUGUI>();
        congratsText.text = "🎉 NIVEAU TERMINÉ !";
        congratsText.fontSize = 42;
        congratsText.color = Color.yellow;
        congratsText.alignment = TextAlignmentOptions.Center;
        titleObj.AddComponent<LayoutElement>().preferredHeight = 60;

        // Temps final
        GameObject timeObj = new GameObject("FinalTime");
        timeObj.transform.SetParent(container.transform);
        finalTimeText = timeObj.AddComponent<TextMeshProUGUI>();
        finalTimeText.text = $"Temps: {FormatTime(time)}";
        finalTimeText.fontSize = 36;
        finalTimeText.color = Color.white;
        finalTimeText.alignment = TextAlignmentOptions.Center;
        timeObj.AddComponent<LayoutElement>().preferredHeight = 50;

        // Champ de saisie du nom
        GameObject inputContainer = new GameObject("InputContainer");
        inputContainer.transform.SetParent(container.transform);
        RectTransform inputContainerRect = inputContainer.AddComponent<RectTransform>();
        inputContainerRect.sizeDelta = new Vector2(300, 40);
        inputContainer.AddComponent<LayoutElement>().preferredHeight = 50;

        // Background de l'input
        Image inputBg = inputContainer.AddComponent<Image>();
        inputBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Text Area pour l'input
        GameObject textArea = new GameObject("TextArea");
        textArea.transform.SetParent(inputContainer.transform);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);

        // Placeholder
        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(textArea.transform);
        TextMeshProUGUI placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Entrez votre nom...";
        placeholderText.fontSize = 18;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.alignment = TextAlignmentOptions.Left;
        RectTransform placeholderRect = placeholder.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;

        // Text du input
        GameObject inputText = new GameObject("Text");
        inputText.transform.SetParent(textArea.transform);
        TextMeshProUGUI inputTextComp = inputText.AddComponent<TextMeshProUGUI>();
        inputTextComp.fontSize = 18;
        inputTextComp.color = Color.white;
        inputTextComp.alignment = TextAlignmentOptions.Left;
        RectTransform inputTextRect = inputText.GetComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = Vector2.zero;
        inputTextRect.offsetMax = Vector2.zero;

        // TMP_InputField
        nameInputField = inputContainer.AddComponent<TMP_InputField>();
        nameInputField.textViewport = textAreaRect;
        nameInputField.textComponent = inputTextComp;
        nameInputField.placeholder = placeholderText;
        nameInputField.characterLimit = 15;

        // Boutons
        GameObject buttonsContainer = new GameObject("Buttons");
        buttonsContainer.transform.SetParent(container.transform);
        HorizontalLayoutGroup buttonsLayout = buttonsContainer.AddComponent<HorizontalLayoutGroup>();
        buttonsLayout.childAlignment = TextAnchor.MiddleCenter;
        buttonsLayout.spacing = 20f;
        buttonsLayout.childControlWidth = false;
        buttonsLayout.childControlHeight = false;
        buttonsContainer.AddComponent<LayoutElement>().preferredHeight = 50;

        // Bouton Submit
        submitButton = CreateButton(buttonsContainer.transform, "Sauvegarder", new Color(0.2f, 0.6f, 0.2f, 1f));
        submitButton.onClick.AddListener(OnSubmitClicked);

        // Bouton Menu
        skipButton = CreateButton(buttonsContainer.transform, "Menu", new Color(0.6f, 0.2f, 0.2f, 1f));
        skipButton.onClick.AddListener(OnSkipClicked);

        // Bouton Rejouer
        tryAgainButton = CreateButton(buttonsContainer.transform, "Rejouer", new Color(0.2f, 0.4f, 0.6f, 1f));
        tryAgainButton.onClick.AddListener(OnTryAgainClicked);

        Debug.Log("✅ UI de fin de secours créée avec succès");
    }

    /// <summary>
    /// Crée un bouton pour l'UI de secours
    /// </summary>
    private Button CreateButton(Transform parent, string text, Color color)
    {
        GameObject buttonObj = new GameObject(text + "Button");
        buttonObj.transform.SetParent(parent);

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = color;

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = color * 1.2f;
        colors.pressedColor = color * 0.8f;
        button.colors = colors;

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(120, 40);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }
}
