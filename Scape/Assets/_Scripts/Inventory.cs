using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxSlots = 2; // 2 places par défaut

    [Header("Status")]
    [SerializeField] private bool hasBackpack = false; // Pour suivre si le sac a été équipé

    // Notre liste d'objets actuelle
    public List<ItemData> items = new List<ItemData>();

    // Propriétés publiques
    public int MaxSlots => maxSlots;
    public int CurrentSlots => items.Count;
    public bool HasBackpack => hasBackpack;

    public bool AddItem(ItemData itemToAdd)
    {
        // Vérification spéciale : Est-ce un sac à dos ?
        if (itemToAdd.isBackpack)
        {
            // Empêcher de ramasser plusieurs sacs
            if (hasBackpack)
            {
                Debug.Log("⚠️ Vous avez déjà un sac à dos !");
                return false;
            }

            maxSlots += 3; // +3 places
            hasBackpack = true;
            Debug.Log($"🎒 Sac à dos équipé ! Capacité : {items.Count}/{maxSlots} (+3 emplacements)");
            return true; // On "consomme" l'objet sans le mettre DANS l'inventaire
        }

        // Vérification de place
        if (items.Count >= maxSlots)
        {
            Debug.Log($"⛔ Inventaire plein ! ({items.Count}/{maxSlots})");
            return false;
        }

        items.Add(itemToAdd);
        Debug.Log($"➕ {itemToAdd.itemName} ajouté | Places : {items.Count}/{maxSlots}");
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