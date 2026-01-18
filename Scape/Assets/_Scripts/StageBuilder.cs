using UnityEngine;

/// <summary>
/// Helper to quickly build stages in the editor.
/// Creates basic rooms with floor, walls, ceiling.
/// Usage: Add this script to an empty GameObject, configure in the Inspector, then click "Build Room".
/// </summary>
[ExecuteInEditMode]
public class StageBuilder : MonoBehaviour
{
    [Header("Room Dimensions")]
    [SerializeField] private Vector3 roomSize = new Vector3(20, 6, 20);
    [SerializeField] private float wallThickness = 0.5f;

    [Header("Materials")]
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material ceilingMaterial;

    [Header("Colors (if no materials)")]
    [SerializeField] private Color floorColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private Color wallColor = new Color(0.5f, 0.4f, 0.3f);
    [SerializeField] private Color ceilingColor = new Color(0.2f, 0.2f, 0.2f);

    [Header("Options")]
    [SerializeField] private bool createFloor = true;
    [SerializeField] private bool createCeiling = true;
    [SerializeField] private bool createWalls = true;
    [SerializeField] private bool addColliders = true;

    [Header("Naming")]
    [SerializeField] private string roomName = "Stage0_TutorialRoom";

    /// <summary>
    /// Builds the room (called from a custom button in the Inspector or manually)
    /// </summary>
    [ContextMenu("Build Room")]
    public void BuildRoom()
    {
        Debug.Log($"🏗️ Building room: {roomName}");

        // Create a parent for organization
        GameObject roomParent = new GameObject(roomName);
        roomParent.transform.position = transform.position;

        // Floor
        if (createFloor)
        {
            CreateFloor(roomParent.transform);
        }

        // Ceiling
        if (createCeiling)
        {
            CreateCeiling(roomParent.transform);
        }

        // Walls (4 walls)
        if (createWalls)
        {
            CreateWalls(roomParent.transform);
        }

        Debug.Log("✅ Room built successfully!");
    }

    /// <summary>
    /// Creates the floor of the room
    /// </summary>
    private void CreateFloor(Transform parent)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(parent);
        floor.transform.localPosition = new Vector3(0, -wallThickness / 2, 0);
        floor.transform.localScale = new Vector3(roomSize.x, wallThickness, roomSize.z);

        // Material or color
        Renderer renderer = floor.GetComponent<Renderer>();
        if (floorMaterial != null)
        {
            renderer.material = floorMaterial;
        }
        else
        {
            renderer.material.color = floorColor;
        }

        // Collider
        if (!addColliders)
        {
            Destroy(floor.GetComponent<Collider>());
        }

        Debug.Log("  ✅ Floor created");
    }

    /// <summary>
    /// Creates the ceiling of the room
    /// </summary>
    private void CreateCeiling(Transform parent)
    {
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.SetParent(parent);
        ceiling.transform.localPosition = new Vector3(0, roomSize.y + wallThickness / 2, 0);
        ceiling.transform.localScale = new Vector3(roomSize.x, wallThickness, roomSize.z);

        // Material or color
        Renderer renderer = ceiling.GetComponent<Renderer>();
        if (ceilingMaterial != null)
        {
            renderer.material = ceilingMaterial;
        }
        else
        {
            renderer.material.color = ceilingColor;
        }

        // Collider
        if (!addColliders)
        {
            Destroy(ceiling.GetComponent<Collider>());
        }

        Debug.Log("  ✅ Ceiling created");
    }

    /// <summary>
    /// Creates the 4 walls of the room
    /// </summary>
    private void CreateWalls(Transform parent)
    {
        // North Wall (+Z)
        CreateWall(parent, "Wall_North", new Vector3(0, roomSize.y / 2, roomSize.z / 2), new Vector3(roomSize.x, roomSize.y, wallThickness));

        // South Wall (-Z)
        CreateWall(parent, "Wall_South", new Vector3(0, roomSize.y / 2, -roomSize.z / 2), new Vector3(roomSize.x, roomSize.y, wallThickness));

        // East Wall (+X)
        CreateWall(parent, "Wall_East", new Vector3(roomSize.x / 2, roomSize.y / 2, 0), new Vector3(wallThickness, roomSize.y, roomSize.z));

        // West Wall (-X)
        CreateWall(parent, "Wall_West", new Vector3(-roomSize.x / 2, roomSize.y / 2, 0), new Vector3(wallThickness, roomSize.y, roomSize.z));

        Debug.Log("  ✅ 4 walls created");
    }

    /// <summary>
    /// Creates an individual wall
    /// </summary>
    private void CreateWall(Transform parent, string wallName, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = wallName;
        wall.transform.SetParent(parent);
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;

        // Material or color
        Renderer renderer = wall.GetComponent<Renderer>();
        if (wallMaterial != null)
        {
            renderer.material = wallMaterial;
        }
        else
        {
            renderer.material.color = wallColor;
        }

        // Collider
        if (!addColliders)
        {
            Destroy(wall.GetComponent<Collider>());
        }
    }

    /// <summary>
    /// Cleans up the existing room (removes children)
    /// </summary>
    [ContextMenu("Clear Room")]
    public void ClearRoom()
    {
        GameObject existingRoom = GameObject.Find(roomName);
        if (existingRoom != null)
        {
            DestroyImmediate(existingRoom);
            Debug.Log($"🗑️ Room '{roomName}' deleted");
        }
        else
        {
            Debug.Log("⚠️ No room to clean up");
        }
    }

    /// <summary>
    /// Visualization of the room in the editor
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, roomSize.y / 2, 0), roomSize);
    }
}
