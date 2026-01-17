using UnityEngine;

/// <summary>
/// Configure automatiquement le joueur pour pousser des objets.
/// Ajoute et configure le composant BasicRigidBodyPush.
/// </summary>
public class PlayerPushSetup : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private bool enablePush = true;
    [SerializeField] private float pushStrength = 2f; // Force de poussée
    [SerializeField] private LayerMask pushableLayers = ~0; // Layers qu'on peut pousser (tout par défaut)

    private BasicRigidBodyPush pushScript;

    private void Start()
    {
        SetupPushSystem();
    }

    [ContextMenu("Setup Push System")]
    public void SetupPushSystem()
    {
        // Vérifier qu'il y a un CharacterController
        CharacterController controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("❌ Le joueur n'a pas de CharacterController ! Le système de poussée nécessite un CharacterController.");
            return;
        }

        // Récupérer ou ajouter BasicRigidBodyPush
        pushScript = GetComponent<BasicRigidBodyPush>();
        if (pushScript == null)
        {
            pushScript = gameObject.AddComponent<BasicRigidBodyPush>();
            Debug.Log("✅ BasicRigidBodyPush ajouté au joueur");
        }

        // Configurer le script de poussée
        pushScript.canPush = enablePush;
        pushScript.strength = pushStrength;
        pushScript.pushLayers = pushableLayers;

        Debug.Log($"✅ Système de poussée configuré (Force: {pushStrength}, Activé: {enablePush})");
    }

    /// <summary>
    /// Active ou désactive la capacité de pousser
    /// </summary>
    public void SetPushEnabled(bool enabled)
    {
        enablePush = enabled;
        if (pushScript != null)
        {
            pushScript.canPush = enabled;
        }
    }

    /// <summary>
    /// Change la force de poussée
    /// </summary>
    public void SetPushStrength(float strength)
    {
        pushStrength = Mathf.Clamp(strength, 0.5f, 5f);
        if (pushScript != null)
        {
            pushScript.strength = pushStrength;
        }
    }

    private void OnValidate()
    {
        // Appliquer les changements en temps réel
        if (pushScript != null && Application.isPlaying)
        {
            pushScript.canPush = enablePush;
            pushScript.strength = Mathf.Clamp(pushStrength, 0.5f, 5f);
            pushScript.pushLayers = pushableLayers;
        }
    }

    // Propriétés publiques
    public bool CanPush => enablePush;
    public float PushStrength => pushStrength;
}
