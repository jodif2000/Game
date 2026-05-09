using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic;

    private void Start()
    {
        if (audioSource == null) return;
        if (backgroundMusic == null) return;

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}