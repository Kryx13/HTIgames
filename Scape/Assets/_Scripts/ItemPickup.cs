using UnityEngine;

/// <summary>
/// Système de ramassage automatique au contact.
/// L'objet est ramassé dès que le joueur touche le trigger.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData; // La fiche d'identité de cet objet

    private void Start()
    {
        // Vérification de l'ItemData
        if (itemData == null)
        {
            Debug.LogError($"❌ ItemPickup sur {gameObject.name} : Aucun ItemData assigné dans l'Inspector !", this);
        }

        // Vérification du Collider (obligatoire pour les triggers)
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"❌ ItemPickup sur {gameObject.name} : Aucun Collider trouvé ! Ajoutez un BoxCollider, SphereCollider ou CapsuleCollider.", this);
        }
        else if (!col.isTrigger)
        {
            Debug.LogError($"❌ ItemPickup sur {gameObject.name} : Le Collider DOIT être en mode 'Is Trigger' pour le ramassage automatique !", this);
        }

        Debug.Log($"✅ ItemPickup {gameObject.name} configuré : {itemData?.itemName ?? "Aucun item"}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si c'est le joueur qui entre dans le trigger
        // On cherche le composant Inventory pour identifier le joueur
        Inventory playerInventory = other.GetComponent<Inventory>();

        if (playerInventory != null)
        {
            TryPickup(playerInventory);
        }
    }

    private void TryPickup(Inventory playerInventory)
    {
        if (itemData == null)
        {
            Debug.LogError("❌ Impossible de ramasser : ItemData manquant !");
            return;
        }

        // On essaie d'ajouter l'objet à l'inventaire
        bool wasAdded = playerInventory.AddItem(itemData);

        if (wasAdded)
        {
            Debug.Log($"✅ {itemData.itemName} ramassé automatiquement !");
            // Si ça a marché, on détruit l'objet au sol
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"⛔ Impossible de ramasser {itemData.itemName} : Inventaire plein !");
        }
    }
}
