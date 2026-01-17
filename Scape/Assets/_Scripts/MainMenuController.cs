using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Nécessaire pour modifier le texte

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bestTimeText;

    private void Start()
    {
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
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
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