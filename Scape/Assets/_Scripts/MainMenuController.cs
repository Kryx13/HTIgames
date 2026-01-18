using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Nécessaire pour modifier le texte

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bestTimeText;

    private void Start()
    {
        // S'assurer que le curseur est visible et déverrouillé dans le menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f; // S'assurer que le jeu n'est pas en pause

        // 1. On vérifie si un score existe (La clé s'appelle "BestTime")
        if (PlayerPrefs.HasKey("BestTime"))
        {
            float time = PlayerPrefs.GetFloat("BestTime");
            bestTimeText.text = "Best Time: " + FormatTime(time);
        }
        else
        {
            bestTimeText.text = "Best Time: --:--";
        }

        Debug.Log("🎮 Main Menu chargé - Curseur déverrouillé");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("🚪 Fermeture du jeu...");

        // Ferme l'application (fonctionne uniquement une fois le jeu compilé/buildé)
        Application.Quit();

        // Si on est dans l'éditeur Unity, on arrête le mode Play
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Petite fonction utilitaire pour formater le temps (copié du GameManager)
    private string FormatTime(float timeInSeconds)
    {
        int m = Mathf.FloorToInt(timeInSeconds / 60F);
        int s = Mathf.FloorToInt(timeInSeconds % 60F);
        int ms = Mathf.FloorToInt((timeInSeconds * 100F) % 100F);
        return string.Format("{0:00}:{1:00}.{2:00}", m, s, ms);
    }
}