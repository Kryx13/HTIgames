using UnityEngine;

/// <summary>
/// Displays a 2D map of the maze on a wall.
/// Shows the correct path (destructible) in green and solid walls in black.
/// The player can consult this map to navigate through the maze.
/// </summary>
public class MapDisplay : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int mazeWidth = 5;
    [SerializeField] private int mazeLength = 20;
    [SerializeField] private int[] correctPath; // Must match MazeBuilder

    [Header("Visual")]
    [SerializeField] private int textureWidth = 100; // Texture resolution (pixels)
    [SerializeField] private int textureHeight = 400;
    [SerializeField] private Color pathColor = Color.green; // Correct path
    [SerializeField] private Color wallColor = Color.black; // Indestructible walls
    [SerializeField] private Color backgroundColor = new Color(0.8f, 0.8f, 0.7f); // Background (parchment)

    [Header("Display")]
    [SerializeField] private bool autoCreateMap = true;
    [SerializeField] private Vector3 mapScale = new Vector3(2f, 4f, 0.1f); // Size of the quad displaying the map

    private Texture2D mapTexture;
    private GameObject mapQuad;

    private void Start()
    {
        if (autoCreateMap)
        {
            GenerateMapTexture();
            CreateMapDisplay();
        }
    }

    /// <summary>
    /// Generates the map texture
    /// </summary>
    [ContextMenu("Generate Map Texture")]
    public void GenerateMapTexture()
    {
        if (correctPath == null || correctPath.Length != mazeLength)
        {
            Debug.LogError("❌ correctPath must have the same length as mazeLength!");
            return;
        }

        // Create texture
        mapTexture = new Texture2D(textureWidth, textureHeight);
        mapTexture.filterMode = FilterMode.Point; // Sharp pixels

        // Fill background
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        mapTexture.SetPixels(pixels);

        // Calculate cell size
        int cellWidth = textureWidth / mazeWidth;
        int cellHeight = textureHeight / mazeLength;

        // Draw the maze
        for (int z = 0; z < mazeLength; z++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                // Pixel coordinates (invert Y so 0 is at bottom)
                int startX = x * cellWidth;
                int startY = (mazeLength - 1 - z) * cellHeight;

                Color cellColor = (x == correctPath[z]) ? pathColor : wallColor;

                // Fill cell
                for (int py = startY; py < startY + cellHeight; py++)
                {
                    for (int px = startX; px < startX + cellWidth; px++)
                    {
                        if (px < textureWidth && py < textureHeight)
                        {
                            mapTexture.SetPixel(px, py, cellColor);
                        }
                    }
                }

                // Borders between cells (grid)
                Color gridColor = new Color(0.5f, 0.5f, 0.5f); // Gray
                for (int px = startX; px < startX + cellWidth; px++)
                {
                    if (px < textureWidth)
                    {
                        mapTexture.SetPixel(px, startY, gridColor); // Horizontal line
                    }
                }
                for (int py = startY; py < startY + cellHeight; py++)
                {
                    if (py < textureHeight)
                    {
                        mapTexture.SetPixel(startX, py, gridColor); // Vertical line
                    }
                }
            }
        }

        // Apply changes
        mapTexture.Apply();

        Debug.Log("✅ Map texture generated!");
    }

    /// <summary>
    /// Creates a quad to display the map
    /// </summary>
    private void CreateMapDisplay()
    {
        if (mapTexture == null)
        {
            Debug.LogWarning("⚠️ Map texture not generated!");
            return;
        }

        // Create a quad
        mapQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mapQuad.name = "MapDisplay";
        mapQuad.transform.SetParent(transform);
        mapQuad.transform.localPosition = Vector3.zero;
        mapQuad.transform.localRotation = Quaternion.identity;
        mapQuad.transform.localScale = mapScale;

        // Apply texture
        Renderer renderer = mapQuad.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Unlit/Texture"));
        renderer.material.mainTexture = mapTexture;

        // Remove collider (not necessary)
        Destroy(mapQuad.GetComponent<Collider>());

        Debug.Log("✅ Map displayed on wall!");
    }

    /// <summary>
    /// Regenerates the map (useful if the path changes)
    /// </summary>
    [ContextMenu("Regenerate Map")]
    public void RegenerateMap()
    {
        if (mapQuad != null)
        {
            DestroyImmediate(mapQuad);
        }

        GenerateMapTexture();
        CreateMapDisplay();
    }

    /// <summary>
    /// Copies the path from a MazeBuilder
    /// </summary>
    [ContextMenu("Copy Path from MazeBuilder")]
    public void CopyPathFromMazeBuilder()
    {
        MazeBuilder builder = FindObjectOfType<MazeBuilder>();
        if (builder != null)
        {
            // Use reflection to access the private correctPath field
            var field = builder.GetType().GetField("correctPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                int[] path = (int[])field.GetValue(builder);
                correctPath = (int[])path.Clone();
                Debug.Log("✅ Path copied from MazeBuilder!");
            }
            else
            {
                Debug.LogWarning("⚠️ Unable to access path from MazeBuilder");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No MazeBuilder found in scene");
        }
    }

    /// <summary>
    /// Manually sets the correct path
    /// </summary>
    public void SetCorrectPath(int[] path)
    {
        correctPath = path;
        mazeLength = path.Length;
        RegenerateMap();
    }

    /// <summary>
    /// Gizmo to visualize the map location
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, mapScale);
    }
}
