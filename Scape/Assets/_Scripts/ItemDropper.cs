using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Système pour lâcher/jeter des objets de l'inventaire.
/// Appuyez sur G pour lâcher le dernier objet ramassé.
/// </summary>
public class ItemDropper : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private float dropDistance = 2f; // Distance devant le joueur
    [SerializeField] private float dropHeight = 0.5f; // Hauteur du drop
    [SerializeField] private GameObject itemPickupPrefab; // Prefab générique pour les items

    private Inventory inventory;
    private Camera mainCam;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        mainCam = Camera.main;

        if (inventory == null)
        {
            Debug.LogError("❌ ItemDropper : Aucun composant Inventory trouvé sur le Player !");
        }
    }

    private void Update()
    {
        // Détection de la touche G via le nouveau Input System
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            DropLastItem();
        }
    }

    /// <summary>
    /// Lâche le dernier objet de l'inventaire
    /// </summary>
    public void DropLastItem()
    {
        if (inventory == null || inventory.items.Count == 0)
        {
            Debug.Log("⚠️ Aucun objet à lâcher (inventaire vide)");
            return;
        }

        // Récupérer le dernier objet
        ItemData itemToDrop = inventory.items[inventory.items.Count - 1];

        // Retirer de l'inventaire
        inventory.RemoveItem(itemToDrop);

        // Créer l'objet dans le monde
        SpawnDroppedItem(itemToDrop);

        Debug.Log($"📦 {itemToDrop.itemName} lâché !");
    }

    /// <summary>
    /// Fait apparaître l'objet devant le joueur
    /// </summary>
    private void SpawnDroppedItem(ItemData itemData)
    {
        // Calculer la position devant le joueur
        Vector3 dropPosition = transform.position + transform.forward * dropDistance;
        dropPosition.y += dropHeight;

        // Créer un nouveau GameObject pour l'item
        GameObject droppedItem = new GameObject($"Pickup_{itemData.itemName}");
        droppedItem.transform.position = dropPosition;
        droppedItem.layer = 6; // Layer Interactable

        // Ajouter les composants nécessaires
        ItemPickup pickup = droppedItem.AddComponent<ItemPickup>();

        // Utiliser reflection pour assigner l'itemData (car c'est un champ privé serialized)
        var field = typeof(ItemPickup).GetField("itemData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(pickup, itemData);
        }

        // Ajouter un collider trigger
        SphereCollider col = droppedItem.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.5f;

        // Ajouter une représentation visuelle simple (cube)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(droppedItem.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one * 0.3f;

        // Supprimer le collider du cube visuel (on utilise celui du parent)
        Destroy(visual.GetComponent<Collider>());

        // Optionnel : Ajouter un petit mouvement de rotation
        ItemRotator rotator = droppedItem.AddComponent<ItemRotator>();

        Debug.Log($"✅ {itemData.itemName} créé à la position {dropPosition}");
    }
}

/// <summary>
/// Petit script pour faire tourner l'objet sur lui-même (visuel)
/// </summary>
public class ItemRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobAmount = 0.2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bobbing (mouvement haut/bas)
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
