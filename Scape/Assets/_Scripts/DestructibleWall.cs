using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mur destructible qui peut être cassé avec la pioche.
/// Peut déclencher des événements quand cassé (ouverture de passage, etc.).
/// </summary>
public class DestructibleWall : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3; // Nombre de coups pour détruire
    [SerializeField] private bool showHealthInName = true; // Afficher la vie dans le nom

    [Header("Visual Feedback")]
    [SerializeField] private bool changeMaterialOnDamage = true;
    [SerializeField] private Color damagedColor = new Color(0.7f, 0.3f, 0.3f); // Rouge/marron
    [SerializeField] private float damageFlashDuration = 0.2f;

    [Header("Destruction")]
    [SerializeField] private GameObject destructionEffectPrefab; // Effet de destruction
    [SerializeField] private bool autoCreateEffect = true;
    [SerializeField] private float effectDuration = 2f;

    [Header("Events")]
    [SerializeField] private UnityEvent onDamaged; // Quand frappé
    [SerializeField] private UnityEvent onDestroyed; // Quand détruit

    [Header("Loot (optional)")]
    [SerializeField] private bool dropItemOnDestroy = false;
    [SerializeField] private GameObject itemPrefab; // Item à faire tomber

    private int currentHealth;
    private Renderer wallRenderer;
    private Color originalColor;
    private bool isDestroyed = false;
    private string originalName;

    private void Start()
    {
        currentHealth = maxHealth;
        wallRenderer = GetComponent<Renderer>();
        originalName = gameObject.name;

        if (wallRenderer != null)
        {
            originalColor = wallRenderer.material.color;
        }

        // Créer un effet de destruction simple si nécessaire
        if (autoCreateEffect && destructionEffectPrefab == null)
        {
            CreateSimpleDestructionEffect();
        }

        UpdateHealthDisplay();

        Debug.Log($"🧱 Mur destructible créé : {gameObject.name} ({currentHealth} HP)");
    }

    /// <summary>
    /// Inflige des dégâts au mur
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        Debug.Log($"🧱 Mur {gameObject.name} frappé ! ({currentHealth}/{maxHealth} HP restants)");

        // Feedback visuel
        if (changeMaterialOnDamage)
        {
            StartCoroutine(DamageFlash());
        }

        // Mettre à jour l'affichage de la vie
        UpdateHealthDisplay();

        // Déclencher l'événement onDamaged
        onDamaged?.Invoke();

        // Vérifier si le mur est détruit
        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    /// <summary>
    /// Détruit le mur
    /// </summary>
    private void DestroyWall()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        Debug.Log($"💥 Mur {gameObject.name} détruit !");

        // Déclencher l'événement onDestroyed
        onDestroyed?.Invoke();

        // Créer l'effet de destruction
        if (destructionEffectPrefab != null)
        {
            GameObject effect = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }

        // Faire tomber un item (optionnel)
        if (dropItemOnDestroy && itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        // Détruire le mur
        Destroy(gameObject);
    }

    /// <summary>
    /// Flash rouge quand le mur est frappé
    /// </summary>
    private System.Collections.IEnumerator DamageFlash()
    {
        if (wallRenderer != null)
        {
            // Couleur endommagée
            wallRenderer.material.color = damagedColor;

            yield return new WaitForSeconds(damageFlashDuration);

            // Revenir à la couleur originale (si pas détruit)
            if (!isDestroyed && wallRenderer != null)
            {
                wallRenderer.material.color = originalColor;
            }
        }
    }

    /// <summary>
    /// Met à jour l'affichage de la vie (dans le nom)
    /// </summary>
    private void UpdateHealthDisplay()
    {
        if (showHealthInName)
        {
            gameObject.name = $"{originalName} [{currentHealth}/{maxHealth}]";
        }
    }

    /// <summary>
    /// Crée un effet de destruction simple (cubes qui explosent)
    /// </summary>
    private void CreateSimpleDestructionEffect()
    {
        destructionEffectPrefab = new GameObject("DestructionEffect");

        // Créer des débris qui explosent
        for (int i = 0; i < 10; i++)
        {
            GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debris.name = "Debris";
            debris.transform.SetParent(destructionEffectPrefab.transform);
            debris.transform.localPosition = Random.insideUnitSphere * 0.5f;
            debris.transform.localScale = Vector3.one * Random.Range(0.1f, 0.3f);

            // Couleur aléatoire grise/marron
            Renderer renderer = debris.GetComponent<Renderer>();
            if (renderer != null)
            {
                float gray = Random.Range(0.3f, 0.6f);
                renderer.material.color = new Color(gray, gray * 0.8f, gray * 0.6f);
            }

            // Ajouter un Rigidbody pour l'explosion
            Rigidbody rb = debris.AddComponent<Rigidbody>();
            rb.mass = 0.1f;
            rb.AddExplosionForce(5f, destructionEffectPrefab.transform.position, 2f);
        }

        // Désactiver pour l'instant
        destructionEffectPrefab.SetActive(false);

        Debug.Log("✅ Effet de destruction simple créé");
    }

    /// <summary>
    /// Restaure le mur (pour tests ou puzzles réinitialisables)
    /// </summary>
    public void ResetWall()
    {
        currentHealth = maxHealth;
        isDestroyed = false;

        if (wallRenderer != null)
        {
            wallRenderer.material.color = originalColor;
        }

        UpdateHealthDisplay();

        Debug.Log($"🔄 Mur {originalName} réinitialisé");
    }

    // Propriétés publiques
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDestroyed => isDestroyed;
}
