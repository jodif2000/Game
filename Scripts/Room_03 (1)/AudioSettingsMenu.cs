using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [Header("Аудио миксер")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Ползунок музыки")]
    [SerializeField] private Slider musicSlider;

    private const string MusicVolumeParameter = "MusicVolume";

    private void Start()
    {
        if (musicSlider != null)
        {
            SetMusicVolume(musicSlider.value);
        }
    }

    public void SetMusicVolume(float value)
    {
        if (audioMixer == null) return;

        if (value <= 0.001f)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
            return;
        }

        float correctedValue = Mathf.Pow(value, 2f);
        float volume = Mathf.Log10(correctedValue) * 20f;

        audioMixer.SetFloat("MusicVolume", volume);
    }
}