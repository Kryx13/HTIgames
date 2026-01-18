using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Affiche le leaderboard avec les 10 meilleurs scores.
/// Attachez ce script au Canvas/Panel du Leaderboard.
/// </summary>
public class LeaderboardUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform entriesContainer; // Parent des entrées (Content d'une ScrollView)
    [SerializeField] private GameObject entryPrefab; // Prefab d'une entrée (TextMeshPro avec rang, nom, temps)
    [SerializeField] private Button backButton;

    [Header("Parent Menu")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject previousMenu;

    [Header("Auto-create entries")]
    [SerializeField] private bool autoCreateEntries = true; // Créer automatiquement les lignes si pas de prefab

    private LeaderboardManager leaderboardManager;

    private void Start()
    {
        leaderboardManager = LeaderboardManager.Instance;

        if (leaderboardManager == null)
        {
            Debug.LogError("❌ LeaderboardManager non trouvé !");
            return;
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    /// <summary>
    /// Ouvre le leaderboard et affiche les scores
    /// </summary>
    public void OpenLeaderboard(GameObject fromMenu)
    {
        previousMenu = fromMenu;

        if (previousMenu != null)
        {
            previousMenu.SetActive(false);
        }

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
        }

        RefreshLeaderboard();
    }

    /// <summary>
    /// Actualise l'affichage du leaderboard
    /// </summary>
    public void RefreshLeaderboard()
    {
        if (leaderboardManager == null) return;

        // Effacer les anciennes entrées
        ClearEntries();

        // Récupérer les scores
        List<LeaderboardEntry> entries = leaderboardManager.GetLeaderboard();

        // Afficher chaque entrée
        for (int i = 0; i < entries.Count; i++)
        {
            CreateEntryUI(i + 1, entries[i].playerName, entries[i].time);
        }

        // Si aucune entrée, afficher un message
        if (entries.Count == 0)
        {
            CreateEmptyMessage();
        }
    }

    /// <summary>
    /// Efface toutes les entrées affichées
    /// </summary>
    private void ClearEntries()
    {
        if (entriesContainer == null) return;

        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Crée une ligne d'entrée dans le leaderboard
    /// </summary>
    private void CreateEntryUI(int rank, string playerName, float time)
    {
        GameObject entryObj;

        if (entryPrefab != null)
        {
            // Utiliser le prefab
            entryObj = Instantiate(entryPrefab, entriesContainer);
        }
        else if (autoCreateEntries)
        {
            // Créer automatiquement
            entryObj = CreateAutoEntry();
        }
        else
        {
            Debug.LogWarning("⚠️ Pas de prefab et autoCreateEntries = false");
            return;
        }

        // Remplir les données
        TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 3)
        {
            texts[0].text = $"#{rank}";
            texts[1].text = playerName;
            texts[2].text = FormatTime(time);
        }
        else
        {
            // Si un seul TextMeshPro, afficher tout sur une ligne
            if (texts.Length > 0)
            {
                texts[0].text = $"#{rank}  {playerName}  {FormatTime(time)}";
            }
        }
    }

    /// <summary>
    /// Crée automatiquement une entrée simple
    /// </summary>
    private GameObject CreateAutoEntry()
    {
        GameObject entryObj = new GameObject("LeaderboardEntry");
        entryObj.transform.SetParent(entriesContainer);

        // Ajouter un Layout Horizontal
        HorizontalLayoutGroup layout = entryObj.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 10f;
        layout.padding = new RectOffset(10, 10, 5, 5);

        RectTransform rect = entryObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 30);

        // Créer 3 textes : Rang, Nom, Temps
        CreateText(entryObj, "Rank", TextAlignmentOptions.Left, 50);
        CreateText(entryObj, "Name", TextAlignmentOptions.Left, 200);
        CreateText(entryObj, "Time", TextAlignmentOptions.Right, 100);

        return entryObj;
    }

    /// <summary>
    /// Crée un TextMeshProUGUI simple
    /// </summary>
    private TextMeshProUGUI CreateText(GameObject parent, string name, TextAlignmentOptions alignment, float width)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 18;
        text.alignment = alignment;
        text.color = Color.white;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        LayoutElement layout = textObj.AddComponent<LayoutElement>();
        layout.preferredWidth = width;

        return text;
    }

    /// <summary>
    /// Affiche un message si le leaderboard est vide
    /// </summary>
    private void CreateEmptyMessage()
    {
        GameObject msgObj = new GameObject("EmptyMessage");
        msgObj.transform.SetParent(entriesContainer);

        TextMeshProUGUI text = msgObj.AddComponent<TextMeshProUGUI>();
        text.text = "Aucun score enregistré.\nSoyez le premier à terminer le temple !";
        text.fontSize = 20;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.gray;

        RectTransform rect = msgObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 100);
    }

    /// <summary>
    /// Retour au menu précédent
    /// </summary>
    private void OnBackClicked()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }

        if (previousMenu != null)
        {
            previousMenu.SetActive(true);
        }
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
