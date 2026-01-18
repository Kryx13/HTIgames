using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Automatically builds Stage 7: Final Room.
/// Creates a room with the Amulet door slot and victory trigger.
/// </summary>
public class FinalRoomBuilder : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private string roomParentName = "Stage_7_FinalRoom";
    [SerializeField] private Vector3 roomSize = new Vector3(15f, 8f, 15f);
    [SerializeField] private Vector3 roomCenter = Vector3.zero;

    [Header("Door Settings")]
    [SerializeField] private Vector3 doorPosition = new Vector3(0f, 2f, 6f);
    [SerializeField] private Vector3 doorSize = new Vector3(4f, 5f, 0.5f);
    [SerializeField] private float doorOpenHeight = 6f;

    [Header("Amulet Slot Settings")]
    [SerializeField] private Vector3 slotPosition = new Vector3(0f, 1.5f, 4f);
    [SerializeField] private Vector3 slotSize = new Vector3(0.8f, 0.8f, 0.3f);
    [SerializeField] private bool createAmuletVisual = true;

    [Header("Spawn Point")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(0f, 1f, -5f);

    [Header("Victory Trigger")]
    [SerializeField] private Vector3 victoryZonePosition = new Vector3(0f, 1f, 10f);
    [SerializeField] private Vector3 victoryZoneSize = new Vector3(5f, 3f, 3f);

    [Header("Decoration")]
    [SerializeField] private bool addPedestal = true;
    [SerializeField] private bool addTorches = true;
    [SerializeField] private int numberOfTorches = 4;

    [Header("Materials")]
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material doorMaterial;
    [SerializeField] private Material slotMaterial;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private GameObject roomParent;

    /// <summary>
    /// Builds the entire final room
    /// </summary>
    [ContextMenu("Build Final Room")]
    public void BuildRoom()
    {
        if (showDebugLogs)
        {
            Debug.Log("🏗️ Building Stage 7: Final Room...");
        }

        // Clean up existing
        CleanupExisting();

        // Create parent
        roomParent = new GameObject(roomParentName);
        roomParent.transform.position = roomCenter;

        // Build components
        BuildFloor();
        BuildDoor();
        BuildAmuletSlot();
        BuildSpawnPoint();
        BuildVictoryZone();

        if (addPedestal)
        {
            BuildPedestal();
        }

        if (addTorches)
        {
            BuildTorches();
        }

        if (showDebugLogs)
        {
            Debug.Log("✅ Final Room built successfully!");
        }
    }

    /// <summary>
    /// Builds the room floor
    /// </summary>
    private void BuildFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(roomParent.transform);
        floor.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        floor.transform.localScale = new Vector3(roomSize.x, 1f, roomSize.z);

        if (floorMaterial != null)
        {
            floor.GetComponent<Renderer>().material = floorMaterial;
        }
        else
        {
            floor.GetComponent<Renderer>().material.color = new Color(0.4f, 0.35f, 0.3f); // Stone color
        }

        if (showDebugLogs)
        {
            Debug.Log($"  ✅ Floor created ({roomSize.x}x{roomSize.z})");
        }
    }

    /// <summary>
    /// Builds the final door (that opens when Amulet placed)
    /// </summary>
    private void BuildDoor()
    {
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name = "FinalDoor";
        door.transform.SetParent(roomParent.transform);
        door.transform.localPosition = doorPosition;
        door.transform.localScale = doorSize;

        if (doorMaterial != null)
        {
            door.GetComponent<Renderer>().material = doorMaterial;
        }
        else
        {
            door.GetComponent<Renderer>().material.color = new Color(0.6f, 0.5f, 0.3f); // Gold-ish
        }

        // Add decorative patterns (optional)
        AddDoorDetails(door);

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Final door created");
        }
    }

    /// <summary>
    /// Adds decorative details to the door
    /// </summary>
    private void AddDoorDetails(GameObject door)
    {
        // Create a circular symbol on the door
        GameObject symbol = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        symbol.name = "DoorSymbol";
        symbol.transform.SetParent(door.transform);
        symbol.transform.localPosition = new Vector3(0f, 0f, -0.3f);
        symbol.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        symbol.transform.localScale = new Vector3(1f, 0.05f, 1f);
        symbol.GetComponent<Renderer>().material.color = Color.gold;
    }

    /// <summary>
    /// Builds the Amulet slot
    /// </summary>
    private void BuildAmuletSlot()
    {
        GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slot.name = "AmuletSlot";
        slot.transform.SetParent(roomParent.transform);
        slot.transform.localPosition = slotPosition;
        slot.transform.localScale = slotSize;

        if (slotMaterial != null)
        {
            slot.GetComponent<Renderer>().material = slotMaterial;
        }
        else
        {
            slot.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.4f);
        }

        // Add AmuletDoorSlot component
        AmuletDoorSlot slotScript = slot.AddComponent<AmuletDoorSlot>();

        // Find the door
        GameObject door = GameObject.Find($"{roomParentName}/FinalDoor");

        // Create amulet visual if requested
        GameObject amuletVisual = null;
        if (createAmuletVisual)
        {
            amuletVisual = CreateAmuletVisual(slot.transform);
        }

        // Configure slot (use reflection)
        var doorField = typeof(AmuletDoorSlot).GetField("door", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var doorOpenHeightField = typeof(AmuletDoorSlot).GetField("doorOpenHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var amuletVisualField = typeof(AmuletDoorSlot).GetField("amuletVisual", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        doorField?.SetValue(slotScript, door);
        doorOpenHeightField?.SetValue(slotScript, doorOpenHeight);
        amuletVisualField?.SetValue(slotScript, amuletVisual);

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Amulet slot created");
        }
    }

    /// <summary>
    /// Creates the visual Amulet that appears when placed
    /// </summary>
    private GameObject CreateAmuletVisual(Transform slotTransform)
    {
        GameObject amulet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        amulet.name = "AmuletVisual";
        amulet.transform.SetParent(slotTransform);
        amulet.transform.localPosition = new Vector3(0f, 0f, -0.2f);
        amulet.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);

        // Gold color
        amulet.GetComponent<Renderer>().material.color = Color.yellow;

        // Make it glow
        Material mat = amulet.GetComponent<Renderer>().material;
        mat.SetColor("_EmissionColor", Color.yellow * 0.5f);
        mat.EnableKeyword("_EMISSION");

        // Initially hidden
        amulet.SetActive(false);

        return amulet;
    }

    /// <summary>
    /// Builds the pedestal beneath the slot
    /// </summary>
    private void BuildPedestal()
    {
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pedestal.name = "Pedestal";
        pedestal.transform.SetParent(roomParent.transform);
        pedestal.transform.localPosition = slotPosition - new Vector3(0f, 0.8f, 0f);
        pedestal.transform.localScale = new Vector3(1.5f, 0.8f, 1.5f);

        pedestal.GetComponent<Renderer>().material.color = new Color(0.5f, 0.45f, 0.4f);

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Pedestal created");
        }
    }

    /// <summary>
    /// Builds decorative torches around the room
    /// </summary>
    private void BuildTorches()
    {
        float radius = roomSize.x * 0.4f;

        for (int i = 0; i < numberOfTorches; i++)
        {
            float angle = (360f / numberOfTorches) * i * Mathf.Deg2Rad;
            Vector3 position = new Vector3(
                Mathf.Sin(angle) * radius,
                2f,
                Mathf.Cos(angle) * radius
            );

            BuildTorch(i, position);
        }

        if (showDebugLogs)
        {
            Debug.Log($"  ✅ {numberOfTorches} torches created");
        }
    }

    /// <summary>
    /// Builds a single torch
    /// </summary>
    private void BuildTorch(int index, Vector3 position)
    {
        // Torch stand
        GameObject stand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stand.name = $"Torch_{index}";
        stand.transform.SetParent(roomParent.transform);
        stand.transform.localPosition = position;
        stand.transform.localScale = new Vector3(0.2f, 2f, 0.2f);
        stand.GetComponent<Renderer>().material.color = new Color(0.3f, 0.25f, 0.2f);

        // Light
        GameObject lightObj = new GameObject($"TorchLight_{index}");
        lightObj.transform.SetParent(stand.transform);
        lightObj.transform.localPosition = new Vector3(0f, 1.2f, 0f);

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 8f;
        light.intensity = 1.5f;
        light.color = new Color(1f, 0.7f, 0.3f); // Warm orange
        light.shadows = LightShadows.Soft;
    }

    /// <summary>
    /// Builds the spawn point
    /// </summary>
    private void BuildSpawnPoint()
    {
        GameObject spawn = new GameObject("SpawnPoint_Stage7");
        spawn.transform.SetParent(roomParent.transform);
        spawn.transform.localPosition = spawnPosition;

        SpawnPoint spawnScript = spawn.AddComponent<SpawnPoint>();

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Spawn point created");
        }
    }

    /// <summary>
    /// Builds the victory zone (player enters after door opens)
    /// </summary>
    private void BuildVictoryZone()
    {
        GameObject victoryZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        victoryZone.name = "VictoryZone";
        victoryZone.transform.SetParent(roomParent.transform);
        victoryZone.transform.localPosition = victoryZonePosition;
        victoryZone.transform.localScale = victoryZoneSize;

        // Make it a trigger
        BoxCollider collider = victoryZone.GetComponent<BoxCollider>();
        collider.isTrigger = true;

        // Make it invisible (or semi-transparent)
        Renderer renderer = victoryZone.GetComponent<Renderer>();
        Color transparentGold = Color.yellow;
        transparentGold.a = 0.1f;
        renderer.material.color = transparentGold;

        // Add VictoryTrigger component
        VictoryTrigger victoryScript = victoryZone.AddComponent<VictoryTrigger>();

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Victory zone created");
        }
    }

    /// <summary>
    /// Cleans up existing room
    /// </summary>
    private void CleanupExisting()
    {
        GameObject existing = GameObject.Find(roomParentName);
        if (existing != null)
        {
            DestroyImmediate(existing);
            if (showDebugLogs)
            {
                Debug.Log("  🗑️ Cleaned up existing room");
            }
        }
    }

    /// <summary>
    /// Gizmo to visualize room bounds
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(roomCenter, roomSize);

        // Draw spawn point
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(roomCenter + spawnPosition, 0.5f);

        // Draw door
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(roomCenter + doorPosition, doorSize);

        // Draw slot
        Gizmos.color = Color.gold;
        Gizmos.DrawWireCube(roomCenter + slotPosition, slotSize);
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = Color.gold;
        Handles.Label(roomCenter + Vector3.up * 8f, "STAGE 7: FINAL ROOM\n(Amulet Required)");
#endif
    }
}

/// <summary>
/// Trigger zone that activates when player enters (after door opens).
/// Optional secondary victory trigger.
/// </summary>
public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private bool showDebugLogs = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (showDebugLogs)
            {
                Debug.Log("🎉 Player entered victory zone! You escaped!");
            }

            // Optional: Additional victory effects
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null && !gameManager.IsGameEnded)
            {
                gameManager.LevelComplete();
            }
        }
    }
}
