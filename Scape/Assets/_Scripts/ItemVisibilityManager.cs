using UnityEngine;

/// <summary>
/// Gère la visibilité de tous les objets équipés (Pistolet, Lampe, Pioche).
/// - Un seul objet visible à la fois
/// - Cache après 5 secondes d'inactivité
/// - Cache quand le joueur sprint
/// - Le sac reste toujours visible
/// </summary>
public class ItemVisibilityManager : MonoBehaviour
{
    public static ItemVisibilityManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float hideDelay = 5f; // Temps avant de cacher (secondes)
    [SerializeField] private bool hideOnSprint = true; // Cacher quand sprint

    [Header("Item Models")]
    [SerializeField] private GameObject gunModel;
    [SerializeField] private GameObject flashlightModel;
    [SerializeField] private GameObject pickaxeModel;

    [Header("Status (Read-only)")]
    [SerializeField] private ItemType currentVisibleItem = ItemType.None;
    [SerializeField] private float lastUseTime = 0f;

    private InputManager inputManager;

    public enum ItemType
    {
        None,
        Gun,
        Flashlight,
        Pickaxe
    }

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        HideAll();
    }

    private void Update()
    {
        // Cacher après inactivité
        if (currentVisibleItem != ItemType.None && Time.time - lastUseTime > hideDelay)
        {
            HideAll();
        }

        // Cacher si sprint
        if (hideOnSprint && inputManager != null && inputManager.RunHeld)
        {
            HideAll();
        }
    }

    /// <summary>
    /// Affiche un objet et cache les autres
    /// </summary>
    public void ShowItem(ItemType item, GameObject model = null)
    {
        // Cacher tous les autres
        HideAll();

        // Enregistrer le modèle si fourni
        if (model != null)
        {
            switch (item)
            {
                case ItemType.Gun:
                    gunModel = model;
                    break;
                case ItemType.Flashlight:
                    flashlightModel = model;
                    break;
                case ItemType.Pickaxe:
                    pickaxeModel = model;
                    break;
            }
        }

        // Afficher l'objet demandé
        GameObject targetModel = GetModelForType(item);
        if (targetModel != null)
        {
            targetModel.SetActive(true);
            currentVisibleItem = item;
            lastUseTime = Time.time;
        }
    }

    /// <summary>
    /// Cache tous les objets
    /// </summary>
    public void HideAll()
    {
        if (gunModel != null) gunModel.SetActive(false);
        if (flashlightModel != null) flashlightModel.SetActive(false);
        if (pickaxeModel != null) pickaxeModel.SetActive(false);

        currentVisibleItem = ItemType.None;
    }

    /// <summary>
    /// Cache un objet spécifique
    /// </summary>
    public void HideItem(ItemType item)
    {
        GameObject model = GetModelForType(item);
        if (model != null)
        {
            model.SetActive(false);
        }

        if (currentVisibleItem == item)
        {
            currentVisibleItem = ItemType.None;
        }
    }

    /// <summary>
    /// Met à jour le temps de dernière utilisation
    /// </summary>
    public void UpdateLastUseTime()
    {
        lastUseTime = Time.time;
    }

    /// <summary>
    /// Enregistre un modèle d'objet
    /// </summary>
    public void RegisterItemModel(ItemType type, GameObject model)
    {
        switch (type)
        {
            case ItemType.Gun:
                gunModel = model;
                break;
            case ItemType.Flashlight:
                flashlightModel = model;
                break;
            case ItemType.Pickaxe:
                pickaxeModel = model;
                break;
        }
    }

    /// <summary>
    /// Récupère le modèle pour un type donné
    /// </summary>
    private GameObject GetModelForType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Gun:
                return gunModel;
            case ItemType.Flashlight:
                return flashlightModel;
            case ItemType.Pickaxe:
                return pickaxeModel;
            default:
                return null;
        }
    }

    // Propriétés publiques
    public ItemType CurrentVisibleItem => currentVisibleItem;
    public bool IsItemVisible(ItemType type) => currentVisibleItem == type;
}
