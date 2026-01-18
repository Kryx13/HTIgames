using UnityEngine;

/// <summary>
/// Identifies a room in the door maze (Stage 2).
/// Detects when the player enters the room and updates the UI.
/// </summary>
public class RoomIdentifier : MonoBehaviour
{
    [Header("Room Info")]
    [SerializeField] private int roomNumber = 1; // Room number (1-5)
    [SerializeField] private string roomName = "Room 1"; // Optional display name

    [Header("Detection")]
    [SerializeField] private bool autoDetectPlayer = true;
    [SerializeField] private float detectionRadius = 2f; // Distance to detect player

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private static int currentRoomNumber = 1; // Global current room
    private static RoomIdentifier currentRoom;
    private bool playerInRoom = false;

    private void Start()
    {
        // If this is room 1, set it as current by default
        if (roomNumber == 1 && currentRoom == null)
        {
            SetAsCurrentRoom();
        }
    }

    private void Update()
    {
        if (autoDetectPlayer)
        {
            DetectPlayer();
        }
    }

    /// <summary>
    /// Detects if the player is in this room
    /// </summary>
    private void DetectPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            bool nowInRoom = distance <= detectionRadius;

            // Player entered room
            if (nowInRoom && !playerInRoom)
            {
                playerInRoom = true;
                SetAsCurrentRoom();
            }
            // Player left room
            else if (!nowInRoom && playerInRoom)
            {
                playerInRoom = false;
            }
        }
    }

    /// <summary>
    /// Sets this room as the current room
    /// </summary>
    public void SetAsCurrentRoom()
    {
        currentRoomNumber = roomNumber;
        currentRoom = this;

        if (showDebugLogs)
        {
            Debug.Log($"📍 Entered {roomName} (Room {roomNumber})");
        }

        // Notify UI
        RoomNumberUI ui = FindObjectOfType<RoomNumberUI>();
        if (ui != null)
        {
            ui.UpdateRoomNumber(roomNumber, roomName);
        }
    }

    /// <summary>
    /// Gets the current room number
    /// </summary>
    public static int GetCurrentRoomNumber()
    {
        return currentRoomNumber;
    }

    /// <summary>
    /// Gets the current room
    /// </summary>
    public static RoomIdentifier GetCurrentRoom()
    {
        return currentRoom;
    }

    /// <summary>
    /// Manually trigger room entry (called by door triggers)
    /// </summary>
    public void OnPlayerEntered()
    {
        SetAsCurrentRoom();
    }

    /// <summary>
    /// Gizmo to visualize room detection radius
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f); // Cyan transparent
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);

        // Draw room number text
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"ROOM {roomNumber}");
#endif
    }
}
