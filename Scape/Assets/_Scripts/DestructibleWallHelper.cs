using UnityEngine;

/// <summary>
/// Helper script pour créer facilement un mur destructible de test.
/// Attachez ce script à un GameObject Cube dans la scène pour le rendre destructible.
/// </summary>
[RequireComponent(typeof(DestructibleWall))]
public class DestructibleWallHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private Color wallColor = new Color(0.6f, 0.4f, 0.3f); // Brun/terre

    private void Start()
    {
        if (autoSetup)
        {
            SetupWall();
        }
    }

    private void SetupWall()
    {
        // S'assurer qu'il y a un Renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = wallColor;
        }

        // S'assurer qu'il y a un Collider (pas en trigger)
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = false;

        // Vérifier que le DestructibleWall est présent
        DestructibleWall wall = GetComponent<DestructibleWall>();
        if (wall != null)
        {
            Debug.Log($"✅ Mur destructible configuré: {gameObject.name}");
        }
    }

    // Dessiner un Gizmo pour voir le mur dans l'éditeur
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(wallColor.r, wallColor.g, wallColor.b, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
