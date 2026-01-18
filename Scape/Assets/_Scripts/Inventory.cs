using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxSlots = 2;

    [Header("Status")]
    [SerializeField] private bool hasBackpack = false;

    public List<ItemData> items = new List<ItemData>();

    public int MaxSlots => maxSlots;
    public int CurrentSlots => items.Count;
    public bool HasBackpack => hasBackpack;

    // ===== PERSISTANCE ENTRE SCÈNES =====
    // Stockage statique qui survit aux changements de scène
    private static List<string> savedItemNames = new List<string>();
    private static bool savedHasBackpack = false;
    private static int savedMaxSlots = 2;
    private static bool hasSavedData = false;

    private void Awake()
    {
        // Restaurer l'inventaire si des données ont été sauvegardées
        if (hasSavedData)
        {
            RestoreInventory();
        }

        // S'abonner aux changements de scène pour sauvegarder avant le changement
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Sauvegarder l'inventaire avant la destruction
        SaveInventory();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si on retourne au menu principal, réinitialiser l'inventaire sauvegardé
        if (scene.buildIndex == 0)
        {
            ClearSavedInventory();
            Debug.Log("🎒 Inventaire réinitialisé (retour au menu)");
        }
    }

    /// <summary>
    /// Sauvegarde l'état de l'inventaire dans les variables statiques
    /// </summary>
    private void SaveInventory()
    {
        savedItemNames.Clear();
        foreach (var item in items)
        {
            if (item != null)
            {
                savedItemNames.Add(item.itemName);
            }
        }
        savedHasBackpack = hasBackpack;
        savedMaxSlots = maxSlots;
        hasSavedData = true;

        Debug.Log($"💾 Inventaire sauvegardé: {savedItemNames.Count} items, Backpack={savedHasBackpack}");
    }

    /// <summary>
    /// Restaure l'inventaire depuis les variables statiques
    /// </summary>
    private void RestoreInventory()
    {
        hasBackpack = savedHasBackpack;
        maxSlots = savedMaxSlots;

        // Restaurer les items en trouvant les ItemData correspondants
        items.Clear();
        foreach (string itemName in savedItemNames)
        {
            // Chercher l'ItemData dans les Resources ou par référence
            ItemData itemData = FindItemDataByName(itemName);
            if (itemData != null)
            {
                items.Add(itemData);
            }
            else
            {
                Debug.LogWarning($"⚠️ Item '{itemName}' non trouvé lors de la restauration");
            }
        }

        Debug.Log($"📦 Inventaire restauré: {items.Count} items, Backpack={hasBackpack}");
    }

    /// <summary>
    /// Cherche un ItemData par son nom
    /// </summary>
    private ItemData FindItemDataByName(string itemName)
    {
        // Chercher dans Resources
        ItemData[] allItems = Resources.LoadAll<ItemData>("");
        foreach (var item in allItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        // Chercher dans les objets chargés
        ItemData[] loadedItems = Resources.FindObjectsOfTypeAll<ItemData>();
        foreach (var item in loadedItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// Efface les données sauvegardées (nouvelle partie)
    /// </summary>
    public static void ClearSavedInventory()
    {
        savedItemNames.Clear();
        savedHasBackpack = false;
        savedMaxSlots = 2;
        hasSavedData = false;
    }

    public bool AddItem(ItemData itemToAdd)
    {
        if (itemToAdd.isBackpack)
        {
            if (hasBackpack)
            {
                Debug.Log("You already have a backpack!");
                return false;
            }

            maxSlots += 3;
            hasBackpack = true;
            Debug.Log($"Backpack equipped! Capacity: {items.Count}/{maxSlots} (+3 slots)");
            return true;
        }

        if (items.Count >= maxSlots)
        {
            Debug.Log($"Inventory full! ({items.Count}/{maxSlots})");
            return false;
        }

        items.Add(itemToAdd);
        Debug.Log($"{itemToAdd.itemName} added | Slots: {items.Count}/{maxSlots}");
        return true;
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
        }
    }

    public void RemoveItem(string itemNameToRemove)
    {
        ItemData itemToRemove = null;
        foreach(var item in items)
        {
            if(item.itemName == itemNameToRemove)
            {
                itemToRemove = item;
                break;
            }
        }
        if(itemToRemove != null)
        {
            items.Remove(itemToRemove);
        }
    }

    public bool HasItem(string itemNameToCheck)
    {
        foreach(var item in items)
        {
            if(item.itemName == itemNameToCheck) return true;
        }
        return false;
    }
}
