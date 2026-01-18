using UnityEngine;
using System.Collections.Generic;

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
