using UnityEngine;
using TMPro;

/// <summary>
/// Displays the current room number in the door maze (Stage 2).
/// Shows on-screen UI to help players track which room they're in.
/// </summary>
public class RoomNumberUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI roomNumberText;
    [SerializeField] private GameObject roomUIPanel;

    [Header("Display Settings")]
    [SerializeField] private bool showRoomName = true;
    [SerializeField] private string roomPrefix = "Room "; // "Room 1", "Room 2", etc.
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Auto-Setup")]
    [SerializeField] private bool autoCreateUI = true;
    [SerializeField] private Vector2 uiPosition = new Vector2(10, -10); // Top-left corner

    [Header("Animation")]
    [SerializeField] private bool animateOnChange = true;
    [SerializeField] private float animationDuration = 0.5f;

    private int currentRoomNumber = 1;
    private float animationTimer = 0f;

    private void Start()
    {
        // Auto-create UI if needed
        if (autoCreateUI && roomNumberText == null)
        {
            CreateRoomUI();
        }

        // Initialize with Room 1
        UpdateRoomNumber(1, "Room 1");
    }

    private void Update()
    {
        // Animation logic
        if (animateOnChange && animationTimer > 0f)
        {
            animationTimer -= Time.deltaTime;
            float t = 1f - (animationTimer / animationDuration);

            if (roomNumberText != null)
            {
                // Pulse effect
                float scale = Mathf.Lerp(1.5f, 1f, t);
                roomNumberText.transform.localScale = Vector3.one * scale;

                // Color transition
                roomNumberText.color = Color.Lerp(highlightColor, normalColor, t);
            }
        }
    }

    /// <summary>
    /// Updates the displayed room number
    /// </summary>
    public void UpdateRoomNumber(int roomNumber, string roomName = "")
    {
        if (currentRoomNumber == roomNumber && !string.IsNullOrEmpty(roomName))
        {
            return; // Already displaying this room
        }

        currentRoomNumber = roomNumber;

        if (roomNumberText != null)
        {
            if (showRoomName && !string.IsNullOrEmpty(roomName))
            {
                roomNumberText.text = roomName;
            }
            else
            {
                roomNumberText.text = $"{roomPrefix}{roomNumber}";
            }

            // Trigger animation
            if (animateOnChange)
            {
                animationTimer = animationDuration;
            }
        }
    }

    /// <summary>
    /// Creates UI automatically
    /// </summary>
    private void CreateRoomUI()
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("⚠️ No Canvas found in scene. Cannot create Room UI.");
            return;
        }

        // Create panel
        GameObject panel = new GameObject("RoomNumberPanel");
        panel.transform.SetParent(canvas.transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();

        // Position in top-left
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = uiPosition;
        panelRect.sizeDelta = new Vector2(200, 50);

        roomUIPanel = panel;

        // Create text
        GameObject textObj = new GameObject("RoomNumberText");
        textObj.transform.SetParent(panel.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        roomNumberText = textObj.AddComponent<TextMeshProUGUI>();
        roomNumberText.text = "Room 1";
        roomNumberText.fontSize = 24;
        roomNumberText.color = normalColor;
        roomNumberText.alignment = TextAlignmentOptions.Left;
        roomNumberText.fontStyle = FontStyles.Bold;

        Debug.Log("✅ Room UI created automatically");
    }

    /// <summary>
    /// Shows or hides the room UI
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (roomUIPanel != null)
        {
            roomUIPanel.SetActive(visible);
        }
    }

    /// <summary>
    /// Gets the current room number being displayed
    /// </summary>
    public int GetCurrentRoomNumber()
    {
        return currentRoomNumber;
    }
}
