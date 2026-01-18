using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI for displaying riddles and accepting player answers.
/// Shows riddle question, input field, and submit button.
/// </summary>
public class RiddleUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject riddlePanel;
    [SerializeField] private TextMeshProUGUI riddleNumberText;
    [SerializeField] private TextMeshProUGUI riddleQuestionText;
    [SerializeField] private TMP_InputField answerInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Auto-Create UI")]
    [SerializeField] private bool autoCreateUI = true;

    [Header("Settings")]
    [SerializeField] private float feedbackDisplayTime = 2f;

    private Stele currentStele;
    private float feedbackTimer = 0f;

    private void Start()
    {
        if (autoCreateUI && riddlePanel == null)
        {
            CreateRiddleUI();
        }

        // Hide panel initially
        if (riddlePanel != null)
        {
            riddlePanel.SetActive(false);
        }

        // Setup submit button listener
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitAnswer);
        }

        // Setup input field submit on Enter key
        if (answerInputField != null)
        {
            answerInputField.onSubmit.AddListener((text) => OnSubmitAnswer());
        }
    }

    private void Update()
    {
        // Hide feedback after delay
        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f && feedbackText != null)
            {
                feedbackText.text = "";
            }
        }
    }

    /// <summary>
    /// Shows the riddle UI for a specific stele
    /// </summary>
    public void ShowRiddle(Stele stele, string question, int riddleNumber)
    {
        currentStele = stele;

        if (riddlePanel != null)
        {
            riddlePanel.SetActive(true);
        }

        if (riddleNumberText != null)
        {
            riddleNumberText.text = $"Riddle {riddleNumber}";
        }

        if (riddleQuestionText != null)
        {
            riddleQuestionText.text = question;
        }

        if (answerInputField != null)
        {
            answerInputField.text = "";
            answerInputField.ActivateInputField();
        }

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        // Unlock cursor for typing
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Hides the riddle UI
    /// </summary>
    public void HideRiddle()
    {
        if (riddlePanel != null)
        {
            riddlePanel.SetActive(false);
        }

        currentStele = null;

        // Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Called when player submits an answer
    /// </summary>
    private void OnSubmitAnswer()
    {
        if (currentStele == null || answerInputField == null)
        {
            return;
        }

        string answer = answerInputField.text.Trim();

        if (string.IsNullOrEmpty(answer))
        {
            ShowFeedback("Please enter an answer!", Color.yellow);
            return;
        }

        // Try the answer
        currentStele.TryAnswer(answer);

        // Check if solved
        if (currentStele.IsSolved())
        {
            ShowFeedback("Correct! Well done!", Color.green);
            Invoke(nameof(HideRiddle), 2f);
        }
        else
        {
            ShowFeedback("Incorrect. Try again!", Color.red);
            answerInputField.text = "";
            answerInputField.ActivateInputField();
        }
    }

    /// <summary>
    /// Shows feedback message
    /// </summary>
    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            feedbackTimer = feedbackDisplayTime;
        }
    }

    /// <summary>
    /// Auto-creates the riddle UI
    /// </summary>
    private void CreateRiddleUI()
    {
        // Create Canvas if needed
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create panel
        GameObject panelObj = new GameObject("RiddlePanel");
        panelObj.transform.SetParent(canvas.transform, false);
        riddlePanel = panelObj;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(600f, 400f);
        panelRect.anchoredPosition = Vector2.zero;

        // Create riddle number text
        GameObject numberTextObj = new GameObject("RiddleNumberText");
        numberTextObj.transform.SetParent(panelObj.transform, false);
        riddleNumberText = numberTextObj.AddComponent<TextMeshProUGUI>();
        riddleNumberText.text = "Riddle 1";
        riddleNumberText.fontSize = 28;
        riddleNumberText.color = Color.cyan;
        riddleNumberText.alignment = TextAlignmentOptions.Center;

        RectTransform numberRect = numberTextObj.GetComponent<RectTransform>();
        numberRect.anchorMin = new Vector2(0.5f, 1f);
        numberRect.anchorMax = new Vector2(0.5f, 1f);
        numberRect.sizeDelta = new Vector2(500f, 40f);
        numberRect.anchoredPosition = new Vector2(0f, -30f);

        // Create question text
        GameObject questionTextObj = new GameObject("RiddleQuestionText");
        questionTextObj.transform.SetParent(panelObj.transform, false);
        riddleQuestionText = questionTextObj.AddComponent<TextMeshProUGUI>();
        riddleQuestionText.text = "Question goes here...";
        riddleQuestionText.fontSize = 20;
        riddleQuestionText.color = Color.white;
        riddleQuestionText.alignment = TextAlignmentOptions.Center;
        riddleQuestionText.textWrappingMode = TMPro.TextWrappingModes.Normal;

        RectTransform questionRect = questionTextObj.GetComponent<RectTransform>();
        questionRect.anchorMin = new Vector2(0.5f, 0.5f);
        questionRect.anchorMax = new Vector2(0.5f, 0.5f);
        questionRect.sizeDelta = new Vector2(550f, 150f);
        questionRect.anchoredPosition = new Vector2(0f, 50f);

        // Create input field
        GameObject inputObj = new GameObject("AnswerInputField");
        inputObj.transform.SetParent(panelObj.transform, false);
        answerInputField = inputObj.AddComponent<TMP_InputField>();

        Image inputImage = inputObj.AddComponent<Image>();
        inputImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        RectTransform inputRect = inputObj.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.5f, 0.5f);
        inputRect.anchorMax = new Vector2(0.5f, 0.5f);
        inputRect.sizeDelta = new Vector2(500f, 40f);
        inputRect.anchoredPosition = new Vector2(0f, -50f);

        // Create text child for input field
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        TextMeshProUGUI inputText = textObj.AddComponent<TextMeshProUGUI>();
        inputText.fontSize = 18;
        inputText.color = Color.white;
        answerInputField.textComponent = inputText;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10f, 5f);
        textRect.offsetMax = new Vector2(-10f, -5f);

        // Create placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Type your answer...";
        placeholderText.fontSize = 18;
        placeholderText.color = new Color(1f, 1f, 1f, 0.5f);
        answerInputField.placeholder = placeholderText;

        RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(10f, 5f);
        placeholderRect.offsetMax = new Vector2(-10f, -5f);

        // Create submit button
        GameObject buttonObj = new GameObject("SubmitButton");
        buttonObj.transform.SetParent(panelObj.transform, false);
        submitButton = buttonObj.AddComponent<Button>();

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f, 1f);

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(200f, 50f);
        buttonRect.anchoredPosition = new Vector2(0f, -120f);

        // Create button text
        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Submit";
        buttonText.fontSize = 22;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;

        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        // Create feedback text
        GameObject feedbackObj = new GameObject("FeedbackText");
        feedbackObj.transform.SetParent(panelObj.transform, false);
        feedbackText = feedbackObj.AddComponent<TextMeshProUGUI>();
        feedbackText.text = "";
        feedbackText.fontSize = 18;
        feedbackText.color = Color.white;
        feedbackText.alignment = TextAlignmentOptions.Center;

        RectTransform feedbackRect = feedbackObj.GetComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.5f, 0f);
        feedbackRect.anchorMax = new Vector2(0.5f, 0f);
        feedbackRect.sizeDelta = new Vector2(500f, 40f);
        feedbackRect.anchoredPosition = new Vector2(0f, 30f);

        Debug.Log("✅ Riddle UI created automatically");
    }
}
