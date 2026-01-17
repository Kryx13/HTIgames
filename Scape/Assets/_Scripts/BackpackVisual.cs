using UnityEngine;

/// <summary>
/// Affiche un sac à dos visuel sur le joueur quand il est équipé.
/// Ce script écoute les changements de l'inventaire et active/désactive le modèle 3D du sac.
/// </summary>
public class BackpackVisual : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject backpackModel; // Le modèle 3D du sac (enfant du Player)

    [Header("Settings")]
    [SerializeField] private bool autoCreateVisual = true; // Créer automatiquement un cube si pas de modèle

    [Header("Visual Position (adjust in Inspector)")]
    [SerializeField] private Vector3 backpackPosition = new Vector3(0, 0.5f, -0.65f); // Position locale (reculé et baissé)
    [SerializeField] private Vector3 backpackScale = new Vector3(0.4f, 0.5f, 0.2f); // Taille
    [SerializeField] private Color backpackColor = new Color(0.4f, 0.25f, 0.1f); // Couleur marron

    private bool wasEquipped = false;

    private void Start()
    {
        // Récupérer l'inventaire si non assigné
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }

        // Créer un visuel simple si activé et pas de modèle
        if (autoCreateVisual && backpackModel == null)
        {
            CreateSimpleBackpack();
        }

        // Cacher le sac au départ
        if (backpackModel != null)
        {
            backpackModel.SetActive(false);
        }
    }

    private void Update()
    {
        // Vérifier si le sac a été équipé
        if (inventory != null && inventory.HasBackpack && !wasEquipped)
        {
            ShowBackpack();
            wasEquipped = true;
        }
    }

    private void ShowBackpack()
    {
        if (backpackModel != null)
        {
            backpackModel.SetActive(true);
            Debug.Log("🎒 Sac à dos visible sur le joueur !");
        }
    }

    /// <summary>
    /// Crée un sac visuel simple (cube) sur le dos du joueur
    /// </summary>
    private void CreateSimpleBackpack()
    {
        backpackModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backpackModel.name = "BackpackVisual";
        backpackModel.transform.SetParent(transform);

        // Positionner sur le dos du joueur (réglable dans l'Inspector)
        backpackModel.transform.localPosition = backpackPosition;
        backpackModel.transform.localScale = backpackScale;
        backpackModel.transform.localRotation = Quaternion.identity;

        // Supprimer le collider (c'est juste visuel)
        Destroy(backpackModel.GetComponent<Collider>());

        // Couleur du sac
        Renderer renderer = backpackModel.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = backpackColor;
        }

        Debug.Log($"✅ Sac à dos visuel créé à la position locale {backpackPosition}");
    }

    // Pour ajuster la position en temps réel dans l'éditeur
    private void OnValidate()
    {
        if (backpackModel != null && Application.isPlaying)
        {
            backpackModel.transform.localPosition = backpackPosition;
            backpackModel.transform.localScale = backpackScale;

            Renderer renderer = backpackModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = backpackColor;
            }
        }
    }
}
