using UnityEngine;

/// <summary>
/// Script de diagnostic pour vérifier que le système EndGame est bien configuré.
/// Attachez ce script à n'importe quel objet dans la scène et lancez le jeu.
/// Il affichera un rapport dans la console.
/// </summary>
public class EndGameDiagnostic : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("========================================");
        Debug.Log("🔍 DIAGNOSTIC SYSTÈME END GAME");
        Debug.Log("========================================");

        // 1. Vérifier GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance est NULL !");
            Debug.LogError("   → Assurez-vous qu'un GameObject avec le script GameManager existe dans la scène.");
        }
        else
        {
            Debug.Log("✅ GameManager trouvé");

            // Vérifier EndGameUI dans GameManager
            GameManager gm = GameManager.Instance;

            // Utiliser la réflexion pour accéder au champ privé
            var endGameUIField = gm.GetType().GetField("endGameUI",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (endGameUIField != null)
            {
                var endGameUI = endGameUIField.GetValue(gm);

                if (endGameUI == null)
                {
                    Debug.LogError("❌ EndGameUI n'est PAS assigné dans le GameManager !");
                    Debug.LogError("   → Sélectionnez le GameManager dans la Hierarchy");
                    Debug.LogError("   → Dans l'Inspector, assignez le EndGamePanel (avec script EndGameUI) dans le champ 'End Game UI'");
                }
                else
                {
                    Debug.Log("✅ EndGameUI assigné dans GameManager");

                    EndGameUI script = endGameUI as EndGameUI;
                    if (script != null)
                    {
                        Debug.Log($"   → Script EndGameUI trouvé sur: {script.gameObject.name}");
                    }
                }
            }
        }

        // 2. Chercher EndGameUI dans la scène
        EndGameUI[] endGameUIs = FindObjectsByType<EndGameUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        Debug.Log($"\n📊 {endGameUIs.Length} EndGameUI trouvé(s) dans la scène:");

        if (endGameUIs.Length == 0)
        {
            Debug.LogError("❌ Aucun EndGameUI trouvé dans la scène !");
            Debug.LogError("   → Créez un Panel 'EndGamePanel'");
            Debug.LogError("   → Ajoutez-lui le script EndGameUI");
            Debug.LogError("   → Assignez-le dans le GameManager");
        }
        else
        {
            foreach (var ui in endGameUIs)
            {
                Debug.Log($"   → {ui.gameObject.name} (Actif: {ui.gameObject.activeInHierarchy})");

                if (ui.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning("⚠️ EndGamePanel est ACTIVÉ au démarrage ! Il devrait être désactivé.");
                }
            }
        }

        // 3. Chercher EndGameTrigger
        EndGameTrigger[] triggers = FindObjectsByType<EndGameTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        Debug.Log($"\n🎯 {triggers.Length} EndGameTrigger trouvé(s) dans la scène:");

        if (triggers.Length == 0)
        {
            Debug.LogWarning("⚠️ Aucun EndGameTrigger trouvé !");
            Debug.LogWarning("   → Créez une WinZone avec le script EndGameTrigger");
        }
        else
        {
            foreach (var trigger in triggers)
            {
                Debug.Log($"   → {trigger.gameObject.name}");

                Collider col = trigger.GetComponent<Collider>();
                if (col != null)
                {
                    if (!col.isTrigger)
                    {
                        Debug.LogError($"   ❌ {trigger.gameObject.name} : Le Collider n'est PAS en mode Trigger !");
                    }
                    else
                    {
                        Debug.Log($"   ✅ Collider en mode Trigger");
                    }
                }
            }
        }

        // 4. Chercher le Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("❌ Aucun GameObject avec le tag 'Player' trouvé !");
            Debug.LogError("   → Sélectionnez votre Player dans la Hierarchy");
            Debug.LogError("   → Dans l'Inspector, en haut, changez le Tag en 'Player'");
        }
        else
        {
            Debug.Log($"\n🎮 Player trouvé: {player.name}");
        }

        Debug.Log("\n========================================");
        Debug.Log("FIN DU DIAGNOSTIC");
        Debug.Log("========================================\n");

        // Désactiver ce script après l'exécution
        enabled = false;
    }
}
