using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Automatically builds the Stage 4 riddle room with steles, platforms, and exit.
/// </summary>
public class RiddleRoomBuilder : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private string roomParentName = "Stage_4_RiddleRoom";
    [SerializeField] private Vector3 roomSize = new Vector3(20f, 10f, 20f);
    [SerializeField] private Vector3 roomCenter = Vector3.zero;

    [Header("Riddle Settings")]
    [SerializeField] private int numberOfRiddles = 3;
    [SerializeField] private int riddlesRequired = 3; // How many must be solved
    [SerializeField] private float steleHeight = 2f;
    [SerializeField] private Vector3 steleSize = new Vector3(1f, 2f, 0.5f);

    [Header("Exit Settings")]
    [SerializeField] private Vector3 exitPlatformPosition = new Vector3(0f, 0f, 15f);
    [SerializeField] private Vector3 exitPlatformSize = new Vector3(5f, 0.5f, 5f);
    [SerializeField] private Vector3 exitDoorPosition = new Vector3(0f, 2f, 18f);

    [Header("Spawn Point")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(0f, 1f, -8f);

    [Header("Materials")]
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material steleMaterial;
    [SerializeField] private Material exitPlatformMaterial;

    [Header("Prefabs")]
    [SerializeField] private GameObject doorPrefab;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private GameObject roomParent;

    // Predefined riddles for Stage 4
    private RiddleData[] predefinedRiddles = new RiddleData[]
    {
        new RiddleData
        {
            question = "I speak without a mouth and hear without ears. I have no body, but come alive with wind. What am I?",
            answer = "echo",
            alternatives = new string[] { "an echo" },
            hint = "Think about sound bouncing off walls..."
        },
        new RiddleData
        {
            question = "The more you take, the more you leave behind. What am I?",
            answer = "footsteps",
            alternatives = new string[] { "steps", "footprints" },
            hint = "Think about walking..."
        },
        new RiddleData
        {
            question = "I have cities, but no houses. I have mountains, but no trees. I have water, but no fish. What am I?",
            answer = "map",
            alternatives = new string[] { "a map" },
            hint = "You use me to navigate..."
        },
        new RiddleData
        {
            question = "What has keys but no locks, space but no room, and you can enter but can't go inside?",
            answer = "keyboard",
            alternatives = new string[] { "a keyboard" },
            hint = "You're probably using one right now..."
        },
        new RiddleData
        {
            question = "I am taken from a mine and shut up in a wooden case, from which I am never released. What am I?",
            answer = "pencil lead",
            alternatives = new string[] { "lead", "graphite", "pencil" },
            hint = "Think about writing instruments..."
        }
    };

    [System.Serializable]
    private class RiddleData
    {
        public string question;
        public string answer;
        public string[] alternatives;
        public string hint;
    }

    /// <summary>
    /// Builds the entire riddle room
    /// </summary>
    [ContextMenu("Build Riddle Room")]
    public void BuildRoom()
    {
        if (showDebugLogs)
        {
            Debug.Log("🏗️ Building Stage 4: Riddle Room...");
        }

        // Clean up existing room
        CleanupExisting();

        // Create parent object
        roomParent = new GameObject(roomParentName);
        roomParent.transform.position = roomCenter;

        // Build room components
        BuildFloor();
        BuildSteles();
        BuildExitPlatform();
        BuildExitDoor();
        BuildSpawnPoint();
        SetupRiddleManager();

        if (showDebugLogs)
        {
            Debug.Log("✅ Riddle Room built successfully!");
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
            floor.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.4f);
        }

        if (showDebugLogs)
        {
            Debug.Log($"  ✅ Floor created ({roomSize.x}x{roomSize.z})");
        }
    }

    /// <summary>
    /// Builds all steles with riddles
    /// </summary>
    private void BuildSteles()
    {
        GameObject stelesParent = new GameObject("Steles");
        stelesParent.transform.SetParent(roomParent.transform);
        stelesParent.transform.localPosition = Vector3.zero;

        // Arrange steles in a semi-circle
        float radius = 8f;
        float angleStep = 120f / (numberOfRiddles - 1); // Spread across 120 degrees
        float startAngle = -60f; // Start from left

        for (int i = 0; i < numberOfRiddles; i++)
        {
            float angle = startAngle + (angleStep * i);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 position = new Vector3(
                Mathf.Sin(rad) * radius,
                steleHeight / 2f,
                Mathf.Cos(rad) * radius - 5f
            );

            BuildStele(i + 1, position, stelesParent.transform);
        }

        if (showDebugLogs)
        {
            Debug.Log($"  ✅ {numberOfRiddles} steles created");
        }
    }

    /// <summary>
    /// Builds a single stele
    /// </summary>
    private void BuildStele(int riddleNumber, Vector3 position, Transform parent)
    {
        GameObject stele = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stele.name = $"Stele_{riddleNumber}";
        stele.transform.SetParent(parent);
        stele.transform.localPosition = position;
        stele.transform.localScale = steleSize;

        // Add Stele component
        Stele steleScript = stele.AddComponent<Stele>();

        // Configure riddle (use reflection to set private fields)
        var riddleNumberField = typeof(Stele).GetField("riddleNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var riddleQuestionField = typeof(Stele).GetField("riddleQuestion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var correctAnswerField = typeof(Stele).GetField("correctAnswer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var alternativeAnswersField = typeof(Stele).GetField("alternativeAnswers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var hintField = typeof(Stele).GetField("hint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (riddleNumber - 1 < predefinedRiddles.Length)
        {
            RiddleData riddle = predefinedRiddles[riddleNumber - 1];
            riddleNumberField?.SetValue(steleScript, riddleNumber);
            riddleQuestionField?.SetValue(steleScript, riddle.question);
            correctAnswerField?.SetValue(steleScript, riddle.answer);
            alternativeAnswersField?.SetValue(steleScript, riddle.alternatives);
            hintField?.SetValue(steleScript, riddle.hint);
        }

        // Set material
        if (steleMaterial != null)
        {
            stele.GetComponent<Renderer>().material = steleMaterial;
        }
        else
        {
            stele.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    /// <summary>
    /// Builds the exit platform (appears when riddles solved)
    /// </summary>
    private void BuildExitPlatform()
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "ExitPlatform";
        platform.transform.SetParent(roomParent.transform);
        platform.transform.localPosition = exitPlatformPosition;
        platform.transform.localScale = exitPlatformSize;

        if (exitPlatformMaterial != null)
        {
            platform.GetComponent<Renderer>().material = exitPlatformMaterial;
        }
        else
        {
            platform.GetComponent<Renderer>().material.color = Color.cyan;
        }

        // Initially hidden
        platform.SetActive(false);

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Exit platform created");
        }
    }

    /// <summary>
    /// Builds the exit door to next stage
    /// </summary>
    private void BuildExitDoor()
    {
        GameObject door;

        if (doorPrefab != null)
        {
            door = Instantiate(doorPrefab, roomParent.transform);
            door.name = "ExitDoor_ToStage5";
        }
        else
        {
            // Create simple door
            door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "ExitDoor_ToStage5";
            door.transform.localScale = new Vector3(3f, 4f, 0.5f);
            door.GetComponent<Renderer>().material.color = Color.green;
        }

        door.transform.SetParent(roomParent.transform);
        door.transform.localPosition = exitDoorPosition;

        // Add DoorTrigger component
        DoorTrigger doorTrigger = door.GetComponent<DoorTrigger>();
        if (doorTrigger == null)
        {
            doorTrigger = door.AddComponent<DoorTrigger>();
        }

        // Configure door (use reflection)
        var doorTypeField = typeof(DoorTrigger).GetField("doorType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var targetSceneField = typeof(DoorTrigger).GetField("targetSceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        doorTypeField?.SetValue(doorTrigger, DoorTrigger.DoorType.SceneTransition);
        targetSceneField?.SetValue(doorTrigger, "Stage_5_Maze");

        // Add trigger collider
        BoxCollider trigger = door.GetComponent<BoxCollider>();
        if (trigger == null)
        {
            trigger = door.AddComponent<BoxCollider>();
        }
        trigger.isTrigger = true;

        // Initially hidden
        door.SetActive(false);

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Exit door created");
        }
    }

    /// <summary>
    /// Builds the spawn point for player entry
    /// </summary>
    private void BuildSpawnPoint()
    {
        GameObject spawn = new GameObject("SpawnPoint_Stage4");
        spawn.transform.SetParent(roomParent.transform);
        spawn.transform.localPosition = spawnPosition;

        SpawnPoint spawnScript = spawn.AddComponent<SpawnPoint>();

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Spawn point created");
        }
    }

    /// <summary>
    /// Sets up the RiddleManager component
    /// </summary>
    private void SetupRiddleManager()
    {
        RiddleManager manager = roomParent.AddComponent<RiddleManager>();

        // Find all steles
        Stele[] steles = roomParent.GetComponentsInChildren<Stele>();

        // Find exit objects
        GameObject exitPlatform = GameObject.Find($"{roomParentName}/ExitPlatform");
        GameObject exitDoor = GameObject.Find($"{roomParentName}/ExitDoor_ToStage5");

        // Configure manager (use reflection)
        var riddlesField = typeof(RiddleManager).GetField("riddles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var riddlesRequiredField = typeof(RiddleManager).GetField("riddlesRequired", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var exitPlatformField = typeof(RiddleManager).GetField("exitPlatform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var exitDoorField = typeof(RiddleManager).GetField("exitDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        riddlesField?.SetValue(manager, steles);
        riddlesRequiredField?.SetValue(manager, riddlesRequired);
        exitPlatformField?.SetValue(manager, exitPlatform);
        exitDoorField?.SetValue(manager, exitDoor);

        if (showDebugLogs)
        {
            Debug.Log($"  ✅ RiddleManager configured ({steles.Length} riddles, {riddlesRequired} required)");
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(roomCenter, roomSize);

        // Draw spawn point
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(roomCenter + spawnPosition, 0.5f);

        // Draw exit platform
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(roomCenter + exitPlatformPosition, exitPlatformSize);
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = Color.cyan;
        Handles.Label(roomCenter + Vector3.up * 8f, $"STAGE 4: RIDDLE ROOM\n{numberOfRiddles} Riddles ({riddlesRequired} required)");
#endif
    }
}
