using UnityEngine;

/// <summary>
/// Manages the shooting gallery (Stage 3).
/// Tracks completion of all 5 target sequences and opens the exit door.
/// </summary>
public class ShootingGalleryManager : MonoBehaviour
{
    [Header("Sequences")]
    [SerializeField] private TargetSequence[] sequences; // All 5 sequences

    [Header("Exit")]
    [SerializeField] private GameObject exitDoor; // Door to Stage 4
    [SerializeField] private bool deactivateExitOnStart = true;

    [Header("UI")]
#pragma warning disable 0414 // Field assigned but never used (reserved for future progress UI)
    [SerializeField] private bool showProgressUI = true;
#pragma warning restore 0414

    [Header("Audio")]
    [SerializeField] private AudioClip galleryCompleteSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private int sequencesCompleted = 0;
    private int totalSequences = 5;
    private bool galleryComplete = false;

    private void Start()
    {
        // Auto-find sequences if not assigned
        if (sequences == null || sequences.Length == 0)
        {
            sequences = GetComponentsInChildren<TargetSequence>();
        }

        totalSequences = sequences.Length;

        // Subscribe to sequence complete events
        foreach (TargetSequence sequence in sequences)
        {
            if (sequence != null)
            {
                // Use reflection to add listener to UnityEvent
                var onComplete = sequence.GetType().GetField("onSequenceComplete", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (onComplete != null)
                {
                    var unityEvent = onComplete.GetValue(sequence) as UnityEngine.Events.UnityEvent;
                    if (unityEvent != null)
                    {
                        unityEvent.AddListener(OnSequenceComplete);
                    }
                }
            }
        }

        // Hide exit door initially
        if (deactivateExitOnStart && exitDoor != null)
        {
            exitDoor.SetActive(false);
        }

        if (showDebugLogs)
        {
            Debug.Log($"✅ Shooting Gallery initialized with {totalSequences} sequences");
        }
    }

    private void Update()
    {
        // Check for manual completion (in case events don't fire)
        if (!galleryComplete)
        {
            CheckCompletion();
        }
    }

    /// <summary>
    /// Called when any sequence is completed
    /// </summary>
    private void OnSequenceComplete()
    {
        sequencesCompleted++;

        if (showDebugLogs)
        {
            Debug.Log($"📊 Sequences completed: {sequencesCompleted}/{totalSequences}");
        }

        // Check if all sequences are done
        if (sequencesCompleted >= totalSequences && !galleryComplete)
        {
            CompleteGallery();
        }
    }

    /// <summary>
    /// Manually checks if all sequences are complete
    /// </summary>
    private void CheckCompletion()
    {
        int completedCount = 0;
        foreach (TargetSequence sequence in sequences)
        {
            if (sequence != null && sequence.IsComplete())
            {
                completedCount++;
            }
        }

        if (completedCount >= totalSequences && !galleryComplete)
        {
            sequencesCompleted = completedCount;
            CompleteGallery();
        }
    }

    /// <summary>
    /// Completes the entire shooting gallery
    /// </summary>
    private void CompleteGallery()
    {
        galleryComplete = true;

        if (showDebugLogs)
        {
            Debug.Log("🎯 SHOOTING GALLERY COMPLETE! Exit door opened.");
        }

        // Open exit door
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);

            // Flash the door
            Renderer renderer = exitDoor.GetComponent<Renderer>();
            if (renderer != null)
            {
                StartCoroutine(FlashExit(renderer));
            }
        }

        // Play sound
        if (galleryCompleteSound != null)
        {
            AudioSource.PlayClipAtPoint(galleryCompleteSound, transform.position);
        }
    }

    /// <summary>
    /// Flashes the exit door
    /// </summary>
    private System.Collections.IEnumerator FlashExit(Renderer renderer)
    {
        Color originalColor = renderer.material.color;
        Color flashColor = Color.green;

        for (int i = 0; i < 5; i++)
        {
            renderer.material.color = flashColor;
            yield return new WaitForSeconds(0.2f);
            renderer.material.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }

        // Final color: green
        renderer.material.color = flashColor;
    }

    /// <summary>
    /// Gets overall gallery progress (0-1)
    /// </summary>
    public float GetOverallProgress()
    {
        if (totalSequences == 0) return 0f;

        float totalProgress = 0f;
        foreach (TargetSequence sequence in sequences)
        {
            if (sequence != null)
            {
                totalProgress += sequence.GetProgress();
            }
        }

        return totalProgress / totalSequences;
    }

    /// <summary>
    /// Resets the entire gallery (for testing)
    /// </summary>
    [ContextMenu("Reset Gallery")]
    public void ResetGallery()
    {
        sequencesCompleted = 0;
        galleryComplete = false;

        // Reset all sequences
        foreach (TargetSequence sequence in sequences)
        {
            if (sequence != null)
            {
                sequence.ResetSequence();
            }
        }

        // Close exit door
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }

        Debug.Log("🔄 Shooting Gallery reset");
    }

    /// <summary>
    /// Gizmo to visualize the gallery structure
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = galleryComplete ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);

        // Draw line to exit
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
        UnityEditor.Handles.Label(transform.position + Vector3.up * 4f,
            $"SHOOTING GALLERY\n{sequencesCompleted}/{totalSequences} Complete\nProgress: {GetOverallProgress() * 100:F0}%");
#endif
    }
}
