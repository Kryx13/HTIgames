using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages a sequence of targets for Stage 3: Shooting Gallery.
/// When all targets in the sequence are destroyed, activates a platform or triggers an event.
/// </summary>
public class TargetSequence : MonoBehaviour
{
    [Header("Sequence Settings")]
    [SerializeField] private int sequenceNumber = 1;
    [SerializeField] private string sequenceName = "Sequence 1";
    [SerializeField] private Target[] targets; // All targets in this sequence

    [Header("Activation")]
    [SerializeField] private GameObject platformToActivate; // Platform that appears when sequence completes
    [SerializeField] private bool deactivateOnStart = true; // Hide platform initially

    [Header("Events")]
    [SerializeField] private UnityEvent onSequenceComplete; // Event when all targets destroyed

    [Header("Audio")]
    [SerializeField] private AudioClip completeSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private int targetsDestroyed = 0;
    private int totalTargets = 0;
    private bool sequenceComplete = false;

    private void Start()
    {
        // Auto-find targets if not assigned
        if (targets == null || targets.Length == 0)
        {
            targets = GetComponentsInChildren<Target>();
        }

        totalTargets = targets.Length;

        // Subscribe to each target's destroy event
        foreach (Target target in targets)
        {
            if (target != null)
            {
                target.OnDestroyed.AddListener(OnTargetDestroyed);
            }
        }

        // Hide platform initially
        if (deactivateOnStart && platformToActivate != null)
        {
            platformToActivate.SetActive(false);
        }

        if (showDebugLogs)
        {
            Debug.Log($"✅ {sequenceName} initialized with {totalTargets} targets");
        }
    }

    /// <summary>
    /// Called when a target in this sequence is destroyed
    /// </summary>
    private void OnTargetDestroyed()
    {
        targetsDestroyed++;

        if (showDebugLogs)
        {
            Debug.Log($"🎯 {sequenceName}: {targetsDestroyed}/{totalTargets} targets destroyed");
        }

        // Check if sequence is complete
        if (targetsDestroyed >= totalTargets && !sequenceComplete)
        {
            CompleteSequence();
        }
    }

    /// <summary>
    /// Completes the sequence and activates the platform
    /// </summary>
    private void CompleteSequence()
    {
        sequenceComplete = true;

        if (showDebugLogs)
        {
            Debug.Log($"✅ {sequenceName} COMPLETE! Platform activated.");
        }

        // Activate platform
        if (platformToActivate != null)
        {
            platformToActivate.SetActive(true);

            // Optional: Add visual effect (particles, glow, etc.)
            AddActivationEffect();
        }

        // Play sound
        if (completeSound != null)
        {
            AudioSource.PlayClipAtPoint(completeSound, transform.position);
        }

        // Trigger event
        onSequenceComplete?.Invoke();
    }

    /// <summary>
    /// Adds a visual effect when platform activates
    /// </summary>
    private void AddActivationEffect()
    {
        if (platformToActivate == null) return;

        // Flash the platform material
        Renderer renderer = platformToActivate.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Temporarily change color
            StartCoroutine(FlashPlatform(renderer));
        }
    }

    /// <summary>
    /// Flashes the platform color briefly
    /// </summary>
    private System.Collections.IEnumerator FlashPlatform(Renderer renderer)
    {
        Color originalColor = renderer.material.color;
        Color flashColor = Color.green;

        for (int i = 0; i < 3; i++)
        {
            renderer.material.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            renderer.material.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Resets the sequence (for testing or retry)
    /// </summary>
    [ContextMenu("Reset Sequence")]
    public void ResetSequence()
    {
        targetsDestroyed = 0;
        sequenceComplete = false;

        // Respawn all targets
        foreach (Target target in targets)
        {
            if (target != null)
            {
                // Reset target health (via reflection or public method if available)
                target.gameObject.SetActive(true);
            }
        }

        // Deactivate platform
        if (platformToActivate != null)
        {
            platformToActivate.SetActive(false);
        }

        Debug.Log($"🔄 {sequenceName} reset");
    }

    /// <summary>
    /// Checks if sequence is complete
    /// </summary>
    public bool IsComplete()
    {
        return sequenceComplete;
    }

    /// <summary>
    /// Gets progress percentage
    /// </summary>
    public float GetProgress()
    {
        if (totalTargets == 0) return 0f;
        return (float)targetsDestroyed / totalTargets;
    }

    /// <summary>
    /// Gizmo to visualize sequence bounds
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = sequenceComplete ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 5f);

        // Draw line to platform
        if (platformToActivate != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, platformToActivate.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f, $"{sequenceName}\n{targetsDestroyed}/{totalTargets}");
#endif
    }
}
