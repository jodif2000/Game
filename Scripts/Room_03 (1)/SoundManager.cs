using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    Stone_PickUp,
    Stone_Throw,
    Stone_Fall,
    Lever_Activated,
    Grate_Open
}

[System.Serializable]
public class Sound
{
    public SoundType soundType;
    public AudioClip audioClip;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    [SerializeField] private AudioSource audioSource;

    private static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void PlaySound(SoundType soundType, float volume = 1f)
    {
        if (instance == null) return;
        if (instance.audioSource == null) return;

        foreach (Sound sound in instance.sounds)
        {
            if (sound.soundType == soundType)
            {
                instance.audioSource.PlayOneShot(sound.audioClip, volume);
                return;
            }
        }
    }
}