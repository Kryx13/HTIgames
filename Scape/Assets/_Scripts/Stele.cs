using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Interactive stone tablet (stele) that displays a riddle.
/// Used in Stage 4: Riddle Room.
/// Players interact to read riddles and provide answers.
/// </summary>
public class Stele : MonoBehaviour
{
    [Header("Riddle Content")]
    [SerializeField] private int riddleNumber = 1;
    [SerializeField][TextArea(3, 5)] private string riddleQuestion = "I speak without a mouth and hear without ears. I have no body, but come alive with wind. What am I?";
    [SerializeField] private string correctAnswer = "echo"; // Case-insensitive
    [SerializeField] private string[] alternativeAnswers; // Other acceptable answers

    [Header("Hints")]
    [SerializeField][TextArea(2, 3)] private string hint = "Think about sound...";
    [SerializeField] private bool showHintAfterAttempts = true;
    [SerializeField] private int attemptsBeforeHint = 2;

    [Header("Visual Settings")]
    [SerializeField] private Color unsolvedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color solvedColor = Color.green;
    [SerializeField] private float textSize = 2f;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Audio")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;

    [Header("Events")]
    [SerializeField] private UnityEvent onRiddleSolved;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private TextMeshPro riddleText;
    private Renderer steleRenderer;
    private bool isSolved = false;
    private int wrongAttempts = 0;
    private Transform player;
    private bool isPlayerNearby = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        steleRenderer = GetComponent<Renderer>();

        // Create riddle text display
        CreateRiddleText();

        // Set initial color
        UpdateVisuals();

        if (showDebugLogs)
        {
            Debug.Log($"📜 Stele {riddleNumber} initialized");
        }
    }

    private void Update()
    {
        CheckPlayerProximity();

        // Show interaction prompt when nearby
        if (isPlayerNearby && !isSolved)
        {
            if (Input.GetKeyDown(interactKey))
            {
                ShowRiddlePrompt();
            }
        }
    }

    /// <summary>
    /// Creates the riddle text above the stele
    /// </summary>
    private void CreateRiddleText()
    {
        GameObject textObj = new GameObject("RiddleText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = Vector3.up * 2f;

        riddleText = textObj.AddComponent<TextMeshPro>();
        riddleText.text = $"Riddle {riddleNumber}";
        riddleText.fontSize = textSize;
        riddleText.color = Color.white;
        riddleText.alignment = TextAlignmentOptions.Center;
        riddleText.rectTransform.sizeDelta = new Vector2(5f, 3f);
    }

    /// <summary>
    /// Check if player is within interaction range
    /// </summary>
    private void CheckPlayerProximity()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distance <= interactionRange;
    }

    /// <summary>
    /// Shows the riddle question to the player
    /// </summary>
    private void ShowRiddlePrompt()
    {
        // Try to find RiddleUI
        RiddleUI riddleUI = FindFirstObjectByType<RiddleUI>();

        if (riddleUI != null)
        {
            // Show UI with riddle
            string displayQuestion = riddleQuestion;

            // Add hint if player has tried enough times
            if (showHintAfterAttempts && wrongAttempts >= attemptsBeforeHint)
            {
                displayQuestion += $"\n\n💡 Hint: {hint}";
            }

            riddleUI.ShowRiddle(this, displayQuestion, riddleNumber);
        }
        else
        {
            // Fallback to console (for testing without UI)
            if (showDebugLogs)
            {
                Debug.Log($"📜 Riddle {riddleNumber}: {riddleQuestion}");

                if (showHintAfterAttempts && wrongAttempts >= attemptsBeforeHint)
                {
                    Debug.Log($"💡 Hint: {hint}");
                }

                Debug.Log("💬 Type your answer in the console (or add RiddleUI to scene)");
            }
        }

        riddleText.text = riddleQuestion;
    }

    /// <summary>
    /// Attempts to solve the riddle with given answer
    /// </summary>
    public void TryAnswer(string answer)
    {
        if (isSolved) return;

        answer = answer.Trim().ToLower();
        string correctLower = correctAnswer.ToLower();

        // Check correct answer
        if (answer == correctLower)
        {
            SolveRiddle();
            return;
        }

        // Check alternative answers
        if (alternativeAnswers != null)
        {
            foreach (string alt in alternativeAnswers)
            {
                if (answer == alt.ToLower())
                {
                    SolveRiddle();
                    return;
                }
            }
        }

        // Wrong answer
        WrongAnswer();
    }

    /// <summary>
    /// Called when riddle is solved correctly
    /// </summary>
    private void SolveRiddle()
    {
        isSolved = true;

        if (showDebugLogs)
        {
            Debug.Log($"✅ Riddle {riddleNumber} SOLVED!");
        }

        riddleText.text = "SOLVED!";
        riddleText.color = Color.green;

        UpdateVisuals();

        // Play success sound
        if (correctSound != null)
        {
            AudioSource.PlayClipAtPoint(correctSound, transform.position);
        }

        // Trigger event
        onRiddleSolved?.Invoke();
    }

    /// <summary>
    /// Called when answer is wrong
    /// </summary>
    private void WrongAnswer()
    {
        wrongAttempts++;

        if (showDebugLogs)
        {
            Debug.Log($"❌ Wrong answer! Attempts: {wrongAttempts}");
        }

        // Play incorrect sound
        if (incorrectSound != null)
        {
            AudioSource.PlayClipAtPoint(incorrectSound, transform.position);
        }

        // Show hint after certain attempts
        if (showHintAfterAttempts && wrongAttempts >= attemptsBeforeHint)
        {
            riddleText.text = $"Hint: {hint}";
        }
    }

    /// <summary>
    /// Updates the stele's visual appearance
    /// </summary>
    private void UpdateVisuals()
    {
        if (steleRenderer != null)
        {
            steleRenderer.material.color = isSolved ? solvedColor : unsolvedColor;
        }
    }

    /// <summary>
    /// Manual solve for testing
    /// </summary>
    [ContextMenu("Force Solve")]
    public void ForceSolve()
    {
        SolveRiddle();
    }

    /// <summary>
    /// Reset the riddle
    /// </summary>
    [ContextMenu("Reset Riddle")]
    public void ResetRiddle()
    {
        isSolved = false;
        wrongAttempts = 0;
        riddleText.text = $"Riddle {riddleNumber}";
        riddleText.color = Color.white;
        UpdateVisuals();

        if (showDebugLogs)
        {
            Debug.Log($"🔄 Riddle {riddleNumber} reset");
        }
    }

    /// <summary>
    /// Check if riddle is solved
    /// </summary>
    public bool IsSolved()
    {
        return isSolved;
    }

    /// <summary>
    /// Get the riddle number
    /// </summary>
    public int GetRiddleNumber()
    {
        return riddleNumber;
    }

    /// <summary>
    /// Gizmo to show interaction range
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isSolved ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f, $"Riddle {riddleNumber}\n{(isSolved ? "SOLVED" : "UNSOLVED")}");
#endif
    }
}
