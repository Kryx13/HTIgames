using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private bool autoStart = true; // For testing, we start immediately

    [SerializeField] private GameObject pauseMenuObject;

    // State
    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object alive when changing scenes
        }
        else
        {
            Destroy(gameObject); // Destroys duplicates if we reload the scene
        }
    }

    private void Start()
    {
        if (autoStart)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (IsRunning)
        {
            // Add the time passed since last frame
            CurrentTime += Time.deltaTime;
            
            // Update the screen
            UpdateTimerUI();
        }
    }

    public void StartTimer()
    {
        IsRunning = true;
        CurrentTime = 0f;
    }

    public void StopTimer()
    {
        IsRunning = false;
        // Later: This is where we will check for High Scores
        Debug.Log($"🏁 Run Finished! Final Time: {FormatTime(CurrentTime)}");
    }

    private void UpdateTimerUI()
    {
        timerText.text = FormatTime(CurrentTime);
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            // Pause the Game
            Time.timeScale = 0f; // Freezes physics and time
            pauseMenuObject.SetActive(true); // Show UI
            Cursor.lockState = CursorLockMode.None; // Unlock mouse
            Cursor.visible = true;
        }
        else
        {
            // Resume the Game
            Time.timeScale = 1f; // Normal speed
            pauseMenuObject.SetActive(false); // Hide UI
            Cursor.lockState = CursorLockMode.Locked; // Lock mouse back
            Cursor.visible = false;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // A utility to make the time look like 00:00.00
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100F) % 100F);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    public void LevelComplete()
    {
        IsRunning = false; // Arrête le timer

        // 1. Récupérer l'ancien record (S'il n'y en a pas, on met un temps infini)
        float currentBest = PlayerPrefs.GetFloat("BestTime", float.MaxValue);

        // 2. Si on a battu le record
        if (CurrentTime < currentBest)
        {
            PlayerPrefs.SetFloat("BestTime", CurrentTime);
            PlayerPrefs.Save(); // Force l'écriture sur le disque
            Debug.Log("Nouveau Record Sauvegardé !");
        }

        // 3. Retour au Menu Principal après 2 secondes (pour voir son temps)
        Invoke("LoadMenu", 2f);
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(0); // 0 est l'index du Menu Principal
    }
}