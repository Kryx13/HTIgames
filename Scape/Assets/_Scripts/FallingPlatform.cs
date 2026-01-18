using UnityEngine;
using System.Collections;

/// <summary>
/// Platform that falls after a delay when the player walks on it.
/// The platform resets when the player returns to the room.
/// Used for Stage 1: Falling Platforms.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallDelay = 3f; // Time before falling (in seconds)
    [SerializeField] private float resetDelay = 5f; // Time before automatic reset
    [SerializeField] private bool autoReset = true; // Automatically resets after a time

    [Header("Visual Feedback")]
    [SerializeField] private bool shakeBeforeFall = true;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color warningColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioClip activateSound; // Sound when player walks on it
    [SerializeField] private AudioClip fallSound; // Falling sound

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showTimer = true; // Display timer above the platform

    private Rigidbody rb;
    private Renderer platformRenderer;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isActivated = false;
    private bool isFalling = false;
    private float timer = 0f;
    private Material platformMaterial;
    private Color originalColor;
    private TextMesh timerText; // 3D text to display the timer

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        platformRenderer = GetComponent<Renderer>();

        // Save initial position for reset
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Rigidbody kinematic at start (doesn't fall)
        rb.isKinematic = true;
        rb.useGravity = false;

        // Save original color
        if (platformRenderer != null)
        {
            platformMaterial = platformRenderer.material;
            originalColor = platformMaterial.color;
        }

        // Create timer text if requested
        if (showTimer)
        {
            CreateTimerText();
        }

        if (showDebugLogs)
        {
            Debug.Log($"✅ FallingPlatform '{gameObject.name}' initialized");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detect when the player walks on the platform
        if (collision.gameObject.CompareTag("Player") && !isActivated && !isFalling)
        {
            Activate();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Continue detecting if the player stays on the platform
        if (collision.gameObject.CompareTag("Player") && !isActivated && !isFalling)
        {
            Activate();
        }
    }

    private void Update()
    {
        // Countdown if activated
        if (isActivated && !isFalling)
        {
            timer -= Time.deltaTime;

            // Update timer text
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timer).ToString();
            }

            // Change color progressively
            if (platformMaterial != null)
            {
                float t = 1f - (timer / fallDelay);
                platformMaterial.color = Color.Lerp(originalColor, warningColor, t);
            }

            // Time elapsed → make it fall
            if (timer <= 0f)
            {
                Fall();
            }
        }
    }

    /// <summary>
    /// Activates the platform countdown
    /// </summary>
    private void Activate()
    {
        if (isActivated || isFalling) return;

        isActivated = true;
        timer = fallDelay;

        if (showDebugLogs)
        {
            Debug.Log($"⏱️ Platform '{gameObject.name}' activated! Falling in {fallDelay}s");
        }

        // Activation sound
        if (activateSound != null)
        {
            AudioSource.PlayClipAtPoint(activateSound, transform.position);
        }

        // Shake animation
        if (shakeBeforeFall)
        {
            StartCoroutine(ShakeBeforeFall());
        }

        // Show timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Makes the platform fall
    /// </summary>
    private void Fall()
    {
        if (isFalling) return;

        isFalling = true;
        isActivated = false;

        if (showDebugLogs)
        {
            Debug.Log($"💥 Platform '{gameObject.name}' is falling!");
        }

        // Enable physics for the fall
        rb.isKinematic = false;
        rb.useGravity = true;

        // Fall sound
        if (fallSound != null)
        {
            AudioSource.PlayClipAtPoint(fallSound, transform.position);
        }

        // Hide timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // Auto-reset after a delay
        if (autoReset)
        {
            StartCoroutine(ResetAfterDelay());
        }
    }

    /// <summary>
    /// Resets the platform to its original position
    /// </summary>
    public void ResetPlatform()
    {
        if (showDebugLogs)
        {
            Debug.Log($"🔄 Platform '{gameObject.name}' reset");
        }

        // Disable physics
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Return to initial position
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Reset state
        isActivated = false;
        isFalling = false;
        timer = fallDelay;

        // Restore color
        if (platformMaterial != null)
        {
            platformMaterial.color = originalColor;
        }

        // Hide timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine for shaking before falling
    /// </summary>
    private IEnumerator ShakeBeforeFall()
    {
        float elapsed = 0f;
        Vector3 originalPos = transform.position;

        while (elapsed < shakeDuration && isActivated && !isFalling)
        {
            // Random oscillation
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetZ = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = originalPos + new Vector3(offsetX, 0, offsetZ);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to initial position
        transform.position = originalPos;
    }

    /// <summary>
    /// Coroutine to automatically reset after a delay
    /// </summary>
    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);
        ResetPlatform();
    }

    /// <summary>
    /// Creates a 3D text displaying the countdown
    /// </summary>
    private void CreateTimerText()
    {
        GameObject textObj = new GameObject("TimerText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 1.5f, 0); // Above the platform
        textObj.transform.localRotation = Quaternion.Euler(90, 0, 0); // Facing the sky

        timerText = textObj.AddComponent<TextMesh>();
        timerText.text = "";
        timerText.fontSize = 50;
        timerText.color = Color.white;
        timerText.anchor = TextAnchor.MiddleCenter;
        timerText.alignment = TextAlignment.Center;
        timerText.characterSize = 0.1f;

        // Hide at start
        textObj.SetActive(false);
    }

    /// <summary>
    /// Visualization in the editor
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = isFalling ? Color.red : (isActivated ? Color.yellow : Color.green);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    private void OnDrawGizmosSelected()
    {
        // Show initial position
        Gizmos.color = Color.cyan;
        Vector3 startPos = Application.isPlaying ? initialPosition : transform.position;
        Gizmos.DrawWireCube(startPos, transform.localScale);
    }

}
