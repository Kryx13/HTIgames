using UnityEngine;

/// <summary>
/// Connects doors in the maze to teleport between rooms.
/// Works with DoorTrigger to handle room-to-room teleportation.
/// Automatically notifies RoomIdentifier when player enters a new room.
/// </summary>
public class DoorMazeConnector : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private int targetRoomNumber = 2; // Which room this door leads to
    [SerializeField] private Transform targetSpawnPoint; // Where player appears in target room

    [Header("Visual")]
    [SerializeField] private Color doorColor = Color.blue;
    [SerializeField] private bool showLabel = true;

    [Header("Auto-Setup")]
    [SerializeField] private bool autoConfigureDoorTrigger = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private DoorTrigger doorTrigger;

    private void Start()
    {
        doorTrigger = GetComponent<DoorTrigger>();

        if (autoConfigureDoorTrigger && doorTrigger != null)
        {
            ConfigureDoorTrigger();
        }

        // Color the door
        ColorDoor();
    }

    /// <summary>
    /// Automatically configures the DoorTrigger component
    /// </summary>
    private void ConfigureDoorTrigger()
    {
        // Use reflection to set DoorTrigger fields
        var doorTypeField = typeof(DoorTrigger).GetField("doorType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (doorTypeField != null)
        {
            doorTypeField.SetValue(doorTrigger, DoorTrigger.DoorType.Teleport);
        }

        var destField = typeof(DoorTrigger).GetField("teleportDestination", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (destField != null && targetSpawnPoint != null)
        {
            destField.SetValue(doorTrigger, targetSpawnPoint);
        }

        if (showDebugLogs)
        {
            Debug.Log($"✅ Door configured to teleport to Room {targetRoomNumber}");
        }
    }

    /// <summary>
    /// Colors the door model for visual identification
    /// </summary>
    private void ColorDoor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = doorColor;
        }

        // Try to find door model as child
        Transform doorModel = transform.Find("DoorModel");
        if (doorModel != null)
        {
            Renderer modelRenderer = doorModel.GetComponent<Renderer>();
            if (modelRenderer != null)
            {
                modelRenderer.material.color = doorColor;
            }
        }
    }

    /// <summary>
    /// Called when player goes through this door
    /// </summary>
    public void OnPlayerTeleported()
    {
        // Find the target room and notify it
        RoomIdentifier[] rooms = FindObjectsOfType<RoomIdentifier>();
        foreach (RoomIdentifier room in rooms)
        {
            // Use reflection to get room number
            var roomNumField = room.GetType().GetField("roomNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (roomNumField != null)
            {
                int roomNum = (int)roomNumField.GetValue(room);
                if (roomNum == targetRoomNumber)
                {
                    room.OnPlayerEntered();
                    break;
                }
            }
        }

        if (showDebugLogs)
        {
            Debug.Log($"🚪 Player teleported to Room {targetRoomNumber}");
        }
    }

    /// <summary>
    /// Sets the target room dynamically
    /// </summary>
    public void SetTargetRoom(int roomNumber, Transform spawnPoint)
    {
        targetRoomNumber = roomNumber;
        targetSpawnPoint = spawnPoint;

        if (doorTrigger != null)
        {
            ConfigureDoorTrigger();
        }
    }

    /// <summary>
    /// Gizmo to visualize door connection
    /// </summary>
    private void OnDrawGizmos()
    {
        if (targetSpawnPoint != null)
        {
            Gizmos.color = doorColor;
            Gizmos.DrawLine(transform.position, targetSpawnPoint.position);
            Gizmos.DrawWireSphere(targetSpawnPoint.position, 0.5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Show label
#if UNITY_EDITOR
        if (showLabel)
        {
            UnityEditor.Handles.color = doorColor;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"→ Room {targetRoomNumber}");
        }
#endif
    }
}
