using UnityEngine;

public class PlatformResetZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FallingPlatform[] platforms = FindObjectsOfType<FallingPlatform>();
            foreach (FallingPlatform platform in platforms)
            {
                platform.ResetPlatform();
            }
            Debug.Log("🔄 Toutes les plateformes réinitialisées !");
        }
    }
}