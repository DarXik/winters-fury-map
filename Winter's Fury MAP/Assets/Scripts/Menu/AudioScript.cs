using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    [Header("Zvuk")]
    public AudioMixer audioMixer;
    public TextMeshProUGUI sliderTextAudio;
    public Slider sliderAudio;
    private float mainVolumePreference;

    public static AudioScript Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
        LoadPreferences();
    }

    public void LoadPreferences()
    {
        mainVolumePreference = PlayerPrefs.HasKey("mainVolumePreference") ? PlayerPrefs.GetFloat("mainVolumePreference") : -10f;
        SetMainVolume(mainVolumePreference);
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("mainVolumePreference", mainVolumePreference);
        Debug.Log("Zvuk uložen");
    }

    public void SetMainVolume(float volume)
    {
        mainVolumePreference = volume;
        double numToBeShown = 1.25 * mainVolumePreference + 100;
        sliderTextAudio.text = numToBeShown.ToString("0") + "%";
        sliderAudio.value = mainVolumePreference;
    }
}