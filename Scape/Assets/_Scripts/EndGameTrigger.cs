using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Si c'est le joueur qui touche le cube
        if (other.CompareTag("Player"))
        {
            // On appelle la fin du jeu dans le GameManager
            GameManager.Instance.LevelComplete();
        }
    }
}