using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Debug visuel pour voir exactement ce que fait le raycast
/// </summary>
public class InteractionDebugVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rayDistance = 4f;
    [SerializeField] private bool showDebugSphere = true;
    [SerializeField] private float sphereRadius = 0.5f;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("❌ Pas de Main Camera !");
        }
        else
        {
            Debug.Log($"✅ Main Camera trouvée : {mainCam.name}");
            Debug.Log($"   Position : {mainCam.transform.position}");
            Debug.Log($"   Rotation : {mainCam.transform.eulerAngles}");
        }
    }

    private void Update()
    {
        if (mainCam == null) return;

        // Test continu du raycast
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Test 1 : Raycast sans filtre (AVEC détection des triggers)
        bool hitSomething = Physics.Raycast(ray, out hit, rayDistance, ~0, QueryTriggerInteraction.Collide);

        // Test 2 : Raycast avec LayerMask Layer 6 (AVEC détection des triggers)
        LayerMask mask = 1 << 6;
        bool hitInteractable = Physics.Raycast(ray, out hit, rayDistance, mask, QueryTriggerInteraction.Collide);

        // Dessiner le rayon EN PERMANENCE dans la Scene View
        Debug.DrawRay(ray.origin, ray.direction * rayDistance,
            hitInteractable ? Color.green : (hitSomething ? Color.yellow : Color.red));

        // Afficher les infos quand on appuie sur T
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log("=== DEBUG RAYCAST (Touche T) ===");
            Debug.Log($"Ray Origin: {ray.origin}");
            Debug.Log($"Ray Direction: {ray.direction}");
            Debug.Log($"Distance: {rayDistance}m");

            if (hitSomething)
            {
                Debug.Log($"✅ HIT (sans filtre): {hit.collider.name}");
                Debug.Log($"   Distance: {hit.distance}m");
                Debug.Log($"   Layer: {hit.collider.gameObject.layer} ({LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                Debug.Log($"   Position: {hit.point}");
            }
            else
            {
                Debug.LogWarning("❌ AUCUN HIT (sans filtre)");
            }

            if (hitInteractable)
            {
                Debug.Log($"✅ HIT Interactable (Layer 6): {hit.collider.name}");
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    Debug.Log($"   → IInteractable trouvé: {interactable.InteractionPrompt}");
                }
            }
            else
            {
                Debug.LogWarning("❌ AUCUN HIT sur Layer Interactable (6)");
            }

            Debug.Log("=================================");
        }
    }

    private void OnDrawGizmos()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null) return;
        }

        // Dessiner le rayon depuis la caméra
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        LayerMask mask = 1 << 6;
        bool hitInteractable = Physics.Raycast(ray, out hit, rayDistance, mask, QueryTriggerInteraction.Collide);

        // Rayon
        Gizmos.color = hitInteractable ? Color.green : Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * rayDistance);

        // Sphère à l'origine du rayon
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ray.origin, 0.1f);

        // Si on touche quelque chose, dessiner une sphère au point d'impact
        if (hitInteractable)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hit.point, 0.2f);

            // Ligne vers l'objet touché
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ray.origin, hit.point);
        }

        // Dessiner une sphère de debug autour de chaque pickup
        if (showDebugSphere)
        {
            ItemPickup[] pickups = FindObjectsOfType<ItemPickup>();
            foreach (var pickup in pickups)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(pickup.transform.position, sphereRadius);
            }
        }
    }
}
