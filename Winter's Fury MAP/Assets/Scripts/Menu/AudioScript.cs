using UnityEngine;
using UnityEngine.Audio;

public class AudioScript : MonoBehaviour
{
    [Header("Zvuk")] public AudioMixer audioMixer;
    private float mainVolumePreference;

    public void Awake()
    {
        mainVolumePreference = PlayerPrefs.HasKey("mainVolumePreference") ? PlayerPrefs.GetFloat("mainVolumePreference") : -10f;
    }

    public void Start()
    {
        SetMainVolume(mainVolumePreference);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("mainVolumePreference", mainVolumePreference);
    }

    public void SetMainVolume(float volume)
    {
        mainVolumePreference = volume;
        audioMixer.SetFloat("MyMainVolume", mainVolumePreference);
        Debug.Log("Vol: " + mainVolumePreference);
    }
}