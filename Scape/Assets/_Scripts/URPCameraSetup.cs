using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to automatically add URP Camera Data component to Main Camera.
/// Fixes the warning: "Camera does not contain an additional camera data component"
/// </summary>
[ExecuteInEditMode]
public class URPCameraSetup : MonoBehaviour
{
    [Header("Auto-Setup")]
    [SerializeField] private bool autoFixCameras = true;

    private void OnValidate()
    {
        if (autoFixCameras && Application.isEditor)
        {
#if UNITY_EDITOR
            FixMainCamera();
#endif
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Fix Main Camera URP Data")]
    public void FixMainCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("No Main Camera found in scene!");
            return;
        }

        // Check if UniversalAdditionalCameraData exists
        var cameraDataType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");

        if (cameraDataType == null)
        {
            Debug.LogError("URP not installed or UniversalAdditionalCameraData not found!");
            Debug.LogError("Make sure you're using Universal Render Pipeline.");
            return;
        }

        // Check if camera already has the component
        Component existingData = mainCam.GetComponent(cameraDataType);
        if (existingData != null)
        {
            Debug.Log("Main Camera already has URP Camera Data component!");
            return;
        }

        // Add the component
        Component cameraData = mainCam.gameObject.AddComponent(cameraDataType);
        if (cameraData != null)
        {
            Debug.Log("Added URP Camera Data to Main Camera!");
            EditorUtility.SetDirty(mainCam.gameObject);
        }
        else
        {
            Debug.LogError("Failed to add URP Camera Data component!");
        }
    }

    [MenuItem("GameObject/HTI Games/Fix All Cameras (URP)", false, 10)]
    public static void FixAllCamerasMenuItem()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        int cameraCount = 0;

        var cameraDataType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");

        if (cameraDataType == null)
        {
            Debug.LogError("URP not installed!");
            return;
        }

        foreach (Camera cam in cameras)
        {
            Component existingData = cam.GetComponent(cameraDataType);
            if (existingData == null)
            {
                cam.gameObject.AddComponent(cameraDataType);
                cameraCount++;
                Debug.Log("Fixed camera: " + cam.gameObject.name);
            }
        }

        Debug.Log("Fixed " + cameraCount + " camera(s)!");
    }
#endif

    /// <summary>
    /// Manual check for camera data at runtime
    /// </summary>
    [ContextMenu("Check Camera Setup")]
    public void CheckCameraSetup()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No Main Camera found!");
            return;
        }

        // Try to find URP camera data
        var cameraDataType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");

        if (cameraDataType == null)
        {
            Debug.LogWarning("URP Camera Data type not found (URP might not be installed)");
            return;
        }

        Component cameraData = mainCam.GetComponent(cameraDataType);
        if (cameraData != null)
        {
            Debug.Log("Main Camera has URP Camera Data component!");
        }
        else
        {
            Debug.LogError("Main Camera missing URP Camera Data component!");
            Debug.LogError("Select Main Camera in Hierarchy");
            Debug.LogError("Click 'Add Component'");
            Debug.LogError("Search for 'Universal Additional Camera Data'");
            Debug.LogError("Or use: GameObject > HTI Games > Fix All Cameras (URP)");
        }
    }
}
