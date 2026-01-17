using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon; // L'image pour l'inventaire (plus tard)
    public GameObject prefab; // L'objet 3D à faire apparaître si on le jette (optionnel)
    
    [Header("Type")]
    public bool isBackpack; // Spécial pour le sac à dos
}