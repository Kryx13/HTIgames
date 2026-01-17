using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData; // La fiche d'identité de cet objet

    public string InteractionPrompt => $"Ramasser {itemData.itemName}";

    public void Interact(PlayerController player)
    {
        // On récupère l'inventaire du joueur
        Inventory playerInventory = player.GetComponent<Inventory>();

        if (playerInventory != null)
        {
            // On essaie d'ajouter l'objet
            bool wasAdded = playerInventory.AddItem(itemData);

            if (wasAdded)
            {
                // Si ça a marché, on détruit l'objet au sol
                Destroy(gameObject); 
            }
        }
    }
}