using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{

    public void Start()
    {
        // sliderTextFPS.gameObject.SetActive(false);

        LoadPreferences();
        GetResolutions();
        QualitySwitcher();
        SetFPS(fpsPreference);
        SetBrightness(brigtnessPreference);
        SetMainVolume(mainVolumePreference);
        SetFullScreen(fullscreenPreference);
    }

    // public void Update()
    // {
    //     sliderTextFPS.gameObject.SetActive(sliderHandleEvent);
    // }

    public void SavePreferences()
    {
        PlayerPrefs.SetInt("qualityPreference", currentQualityIndex);
        PlayerPrefs.SetInt("fpsPreference", fpsPreference);
        PlayerPrefs.SetFloat("mainVolumePreference", mainVolumePreference);
        PlayerPrefs.SetFloat("brightnessPreference", brigtnessPreference);
        PlayerPrefs.SetInt("fullscreenPreference", fullscreenPreference ? 1 : 0);
        Debug.Log("Uloženo");
    }

    public void LoadPreferences()
    {
        currentQualityIndex = PlayerPrefs.HasKey("qualityPreference") ? PlayerPrefs.GetInt("qualityPreference") : 1;
        fpsPreference = PlayerPrefs.HasKey("fpsPreference") ? PlayerPrefs.GetInt("fpsPreference") : 60;
        mainVolumePreference = PlayerPrefs.HasKey("mainVolumePreference") ? PlayerPrefs.GetFloat("mainVolumePreference") : -10f;
        brigtnessPreference = PlayerPrefs.HasKey("brightnessPreference") ? PlayerPrefs.GetFloat("brightnessPreference") : -0.5f;
        fullscreenPreference = PlayerPrefs.HasKey("fullscreenPreference") ? PlayerPrefs.GetInt("fullscreenPreference") == 1 : PlayerPrefs.GetInt("fullscreenPreference") == 0;
        Debug.Log("Načteno");
    }

    public void DefaultOptions()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SetMainVolume(float volume)
    {
        mainVolumePreference = volume;
        audioMixer.SetFloat("MyMainVolume", mainVolumePreference);
        Debug.Log("Vol: " + mainVolumePreference);
    }

    public void SetFullScreen(bool isFullscreen) // nefunguje ukazování value v togglu
    {
        fullscreenPreference = isFullscreen;
        Screen.fullScreen = isFullscreen;
        toggleFullscreen.isOn = fullscreenPreference;
    }

    public void SetFPS(float fps)
    {
        sliderTextFPS.text = fps.ToString("0");
        fpsPreference = Convert.ToInt32(fps);

        sliderFPS.value = fpsPreference;
        if (Convert.ToInt32(fps) == 241)
        {
            fpsPreference = -1;
            sliderTextFPS.text = "Unlimited";
        }

        Application.targetFrameRate = fpsPreference;
        Debug.Log("FPS " + fpsPreference);
    }

    public void SetBrightness(float lumen)
    {
        brigtnessPreference = lumen;
        sliderTextBrigtness.text = brigtnessPreference.ToString("0.00");
        sliderBrightness.value = brigtnessPreference;
        Debug.Log("Brig. " + brigtnessPreference);
    }

    private void QualitySwitcher()
    {
        if (clickedRight && currentQualityIndex != 2)
        {
            currentQualityIndex += 1;
        }
        else if (clickedLeft && currentQualityIndex != 0)
        {
            currentQualityIndex -= 1;
        }

        QualitySettings.SetQualityLevel(currentQualityIndex);
        qualityOptionsText.text = QualitySettings.names[currentQualityIndex];
        Debug.Log("Qua. " + currentQualityIndex);
    }

    public void RightClick()
    {
        clickedRight = true;
        clickedLeft = false;
        QualitySwitcher();
    }

    public void LeftClick()
    {
        clickedLeft = true;
        clickedRight = false;
        QualitySwitcher();
    }

    private void GetResolutions()
    {
        // vezme dostupná rozlišení, pro každý pc jiné
        resolutions = Screen.resolutions;
        // rates = Screen.
        resolutionDropdown.ClearOptions();

        // list pro dostupná rozlišení
        var availableResolutions = new List<string>();

        int currResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // projede každým rozlišením a uloží zformátované ve stringu
            string option = resolutions[i].width + "x" + resolutions[i].height;
            availableResolutions.Add(option);

            // if (resolutions[i].width == Screen.currentResolution.width &&
            //     resolutions[i].height == Screen.currentResolution.height &&
            //     resolutions[i].refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            if (resolutions[i].Equals(Screen.currentResolution))
            {
                currResIndex = i;
            }
        }

        // vezme zformátovaná rozlišení a dá je do dropdownu
        resolutionDropdown.AddOptions(availableResolutions);
        // nastaví rozlišení na dropdown na naše aktuální
        resolutionDropdown.value = currResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void UpdateResolution(int resolutionIndex) // v unity pro update, když uživatel změní, tak unity předá info a index - v dropdownu
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("Res. updated");
    }

    [Header("Brightness")]
    public TextMeshProUGUI sliderTextBrigtness;
    public Slider sliderBrightness;
    [Header("FPS")]
    public TextMeshProUGUI sliderTextFPS;
    public Slider sliderFPS;
    [Header("Kvalita")]
    public TextMeshProUGUI qualityOptionsText;
    private bool clickedRight;
    private bool clickedLeft;
    [Header("Rozlišení")]
    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;
    [Header("Fullscreen")]
    public Toggle toggleFullscreen;

    [Header("Pro ukládání")]
    private int fpsPreference;
    private int currentQualityIndex;
    private float mainVolumePreference;
    private float brigtnessPreference;
    private bool fullscreenPreference;

    [Header("Zvuk")]
    public AudioMixer audioMixer;
}