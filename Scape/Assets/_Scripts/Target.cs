using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Cible qui peut être touchée par le pistolet.
/// Peut activer des événements quand touchée ou détruite.
/// </summary>
public class Target : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private int maxHealth = 1; // Points de vie
    [SerializeField] private bool destroyOnHit = true; // Détruire dès qu'on est touché
    [SerializeField] private float destroyDelay = 0f; // Délai avant destruction

    [Header("Visual Feedback")]
    [SerializeField] private Color hitColor = Color.yellow; // Couleur quand touché
    [SerializeField] private float hitColorDuration = 0.2f; // Durée du feedback visuel
    [SerializeField] private bool changeColorOnHit = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onHit; // Événement déclenché quand touché
    [SerializeField] private UnityEvent onDestroyed; // Événement déclenché quand détruit

    private int currentHealth;
    private Renderer targetRenderer;
    private Color originalColor;
    private bool isDestroyed = false;

    private void Start()
    {
        currentHealth = maxHealth;
        targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color;
        }

        Debug.Log($"🎯 Cible {gameObject.name} créée ({currentHealth} HP)");
    }

    /// <summary>
    /// Inflige des dégâts à la cible
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        Debug.Log($"🎯 Cible {gameObject.name} touchée ! ({currentHealth}/{maxHealth} HP)");

        // Feedback visuel
        if (changeColorOnHit)
        {
            StartCoroutine(HitFeedback());
        }

        // Déclencher l'événement onHit
        onHit?.Invoke();

        // Vérifier si la cible est détruite
        if (currentHealth <= 0 || destroyOnHit)
        {
            DestroyTarget();
        }
    }

    /// <summary>
    /// Détruit la cible
    /// </summary>
    private void DestroyTarget()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        Debug.Log($"💥 Cible {gameObject.name} détruite !");

        // Déclencher l'événement onDestroyed
        onDestroyed?.Invoke();

        // Détruire l'objet
        Destroy(gameObject, destroyDelay);
    }

    /// <summary>
    /// Feedback visuel quand la cible est touchée
    /// </summary>
    private System.Collections.IEnumerator HitFeedback()
    {
        if (targetRenderer != null)
        {
            // Changer la couleur
            targetRenderer.material.color = hitColor;

            // Attendre
            yield return new WaitForSeconds(hitColorDuration);

            // Revenir à la couleur originale (si pas détruit)
            if (!isDestroyed && targetRenderer != null)
            {
                targetRenderer.material.color = originalColor;
            }
        }
    }

    /// <summary>
    /// Restaure la vie de la cible (pour réutilisation)
    /// </summary>
    public void ResetTarget()
    {
        currentHealth = maxHealth;
        isDestroyed = false;

        if (targetRenderer != null)
        {
            targetRenderer.material.color = originalColor;
        }

        Debug.Log($"🔄 Cible {gameObject.name} réinitialisée");
    }

    // Propriétés publiques
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDestroyed => isDestroyed;
}
