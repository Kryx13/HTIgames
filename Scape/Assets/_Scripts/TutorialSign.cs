using UnityEngine;
using TMPro;

/// <summary>
/// Tutorial panel with 3D text in the world.
/// Displays instructions to help the player.
/// The text always faces the player (Billboard).
/// </summary>
public class TutorialSign : MonoBehaviour
{
    [Header("Content")]
    [SerializeField][TextArea(3, 10)] private string tutorialText = "Use WASD to move";

    [Header("Appearance")]
    [SerializeField] private float textSize = 0.5f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.7f);
    [SerializeField] private bool billboard = true; // Always face the player

    [Header("Auto-Setup")]
    [SerializeField] private bool autoCreateText = true;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 1.5f, 0); // Text height

    private TextMeshPro textMesh;
    private Transform playerCamera;
    private GameObject backgroundPanel;

    private void Start()
    {
        // Find the player's camera for billboard
        if (billboard)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                playerCamera = mainCam.transform;
            }
        }

        // Create text automatically if needed
        if (autoCreateText && textMesh == null)
        {
            CreateTutorialText();
        }
    }

    private void Update()
    {
        // Face the player (billboard)
        if (billboard && playerCamera != null && textMesh != null)
        {
            textMesh.transform.LookAt(playerCamera);
            textMesh.transform.Rotate(0, 180, 0); // Flip so text is readable
        }
    }

    /// <summary>
    /// Automatically creates a TextMeshPro in the world
    /// </summary>
    private void CreateTutorialText()
    {
        // Create a child object for the text
        GameObject textObj = new GameObject("TutorialText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = textOffset;
        textObj.transform.localRotation = Quaternion.identity;

        // Add TextMeshPro
        textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.text = tutorialText;
        textMesh.fontSize = textSize;
        textMesh.color = textColor;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontStyle = FontStyles.Bold;

        // Enable face rendering (visible from both sides)
        textMesh.fontSharedMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.3f);

        // Create dark background for better readability
        CreateBackground(textObj.transform);

        Debug.Log($"✅ Tutorial Sign created: '{tutorialText}'");
    }

    /// <summary>
    /// Creates a dark background panel behind the text
    /// </summary>
    private void CreateBackground(Transform parent)
    {
        backgroundPanel = GameObject.CreatePrimitive(PrimitiveType.Quad);
        backgroundPanel.name = "Background";
        backgroundPanel.transform.SetParent(parent);
        backgroundPanel.transform.localPosition = new Vector3(0, 0, 0.01f); // Slightly behind
        backgroundPanel.transform.localRotation = Quaternion.identity;
        backgroundPanel.transform.localScale = new Vector3(2f, 1f, 1f); // Adjust to text size

        // Transparent black material
        Renderer renderer = backgroundPanel.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = backgroundColor;
        }

        // No collider needed
        Destroy(backgroundPanel.GetComponent<Collider>());
    }

    /// <summary>
    /// Changes the panel text (useful for dynamic tutorials)
    /// </summary>
    public void SetText(string newText)
    {
        tutorialText = newText;
        if (textMesh != null)
        {
            textMesh.text = newText;
        }
    }

    /// <summary>
    /// Shows or hides the panel
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(visible);
        }
    }

    // Display a gizmo in the editor for easier placement
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + textOffset, new Vector3(2f, 1f, 0.1f));
    }
}
