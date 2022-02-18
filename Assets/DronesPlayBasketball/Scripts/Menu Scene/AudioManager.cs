using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;

    public AudioSource[] musicAudioSources;
    public AudioSource[] soundAudioSources;

    void Start()
    {
        if (PlayerPrefs.HasKey(ConstantData.musicPlayerPrefs))
        {
            GetMusicVolume();
        }
        else
        {
            SetMusicVolume(1f);
        }
        if (PlayerPrefs.HasKey(ConstantData.soundPlayerPrefs))
        {
            GetSoundVolume();
        }
        else
        {
            SetSoundVolume(1f);
        }
    }

    public void SetMusicVolume(float _volume)
    {
        foreach (var musicSource in musicAudioSources)
        {
            musicSource.volume = _volume;
        }

        PlayerPrefs.SetFloat(ConstantData.musicPlayerPrefs, _volume);
        PlayerPrefs.Save();
    }

    public void SetSoundVolume(float _volume)
    {
        foreach (var soundSource in soundAudioSources)
        {
            soundSource.volume = _volume;
        }

        PlayerPrefs.SetFloat(ConstantData.soundPlayerPrefs, _volume);
        PlayerPrefs.Save();
    }

    public void GetMusicVolume()
    {
        float _volume = PlayerPrefs.GetFloat(ConstantData.musicPlayerPrefs);

        foreach (var musicSource in musicAudioSources)
        {
            musicSource.volume = _volume;
        }

        if (musicSlider)
            musicSlider.value = _volume;
    }

    public void GetSoundVolume()
    {
        float _volume = PlayerPrefs.GetFloat(ConstantData.soundPlayerPrefs);

        foreach (var soundSource in soundAudioSources)
        {
            soundSource.volume = _volume;
        }

        if (soundSlider)
            soundSlider.value = _volume;
    }
}