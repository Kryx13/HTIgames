using UnityEngine;

/// <summary>
/// Script de setup automatique pour vérifier que tous les composants nécessaires
/// au système d'items sont présents dans la scène.
/// Attachez ce script à un GameObject vide dans la scène (ex: "ItemSystemManager").
/// </summary>
public class ItemSystemSetup : MonoBehaviour
{
    private void Awake()
    {
        // Vérifier si ItemVisibilityManager existe
        if (ItemVisibilityManager.Instance == null)
        {
            Debug.LogWarning("⚠️ ItemVisibilityManager non trouvé ! Création automatique...");

            // Créer un GameObject pour le manager
            GameObject managerObj = new GameObject("ItemVisibilityManager");
            managerObj.AddComponent<ItemVisibilityManager>();

            Debug.Log("✅ ItemVisibilityManager créé automatiquement");
        }
        else
        {
            Debug.Log("✅ ItemVisibilityManager détecté");
        }
    }
}
