using UnityEngine;

/// <summary>
/// Manages the riddle room (Stage 4).
/// Tracks completion of all riddles and opens the exit.
/// </summary>
public class RiddleManager : MonoBehaviour
{
    [Header("Riddles")]
    [SerializeField] private Stele[] riddles; // All riddles in the room
    [SerializeField] private int riddlesRequired = 3; // How many must be solved

    [Header("Exit")]
    [SerializeField] private GameObject exitPlatform; // Platform that appears when riddles solved
    [SerializeField] private GameObject exitDoor; // Door to next stage
    [SerializeField] private bool deactivateExitOnStart = true;

    [Header("Hints")]
    [SerializeField] private bool provideProgressHints = true;

    [Header("Audio")]
    [SerializeField] private AudioClip roomCompleteSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private int riddlesSolved = 0;
    private bool roomComplete = false;

    private void Start()
    {
        // Auto-find riddles if not assigned
        if (riddles == null || riddles.Length == 0)
        {
            riddles = GetComponentsInChildren<Stele>();
        }

        // Subscribe to each riddle's solve event
        foreach (Stele riddle in riddles)
        {
            if (riddle != null)
            {
                // Use reflection to add listener to UnityEvent
                var onSolved = riddle.GetType().GetField("onRiddleSolved", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (onSolved != null)
                {
                    var unityEvent = onSolved.GetValue(riddle) as UnityEngine.Events.UnityEvent;
                    if (unityEvent != null)
                    {
                        unityEvent.AddListener(OnRiddleSolved);
                    }
                }
            }
        }

        // Hide exit initially
        if (deactivateExitOnStart)
        {
            if (exitPlatform != null)
            {
                exitPlatform.SetActive(false);
            }
            if (exitDoor != null)
            {
                exitDoor.SetActive(false);
            }
        }

        if (showDebugLogs)
        {
            Debug.Log($"📜 Riddle Room initialized with {riddles.Length} riddles ({riddlesRequired} required)");
        }
    }

    private void Update()
    {
        // Manual check for completion (backup)
        if (!roomComplete)
        {
            CheckCompletion();
        }
    }

    /// <summary>
    /// Called when any riddle is solved
    /// </summary>
    private void OnRiddleSolved()
    {
        riddlesSolved++;

        if (showDebugLogs)
        {
            Debug.Log($"📊 Riddles solved: {riddlesSolved}/{riddlesRequired}");
        }

        // Provide progress hint
        if (provideProgressHints)
        {
            int remaining = riddlesRequired - riddlesSolved;
            if (remaining > 0)
            {
                Debug.Log($"💡 {remaining} more riddle(s) to solve!");
            }
        }

        // Check if room is complete
        if (riddlesSolved >= riddlesRequired && !roomComplete)
        {
            CompleteRoom();
        }
    }

    /// <summary>
    /// Manually checks if enough riddles are solved
    /// </summary>
    private void CheckCompletion()
    {
        int solvedCount = 0;
        foreach (Stele riddle in riddles)
        {
            if (riddle != null && riddle.IsSolved())
            {
                solvedCount++;
            }
        }

        if (solvedCount >= riddlesRequired && !roomComplete)
        {
            riddlesSolved = solvedCount;
            CompleteRoom();
        }
    }

    /// <summary>
    /// Completes the riddle room
    /// </summary>
    private void CompleteRoom()
    {
        roomComplete = true;

        if (showDebugLogs)
        {
            Debug.Log("🎯 RIDDLE ROOM COMPLETE! Exit unlocked.");
        }

        // Activate exit platform
        if (exitPlatform != null)
        {
            exitPlatform.SetActive(true);

            // Flash effect
            Renderer renderer = exitPlatform.GetComponent<Renderer>();
            if (renderer != null)
            {
                StartCoroutine(FlashObject(renderer));
            }
        }

        // Activate exit door
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }

        // Play sound
        if (roomCompleteSound != null)
        {
            AudioSource.PlayClipAtPoint(roomCompleteSound, transform.position);
        }
    }

    /// <summary>
    /// Flashes an object's color
    /// </summary>
    private System.Collections.IEnumerator FlashObject(Renderer renderer)
    {
        Color originalColor = renderer.material.color;
        Color flashColor = Color.cyan;

        for (int i = 0; i < 5; i++)
        {
            renderer.material.color = flashColor;
            yield return new WaitForSeconds(0.2f);
            renderer.material.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }

        // Final color: cyan
        renderer.material.color = flashColor;
    }

    /// <summary>
    /// Gets progress (0-1)
    /// </summary>
    public float GetProgress()
    {
        if (riddlesRequired == 0) return 0f;
        return (float)riddlesSolved / riddlesRequired;
    }

    /// <summary>
    /// Resets the riddle room (for testing)
    /// </summary>
    [ContextMenu("Reset Room")]
    public void ResetRoom()
    {
        riddlesSolved = 0;
        roomComplete = false;

        // Reset all riddles
        foreach (Stele riddle in riddles)
        {
            if (riddle != null)
            {
                riddle.ResetRiddle();
            }
        }

        // Hide exit
        if (exitPlatform != null)
        {
            exitPlatform.SetActive(false);
        }
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }

        Debug.Log("🔄 Riddle Room reset");
    }

    /// <summary>
    /// Gizmo to visualize room structure
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = roomComplete ? Color.green : Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 3f);

        // Draw lines to exit
        if (exitPlatform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, exitPlatform.transform.position);
        }
        if (exitDoor != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, exitDoor.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 5f,
            $"RIDDLE ROOM\n{riddlesSolved}/{riddlesRequired} Solved\nProgress: {GetProgress() * 100:F0}%");
#endif
    }
}
