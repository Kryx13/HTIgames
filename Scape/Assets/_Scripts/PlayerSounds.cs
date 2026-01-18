using UnityEngine;
using System.Collections.Generic; 

public class PlayerSounds : MonoBehaviour
{
    [Header("Sons")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> stepSounds; 

    public void PlayStep()
    {
        if (stepSounds.Count > 0)
        {
            int index = Random.Range(0, stepSounds.Count);
            
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = Random.Range(0.8f, 1.0f);

            audioSource.PlayOneShot(stepSounds[index]);
        }
    }
}