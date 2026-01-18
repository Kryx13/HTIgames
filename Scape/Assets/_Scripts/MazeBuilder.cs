using UnityEngine;

/// <summary>
/// Destructible maze generator 5x20 for Stage 5.
/// Automatically creates destructible and indestructible walls.
/// The correct path is marked as destructible, the rest is indestructible.
/// </summary>
[ExecuteInEditMode]
public class MazeBuilder : MonoBehaviour
{
    [Header("Maze Dimensions")]
    [SerializeField] private int width = 5; // Width (X)
    [SerializeField] private int length = 20; // Length (Z)
    [SerializeField] private float blockSize = 2f; // Size of each block
    [SerializeField] private float blockHeight = 3f;

    [Header("Path Definition")]
    [Tooltip("Define the correct path (list of Z positions for each X row)")]
    [SerializeField] private int[] correctPath = new int[20]
    {
        2, 2, 1, 1, 2, 3, 3, 2, 1, 0,
        0, 1, 2, 3, 4, 4, 3, 2, 2, 2
    }; // Example: winding path

    [Header("Materials")]
    [SerializeField] private Material destructibleMaterial;
    [SerializeField] private Material indestructibleMaterial;

    [Header("Colors (if no materials)")]
    [SerializeField] private Color destructibleColor = new Color(0.6f, 0.4f, 0.2f); // Brown
    [SerializeField] private Color indestructibleColor = new Color(0.3f, 0.3f, 0.3f); // Dark gray

    [Header("Item Placements")]
    [SerializeField] private Vector2Int backpackPosition = new Vector2Int(0, 5); // (X, Z)
    [SerializeField] private Vector2Int pickaxePosition = new Vector2Int(2, 10);
    [SerializeField] private Vector2Int flashlightPosition = new Vector2Int(4, 15); // Hidden in a block

    [Header("Naming")]
    [SerializeField] private string mazeName = "Stage5_Maze";

    private GameObject mazeParent;

    /// <summary>
    /// Builds the maze
    /// </summary>
    [ContextMenu("Build Maze")]
    public void BuildMaze()
    {
        Debug.Log($"🏗️ Building maze {width}x{length}...");

        // Create parent
        mazeParent = new GameObject(mazeName);
        mazeParent.transform.position = transform.position;

        // Verify that correctPath has the right length
        if (correctPath == null || correctPath.Length != length)
        {
            Debug.LogError($"❌ correctPath must have {length} elements (maze length)!");
            return;
        }

        // Generate the maze
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * blockSize, blockHeight / 2, z * blockSize);
                bool isOnPath = (x == correctPath[z]);

                CreateWallBlock(position, isOnPath, x, z);
            }
        }

        // Place items
        PlaceItems();

        Debug.Log("✅ Maze built successfully!");
    }

    /// <summary>
    /// Creates a wall block (destructible or not)
    /// </summary>
    private void CreateWallBlock(Vector3 position, bool isDestructible, int x, int z)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = $"Wall_{x}_{z}" + (isDestructible ? "_Destructible" : "_Solid");
        block.transform.SetParent(mazeParent.transform);
        block.transform.position = transform.position + position;
        block.transform.localScale = new Vector3(blockSize * 0.9f, blockHeight, blockSize * 0.9f);

        // Material/Color
        Renderer renderer = block.GetComponent<Renderer>();
        if (isDestructible)
        {
            if (destructibleMaterial != null)
                renderer.material = destructibleMaterial;
            else
                renderer.material.color = destructibleColor;

            // Add DestructibleWall script
            DestructibleWall destructibleWall = block.AddComponent<DestructibleWall>();
            // Note: maxHealth is set via SerializeField, default is 3 hits
        }
        else
        {
            if (indestructibleMaterial != null)
                renderer.material = indestructibleMaterial;
            else
                renderer.material.color = indestructibleColor;
        }

        // Tag for identification
        if (isDestructible)
        {
            block.tag = "Destructible";
        }
    }

    /// <summary>
    /// Places items in the maze
    /// </summary>
    private void PlaceItems()
    {
        // Place backpack
        PlaceItem("Pickup_Backpack", backpackPosition, "Data_Backpack");

        // Place pickaxe
        PlaceItem("Pickup_Pickaxe", pickaxePosition, "Data_Pickaxe");

        // Place flashlight (hidden in a destructible wall)
        PlaceItem("Pickup_Flashlight", flashlightPosition, "Data_Flashlight");

        Debug.Log("✅ Items placed in the maze");
    }

    /// <summary>
    /// Places an item at a given position
    /// </summary>
    private void PlaceItem(string itemName, Vector2Int gridPos, string dataAssetName)
    {
        Vector3 worldPos = transform.position + new Vector3(gridPos.x * blockSize, blockHeight / 2, gridPos.y * blockSize);

        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        item.name = itemName;
        item.transform.SetParent(mazeParent.transform);
        item.transform.position = worldPos;
        item.transform.localScale = Vector3.one * 0.5f;

        // Trigger collider
        SphereCollider collider = item.GetComponent<SphereCollider>();
        collider.isTrigger = true;

        // ItemPickup script
        ItemPickup pickup = item.AddComponent<ItemPickup>();
        // Note: You'll need to manually assign the ItemData asset in the Inspector

        // Layer
        item.layer = LayerMask.NameToLayer("Interactable");

        // Temporary color (replace with 3D model later)
        Renderer renderer = item.GetComponent<Renderer>();
        if (itemName.Contains("Backpack"))
            renderer.material.color = Color.yellow;
        else if (itemName.Contains("Pickaxe"))
            renderer.material.color = Color.gray;
        else if (itemName.Contains("Flashlight"))
            renderer.material.color = Color.white;

        Debug.Log($"  ✅ {itemName} placed at ({gridPos.x}, {gridPos.y})");
    }

    /// <summary>
    /// Cleans up the existing maze
    /// </summary>
    [ContextMenu("Clear Maze")]
    public void ClearMaze()
    {
        GameObject existing = GameObject.Find(mazeName);
        if (existing != null)
        {
            DestroyImmediate(existing);
            Debug.Log($"🗑️ Maze '{mazeName}' deleted");
        }
        else
        {
            Debug.Log("⚠️ No maze to clean up");
        }
    }

    /// <summary>
    /// Displays the correct path in the editor
    /// </summary>
    [ContextMenu("Show Path in Console")]
    public void ShowPath()
    {
        Debug.Log("=== CORRECT PATH ===");
        for (int z = 0; z < correctPath.Length; z++)
        {
            Debug.Log($"Row {z}: Position X = {correctPath[z]}");
        }
    }

    /// <summary>
    /// Generates a random path (for testing)
    /// </summary>
    [ContextMenu("Generate Random Path")]
    public void GenerateRandomPath()
    {
        correctPath = new int[length];
        int currentX = width / 2; // Start in the middle

        for (int z = 0; z < length; z++)
        {
            correctPath[z] = currentX;

            // Random movement (left, right, or straight)
            int move = Random.Range(-1, 2); // -1, 0, 1
            currentX = Mathf.Clamp(currentX + move, 0, width - 1);
        }

        Debug.Log("✅ Random path generated!");
    }

    /// <summary>
    /// Gizmo to visualize the maze
    /// </summary>
    private void OnDrawGizmos()
    {
        if (correctPath == null || correctPath.Length == 0) return;

        Gizmos.color = Color.green;

        for (int z = 0; z < Mathf.Min(correctPath.Length, length); z++)
        {
            int x = correctPath[z];
            Vector3 pos = transform.position + new Vector3(x * blockSize, blockHeight / 2, z * blockSize);
            Gizmos.DrawWireCube(pos, new Vector3(blockSize * 0.9f, blockHeight, blockSize * 0.9f));
        }
    }
}
