using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxSlots = 2; // 2 places par défaut

    // Notre liste d'objets actuelle
    public List<ItemData> items = new List<ItemData>();

    public bool AddItem(ItemData itemToAdd)
    {
        // Vérification spéciale : Est-ce un sac à dos ?
        if (itemToAdd.isBackpack)
        {
            maxSlots += 3; // +3 places
            Debug.Log($"🎒 Sac à dos équipé ! Places : {items.Count}/{maxSlots}");
            return true; // On "consomme" l'objet sans le mettre DANS l'inventaire
        }

        // Vérification de place
        if (items.Count >= maxSlots)
        {
            Debug.Log("⛔ Inventaire plein !");
            return false;
        }

        items.Add(itemToAdd);
        Debug.Log($"➕ Ajouté : {itemToAdd.itemName} | Places : {items.Count}/{maxSlots}");
        return true;
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
        }
    }
    
    // Pour savoir si on possède un objet spécifique (ex: Pistolet)
    public bool HasItem(string itemNameToCheck)
    {
        foreach(var item in items)
        {
            if(item.itemName == itemNameToCheck) return true;
        }
        return false;
    }
}