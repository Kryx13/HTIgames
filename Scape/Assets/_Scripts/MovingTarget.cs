using UnityEngine;

/// <summary>
/// Cible mobile qui se déplace selon différents patterns.
/// Utilisé pour le Stage 3 - Shooting Gallery.
/// </summary>
public class MovingTarget : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private MovementType movementType = MovementType.Horizontal;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveRange = 5f; // Distance de déplacement

    [Header("Random Movement (Type: Random)")]
    [SerializeField] private float randomChangeInterval = 1f; // Changement de direction

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveTimer = 0f;
#pragma warning disable 0414 // Field assigned but never used
    private int direction = 1; // 1 = right/up, -1 = left/down (reserved for future use)
#pragma warning restore 0414

    public enum MovementType
    {
        Static,           // Ne bouge pas
        Horizontal,       // Gauche-droite
        Vertical,         // Haut-bas
        Random            // Mouvement aléatoire
    }

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;

        Debug.Log($"🎯 Cible mobile créée ({movementType})");
    }

    private void Update()
    {
        switch (movementType)
        {
            case MovementType.Static:
                // Ne bouge pas
                break;

            case MovementType.Horizontal:
                MoveHorizontal();
                break;

            case MovementType.Vertical:
                MoveVertical();
                break;

            case MovementType.Random:
                MoveRandom();
                break;
        }
    }

    /// <summary>
    /// Mouvement horizontal (gauche-droite)
    /// </summary>
    private void MoveHorizontal()
    {
        // Calculer la nouvelle position
        float newX = startPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    /// <summary>
    /// Mouvement vertical (haut-bas)
    /// </summary>
    private void MoveVertical()
    {
        // Calculer la nouvelle position
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    /// <summary>
    /// Mouvement aléatoire (imprévisible)
    /// </summary>
    private void MoveRandom()
    {
        moveTimer += Time.deltaTime;

        // Changer de direction aléatoirement
        if (moveTimer >= randomChangeInterval)
        {
            moveTimer = 0f;

            // Nouvelle position aléatoire dans le range
            float randomX = startPosition.x + Random.Range(-moveRange, moveRange);
            float randomY = startPosition.y + Random.Range(-moveRange * 0.5f, moveRange * 0.5f);

            targetPosition = new Vector3(randomX, randomY, transform.position.z);
        }

        // Se déplacer vers la position cible
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// Change le type de mouvement (utile pour les séquences du Stage 3)
    /// </summary>
    public void SetMovementType(MovementType newType)
    {
        movementType = newType;
        startPosition = transform.position;
        Debug.Log($"🎯 Type de mouvement changé : {movementType}");
    }

    /// <summary>
    /// Change la vitesse de déplacement
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    // Propriétés publiques
    public MovementType CurrentMovementType => movementType;
}
