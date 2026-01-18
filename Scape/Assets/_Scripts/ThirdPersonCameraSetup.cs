using UnityEngine;

/// <summary>
/// Automatically sets up the 3rd person Cinemachine camera to follow the player.
/// Add this script to the PlayerFollowCamera or any object in the scene.
/// </summary>
public class ThirdPersonCameraSetup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform followTarget; // Player's CameraRoot
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string cameraRootName = "CameraRoot";

    [Header("Auto-Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool createCameraIfMissing = true;

    [Header("Camera Prefab")]
    [SerializeField] private GameObject playerFollowCameraPrefab;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCamera();
        }
    }

    /// <summary>
    /// Sets up the Cinemachine camera to follow the player
    /// </summary>
    [ContextMenu("Setup Camera")]
    public void SetupCamera()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("ThirdPersonCameraSetup: No player found with tag '" + playerTag + "'");
            return;
        }

        // Find the CameraRoot on the player
        Transform cameraRoot = FindCameraRoot(player.transform);
        if (cameraRoot == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("ThirdPersonCameraSetup: No CameraRoot found on player");
            return;
        }

        followTarget = cameraRoot;

        // Find or create the Cinemachine Virtual Camera
        var virtualCamera = FindOrCreateVirtualCamera();
        if (virtualCamera == null)
        {
            if (showDebugLogs)
                Debug.LogError("ThirdPersonCameraSetup: Could not find or create Cinemachine Virtual Camera");
            return;
        }

        // Set the Follow and LookAt targets using reflection (to avoid direct Cinemachine dependency)
        SetCinemachineTargets(virtualCamera, cameraRoot);

        if (showDebugLogs)
            Debug.Log("ThirdPersonCameraSetup: Camera configured successfully!");
    }

    /// <summary>
    /// Finds the CameraRoot transform on the player
    /// </summary>
    private Transform FindCameraRoot(Transform playerTransform)
    {
        // First try direct child
        Transform cameraRoot = playerTransform.Find(cameraRootName);
        if (cameraRoot != null)
            return cameraRoot;

        // Try recursive search
        cameraRoot = FindChildRecursive(playerTransform, cameraRootName);
        if (cameraRoot != null)
            return cameraRoot;

        // Try finding PlayerCameraRoot (alternative name)
        cameraRoot = playerTransform.Find("PlayerCameraRoot");
        if (cameraRoot != null)
            return cameraRoot;

        cameraRoot = FindChildRecursive(playerTransform, "PlayerCameraRoot");
        return cameraRoot;
    }

    /// <summary>
    /// Recursively searches for a child transform by name
    /// </summary>
    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// Finds an existing Cinemachine Virtual Camera or creates one from prefab
    /// </summary>
    private Component FindOrCreateVirtualCamera()
    {
        // Try to find CinemachineVirtualCamera type
        System.Type vcamType = System.Type.GetType("Cinemachine.CinemachineVirtualCamera, Cinemachine") ??
                               System.Type.GetType("Unity.Cinemachine.CinemachineVirtualCamera, Unity.Cinemachine");

        if (vcamType == null)
        {
            if (showDebugLogs)
                Debug.LogError("ThirdPersonCameraSetup: Cinemachine not found! Make sure Cinemachine package is installed.");
            return null;
        }

        // Try to find existing virtual camera in scene
        Component existingVCam = FindObjectOfType(vcamType) as Component;
        if (existingVCam != null)
        {
            if (showDebugLogs)
                Debug.Log("ThirdPersonCameraSetup: Found existing Cinemachine Virtual Camera: " + existingVCam.gameObject.name);
            return existingVCam;
        }

        // Try to find PlayerFollowCamera by name
        GameObject playerFollowCam = GameObject.Find("PlayerFollowCamera");
        if (playerFollowCam != null)
        {
            Component vcam = playerFollowCam.GetComponent(vcamType);
            if (vcam != null)
            {
                if (showDebugLogs)
                    Debug.Log("ThirdPersonCameraSetup: Found PlayerFollowCamera");
                return vcam;
            }
        }

        // Create from prefab if available
        if (createCameraIfMissing && playerFollowCameraPrefab != null)
        {
            GameObject newCam = Instantiate(playerFollowCameraPrefab);
            newCam.name = "PlayerFollowCamera";
            Component vcam = newCam.GetComponent(vcamType);
            if (vcam != null)
            {
                if (showDebugLogs)
                    Debug.Log("ThirdPersonCameraSetup: Created PlayerFollowCamera from prefab");
                return vcam;
            }
        }

        // Try to load from Resources
        if (createCameraIfMissing)
        {
            GameObject prefab = Resources.Load<GameObject>("PlayerFollowCamera");
            if (prefab != null)
            {
                GameObject newCam = Instantiate(prefab);
                newCam.name = "PlayerFollowCamera";
                Component vcam = newCam.GetComponent(vcamType);
                if (vcam != null)
                {
                    if (showDebugLogs)
                        Debug.Log("ThirdPersonCameraSetup: Created PlayerFollowCamera from Resources");
                    return vcam;
                }
            }
        }

        if (showDebugLogs)
            Debug.LogWarning("ThirdPersonCameraSetup: No Cinemachine Virtual Camera found and could not create one. " +
                           "Please add the PlayerFollowCamera prefab to your scene manually.");

        return null;
    }

    /// <summary>
    /// Sets the Follow and LookAt targets on the Cinemachine camera using reflection
    /// </summary>
    private void SetCinemachineTargets(Component virtualCamera, Transform target)
    {
        if (virtualCamera == null || target == null)
            return;

        System.Type vcamType = virtualCamera.GetType();

        // Set Follow target
        var followProperty = vcamType.GetProperty("Follow");
        if (followProperty != null)
        {
            followProperty.SetValue(virtualCamera, target);
            if (showDebugLogs)
                Debug.Log("ThirdPersonCameraSetup: Set Follow target to " + target.name);
        }

        // Set LookAt target
        var lookAtProperty = vcamType.GetProperty("LookAt");
        if (lookAtProperty != null)
        {
            lookAtProperty.SetValue(virtualCamera, target);
            if (showDebugLogs)
                Debug.Log("ThirdPersonCameraSetup: Set LookAt target to " + target.name);
        }
    }

    /// <summary>
    /// Editor visualization
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (followTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(followTarget.position, 0.5f);
            Gizmos.DrawLine(transform.position, followTarget.position);
        }
    }
}
