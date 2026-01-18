using UnityEngine;

/// <summary>
/// Builds the 5-room door maze for Stage 2.
/// Automatically creates rooms with correct door connections.
/// Room network: Room 1 (2 doors) → Room 2 (3 doors) → Room 3 (4 doors) → Room 4 (5 doors) → Room 5 (5 doors, has Pistol)
/// </summary>
[ExecuteInEditMode]
public class MazeRoomBuilder : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private Vector3 roomSize = new Vector3(10, 6, 10);
    [SerializeField] private float roomSpacing = 20f; // Distance between rooms (won't be visible to player)
    [SerializeField] private Color[] roomColors = new Color[5]
    {
        new Color(0.8f, 0.3f, 0.3f), // Red tint
        new Color(0.3f, 0.8f, 0.3f), // Green tint
        new Color(0.3f, 0.3f, 0.8f), // Blue tint
        new Color(0.8f, 0.8f, 0.3f), // Yellow tint
        new Color(0.8f, 0.3f, 0.8f)  // Purple tint
    };

    [Header("Door Settings")]
    [SerializeField] private Vector3 doorSize = new Vector3(2, 3, 0.5f);
    [SerializeField] private float doorSpacing = 3f; // Space between doors on wall

    [Header("Items")]
    [SerializeField] private bool placePistolInRoom5 = true;
    [SerializeField] private Vector3 pistolOffset = new Vector3(0, 1, 0);

    [Header("Naming")]
    [SerializeField] private string mazeParentName = "Stage2_DoorMaze";

    private GameObject mazeParent;
    private GameObject[] rooms = new GameObject[5];
    private Transform[] spawnPoints = new Transform[5];

    /// <summary>
    /// Builds the complete 5-room maze
    /// </summary>
    [ContextMenu("Build Maze")]
    public void BuildMaze()
    {
        Debug.Log("🏗️ Building 5-room door maze...");

        // Create parent
        mazeParent = new GameObject(mazeParentName);
        mazeParent.transform.position = transform.position;

        // Build all 5 rooms
        for (int i = 0; i < 5; i++)
        {
            BuildRoom(i + 1);
        }

        // Connect doors between rooms
        ConnectDoors();

        // Place pistol in Room 5
        if (placePistolInRoom5)
        {
            PlacePistol();
        }

        Debug.Log("✅ 5-room maze built successfully!");
    }

    /// <summary>
    /// Builds a single room
    /// </summary>
    private void BuildRoom(int roomNumber)
    {
        // Position rooms in a line (won't matter since they're teleport-connected)
        Vector3 position = transform.position + new Vector3((roomNumber - 1) * roomSpacing, 0, 0);

        GameObject room = new GameObject($"Room_{roomNumber}");
        room.transform.SetParent(mazeParent.transform);
        room.transform.position = position;

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(room.transform);
        floor.transform.localPosition = new Vector3(0, -0.25f, 0);
        floor.transform.localScale = new Vector3(roomSize.x, 0.5f, roomSize.z);
        floor.GetComponent<Renderer>().material.color = roomColors[roomNumber - 1];

        // Walls (4 walls)
        CreateWall(room.transform, "Wall_North", new Vector3(0, roomSize.y / 2, roomSize.z / 2), new Vector3(roomSize.x, roomSize.y, 0.5f));
        CreateWall(room.transform, "Wall_South", new Vector3(0, roomSize.y / 2, -roomSize.z / 2), new Vector3(roomSize.x, roomSize.y, 0.5f));
        CreateWall(room.transform, "Wall_East", new Vector3(roomSize.x / 2, roomSize.y / 2, 0), new Vector3(0.5f, roomSize.y, roomSize.z));
        CreateWall(room.transform, "Wall_West", new Vector3(-roomSize.x / 2, roomSize.y / 2, 0), new Vector3(0.5f, roomSize.y, roomSize.z));

        // Ceiling
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.SetParent(room.transform);
        ceiling.transform.localPosition = new Vector3(0, roomSize.y + 0.25f, 0);
        ceiling.transform.localScale = new Vector3(roomSize.x, 0.5f, roomSize.z);

        // Spawn point (center of room)
        GameObject spawnObj = new GameObject("SpawnPoint");
        spawnObj.transform.SetParent(room.transform);
        spawnObj.transform.localPosition = new Vector3(0, 1, 0);
        spawnPoints[roomNumber - 1] = spawnObj.transform;

        // Add SpawnPoint component
        SpawnPoint spawn = spawnObj.AddComponent<SpawnPoint>();
        // Configure it via reflection
        var idField = spawn.GetType().GetField("spawnID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (idField != null)
        {
            idField.SetValue(spawn, $"Room{roomNumber}");
        }

        // Add RoomIdentifier component
        RoomIdentifier identifier = room.AddComponent<RoomIdentifier>();
        // Configure via reflection
        var roomNumField = identifier.GetType().GetField("roomNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (roomNumField != null)
        {
            roomNumField.SetValue(identifier, roomNumber);
        }
        var roomNameField = identifier.GetType().GetField("roomName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (roomNameField != null)
        {
            roomNameField.SetValue(identifier, $"Room {roomNumber}");
        }

        rooms[roomNumber - 1] = room;

        Debug.Log($"  ✅ Room {roomNumber} created");
    }

    /// <summary>
    /// Creates a wall
    /// </summary>
    private void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent);
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f);
    }

    /// <summary>
    /// Connects doors between rooms according to the maze network
    /// </summary>
    private void ConnectDoors()
    {
        // Room 1: 2 doors → Room 2, Room 1 (loop)
        CreateDoor(rooms[0], 1, spawnPoints[1], 0); // To Room 2
        CreateDoor(rooms[0], 1, spawnPoints[0], 1); // To Room 1 (loop)

        // Room 2: 3 doors → Room 3, Room 2, Room 1
        CreateDoor(rooms[1], 2, spawnPoints[2], 0); // To Room 3
        CreateDoor(rooms[1], 2, spawnPoints[1], 1); // To Room 2 (loop)
        CreateDoor(rooms[1], 1, spawnPoints[0], 2); // To Room 1

        // Room 3: 4 doors → Room 4, Room 3, Room 2, Room 1
        CreateDoor(rooms[2], 3, spawnPoints[3], 0); // To Room 4
        CreateDoor(rooms[2], 3, spawnPoints[2], 1); // To Room 3 (loop)
        CreateDoor(rooms[2], 2, spawnPoints[1], 2); // To Room 2
        CreateDoor(rooms[2], 1, spawnPoints[0], 3); // To Room 1

        // Room 4: 5 doors → Stage 3 Exit, Room 4, Room 5, Room 3, Room 2
        // (Note: First door goes to Stage 3, we'll handle that manually)
        CreateDoor(rooms[3], 4, spawnPoints[3], 1); // To Room 4 (loop)
        CreateDoor(rooms[3], 5, spawnPoints[4], 2); // To Room 5
        CreateDoor(rooms[3], 3, spawnPoints[2], 3); // To Room 3
        CreateDoor(rooms[3], 2, spawnPoints[1], 4); // To Room 2

        // Room 5: 5 doors → Room 1-5
        CreateDoor(rooms[4], 1, spawnPoints[0], 0); // To Room 1
        CreateDoor(rooms[4], 2, spawnPoints[1], 1); // To Room 2
        CreateDoor(rooms[4], 3, spawnPoints[2], 2); // To Room 3
        CreateDoor(rooms[4], 4, spawnPoints[3], 3); // To Room 4
        CreateDoor(rooms[4], 5, spawnPoints[4], 4); // To Room 5 (loop)

        Debug.Log("  ✅ Doors connected");
    }

    /// <summary>
    /// Creates a door in a room
    /// </summary>
    private void CreateDoor(GameObject room, int targetRoom, Transform targetSpawn, int doorIndex)
    {
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = $"Door_To_Room{targetRoom}";
        door.transform.SetParent(room.transform);

        // Position door on north wall
        float offsetX = (doorIndex - 2) * doorSpacing; // Center multiple doors
        door.transform.localPosition = new Vector3(offsetX, 1.5f, roomSize.z / 2 - 0.5f);
        door.transform.localScale = doorSize;

        // Make it a trigger
        BoxCollider collider = door.GetComponent<BoxCollider>();
        collider.isTrigger = true;

        // Add DoorTrigger component
        DoorTrigger doorTrigger = door.AddComponent<DoorTrigger>();
        // Configure via reflection
        var doorTypeField = doorTrigger.GetType().GetField("doorType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (doorTypeField != null)
        {
            doorTypeField.SetValue(doorTrigger, DoorTrigger.DoorType.Teleport);
        }
        var destField = doorTrigger.GetType().GetField("teleportDestination", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (destField != null)
        {
            destField.SetValue(doorTrigger, targetSpawn);
        }

        // Color door
        door.GetComponent<Renderer>().material.color = Color.blue;
    }

    /// <summary>
    /// Places pistol in Room 5
    /// </summary>
    private void PlacePistol()
    {
        if (rooms[4] == null) return;

        GameObject pistol = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pistol.name = "Pickup_Pistol";
        pistol.transform.SetParent(rooms[4].transform);
        pistol.transform.localPosition = pistolOffset;
        pistol.transform.localScale = Vector3.one * 0.5f;

        // Make it a trigger
        SphereCollider collider = pistol.GetComponent<SphereCollider>();
        collider.isTrigger = true;

        // Add ItemPickup component
        ItemPickup pickup = pistol.AddComponent<ItemPickup>();
        // Note: Manually assign Data_Pistol in Inspector

        // Set layer
        pistol.layer = LayerMask.NameToLayer("Interactable");

        // Color
        pistol.GetComponent<Renderer>().material.color = Color.red;

        Debug.Log("  ✅ Pistol placed in Room 5");
    }

    /// <summary>
    /// Clears the maze
    /// </summary>
    [ContextMenu("Clear Maze")]
    public void ClearMaze()
    {
        GameObject existing = GameObject.Find(mazeParentName);
        if (existing != null)
        {
            DestroyImmediate(existing);
            Debug.Log($"🗑️ Maze '{mazeParentName}' deleted");
        }
        else
        {
            Debug.Log("⚠️ No maze to clean up");
        }
    }

    /// <summary>
    /// Gizmo to visualize the maze structure
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (int i = 0; i < 5; i++)
        {
            Vector3 position = transform.position + new Vector3(i * roomSpacing, 0, 0);
            Gizmos.DrawWireCube(position + new Vector3(0, roomSize.y / 2, 0), roomSize);
        }
    }
}
