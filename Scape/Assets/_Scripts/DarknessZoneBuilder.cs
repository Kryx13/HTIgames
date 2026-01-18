using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Automatically builds Stage 6: Darkness Zone.
/// Creates narrow winding path with kill zones and darkness effect.
/// </summary>
public class DarknessZoneBuilder : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private string zoneParentName = "Stage_6_DarknessZone";
    [SerializeField] private Vector3 zoneCenter = Vector3.zero;

    [Header("Path Settings")]
    [SerializeField] private int numberOfSegments = 15;
    [SerializeField] private float segmentLength = 8f;
    [SerializeField] private float pathWidth = 2f;
    [SerializeField] private float pathHeight = 0.5f;
    [SerializeField] private bool randomizePath = true;

    [Header("Kill Zone Settings")]
    [SerializeField] private float killZoneDepth = 20f; // How far below path
    [SerializeField] private Vector3 killZoneSize = new Vector3(100f, 1f, 100f);
    [SerializeField] private float killZoneYPosition = -10f;

    [Header("Spawn & Exit")]
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 exitPlatformSize = new Vector3(6f, 0.5f, 6f);

    [Header("Lighting")]
    [SerializeField] private bool addPathLights = true;
    [SerializeField] private int lightsPerSegment = 2;
    [SerializeField] private float lightRange = 8f;
    [SerializeField] private float lightIntensity = 0.8f;
    [SerializeField] private Color lightColor = new Color(0.5f, 0.7f, 1f); // Cool blue

    [Header("Materials")]
    [SerializeField] private Material pathMaterial;
    [SerializeField] private Material exitPlatformMaterial;

    [Header("Prefabs")]
    [SerializeField] private GameObject doorPrefab;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private GameObject zoneParent;
    private Vector3 currentPosition;
    private Quaternion currentRotation;

    /// <summary>
    /// Builds the entire darkness zone
    /// </summary>
    [ContextMenu("Build Darkness Zone")]
    public void BuildZone()
    {
        if (showDebugLogs)
        {
            Debug.Log("🏗️ Building Stage 6: Darkness Zone...");
        }

        // Clean up existing
        CleanupExisting();

        // Create parent
        zoneParent = new GameObject(zoneParentName);
        zoneParent.transform.position = zoneCenter;

        // Initialize path tracking
        currentPosition = zoneCenter;
        currentRotation = Quaternion.identity;

        // Build components
        BuildKillZone();
        BuildPath();
        BuildExitPlatform();
        BuildExitDoor();
        BuildSpawnPoint();
        SetupDarknessZone();

        if (showDebugLogs)
        {
            Debug.Log("✅ Darkness Zone built successfully!");
        }
    }

    /// <summary>
    /// Builds the kill zone beneath the path
    /// </summary>
    private void BuildKillZone()
    {
        GameObject killZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        killZone.name = "KillZone";
        killZone.transform.SetParent(zoneParent.transform);
        killZone.transform.localPosition = new Vector3(0f, killZoneYPosition, 0f);
        killZone.transform.localScale = killZoneSize;

        // Make it a trigger
        BoxCollider collider = killZone.GetComponent<BoxCollider>();
        collider.isTrigger = true;

        // Add KillZone component
        KillZone killZoneScript = killZone.AddComponent<KillZone>();

        // Make it invisible (or very dark)
        Renderer renderer = killZone.GetComponent<Renderer>();
        renderer.material.color = new Color(0.05f, 0.05f, 0.1f, 0.5f);

        if (showDebugLogs)
        {
            Debug.Log($"  ⚠️ Kill zone created at Y={killZoneYPosition}");
        }
    }

    /// <summary>
    /// Builds the winding narrow path
    /// </summary>
    private void BuildPath()
    {
        GameObject pathParent = new GameObject("NarrowPath");
        pathParent.transform.SetParent(zoneParent.transform);
        pathParent.transform.localPosition = Vector3.zero;

        for (int i = 0; i < numberOfSegments; i++)
        {
            // Determine segment type
            PathSegment.SegmentType segmentType = DetermineSegmentType(i);

            // Create segment
            GameObject segment = CreatePathSegment(i, segmentType, pathParent.transform);

            // Add lights
            if (addPathLights && i % lightsPerSegment == 0)
            {
                AddPathLight(segment.transform);
            }

            // Update position for next segment
            UpdatePathPosition(segment, segmentType);
        }

        if (showDebugLogs)
        {
            Debug.Log($"  ✅ Path created with {numberOfSegments} segments");
        }
    }

    /// <summary>
    /// Determines what type of segment to create
    /// </summary>
    private PathSegment.SegmentType DetermineSegmentType(int index)
    {
        if (!randomizePath)
        {
            // Straight path
            return PathSegment.SegmentType.Straight;
        }

        // First segment always straight
        if (index == 0)
        {
            return PathSegment.SegmentType.Straight;
        }

        // Random selection
        float random = Random.value;

        if (random < 0.6f)
        {
            return PathSegment.SegmentType.Straight;
        }
        else if (random < 0.9f)
        {
            return PathSegment.SegmentType.Curved;
        }
        else
        {
            return PathSegment.SegmentType.WithObstacle;
        }
    }

    /// <summary>
    /// Creates a single path segment
    /// </summary>
    private GameObject CreatePathSegment(int index, PathSegment.SegmentType type, Transform parent)
    {
        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = $"PathSegment_{index}_{type}";
        segment.transform.SetParent(parent);
        segment.transform.position = currentPosition;
        segment.transform.rotation = currentRotation;
        segment.transform.localScale = new Vector3(pathWidth, pathHeight, segmentLength);

        // Apply material
        if (pathMaterial != null)
        {
            segment.GetComponent<Renderer>().material = pathMaterial;
        }
        else
        {
            segment.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.35f);
        }

        return segment;
    }

    /// <summary>
    /// Updates the path position for the next segment
    /// </summary>
    private void UpdatePathPosition(GameObject segment, PathSegment.SegmentType type)
    {
        // Move forward by segment length
        currentPosition += currentRotation * Vector3.forward * segmentLength;

        // Randomly rotate for curved segments
        if (type == PathSegment.SegmentType.Curved && randomizePath)
        {
            float rotationAngle = Random.Range(-45f, 45f);
            currentRotation *= Quaternion.Euler(0f, rotationAngle, 0f);
        }
    }

    /// <summary>
    /// Adds a light to the path
    /// </summary>
    private void AddPathLight(Transform pathTransform)
    {
        GameObject lightObj = new GameObject("PathLight");
        lightObj.transform.SetParent(pathTransform);
        lightObj.transform.localPosition = new Vector3(0f, 2f, 0f); // Above path

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = lightRange;
        light.intensity = lightIntensity;
        light.color = lightColor;
        light.shadows = LightShadows.None; // Performance optimization in darkness

        // Flicker effect (optional)
        // You could add a script to make lights flicker for atmosphere
    }

    /// <summary>
    /// Builds the exit platform at the end
    /// </summary>
    private void BuildExitPlatform()
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "ExitPlatform";
        platform.transform.SetParent(zoneParent.transform);

        // Place at end of path
        platform.transform.position = currentPosition;
        platform.transform.localScale = exitPlatformSize;

        if (exitPlatformMaterial != null)
        {
            platform.GetComponent<Renderer>().material = exitPlatformMaterial;
        }
        else
        {
            platform.GetComponent<Renderer>().material.color = Color.cyan;
        }

        // Add trigger to activate exit door
        BoxCollider trigger = platform.GetComponent<BoxCollider>();
        trigger.isTrigger = true;

        // Add script to activate exit when player reaches
        ExitTrigger exitTrigger = platform.AddComponent<ExitTrigger>();

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Exit platform created");
        }
    }

    /// <summary>
    /// Builds the exit door
    /// </summary>
    private void BuildExitDoor()
    {
        GameObject door;

        if (doorPrefab != null)
        {
            door = Instantiate(doorPrefab, zoneParent.transform);
            door.name = "ExitDoor_ToStage7";
        }
        else
        {
            door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "ExitDoor_ToStage7";
            door.transform.localScale = new Vector3(3f, 4f, 0.5f);
            door.GetComponent<Renderer>().material.color = Color.green;
        }

        door.transform.SetParent(zoneParent.transform);
        door.transform.position = currentPosition + Vector3.forward * 5f;

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
        targetSceneField?.SetValue(doorTrigger, "Stage_7_Final");

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
    /// Builds the spawn point
    /// </summary>
    private void BuildSpawnPoint()
    {
        GameObject spawn = new GameObject("SpawnPoint_Stage6");
        spawn.transform.SetParent(zoneParent.transform);
        spawn.transform.localPosition = zoneCenter + spawnOffset;

        SpawnPoint spawnScript = spawn.AddComponent<SpawnPoint>();

        if (showDebugLogs)
        {
            Debug.Log("  ✅ Spawn point created");
        }
    }

    /// <summary>
    /// Sets up the DarknessZone component
    /// </summary>
    private void SetupDarknessZone()
    {
        DarknessZone darknessZone = zoneParent.AddComponent<DarknessZone>();

        // Find exit door
        GameObject exitDoor = GameObject.Find($"{zoneParentName}/ExitDoor_ToStage7");

        // Configure (use reflection)
        var exitDoorField = typeof(DarknessZone).GetField("exitDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        exitDoorField?.SetValue(darknessZone, exitDoor);

        if (showDebugLogs)
        {
            Debug.Log("  ✅ DarknessZone configured");
        }
    }

    /// <summary>
    /// Cleans up existing zone
    /// </summary>
    private void CleanupExisting()
    {
        GameObject existing = GameObject.Find(zoneParentName);
        if (existing != null)
        {
            DestroyImmediate(existing);
            if (showDebugLogs)
            {
                Debug.Log("  🗑️ Cleaned up existing zone");
            }
        }
    }

    /// <summary>
    /// Gizmo to visualize zone
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.2f, 0.5f, 0.3f);
        Gizmos.DrawCube(zoneCenter, new Vector3(killZoneSize.x, 20f, killZoneSize.z));

        // Draw spawn point
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(zoneCenter + spawnOffset, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = new Color(0.3f, 0.3f, 0.8f);
        Handles.Label(zoneCenter + Vector3.up * 10f,
            $"STAGE 6: DARKNESS ZONE\n{numberOfSegments} Path Segments\nPath Width: {pathWidth}m");
#endif
    }
}

/// <summary>
/// Simple trigger to activate exit when player reaches end
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DarknessZone zone = FindFirstObjectByType<DarknessZone>();
            if (zone != null)
            {
                zone.ActivateExit();
            }

            Debug.Log("✅ Reached end of Darkness Zone!");
        }
    }
}
