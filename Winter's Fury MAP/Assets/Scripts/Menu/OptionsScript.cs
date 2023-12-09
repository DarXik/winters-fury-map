using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    public void Start()
    {
        GetResolutions();
        QualitySwitcher();
        SetFPS(fpsPreference);
        SetBrightness(brigtnessPreference);
        SetMainVolume(mainVolumePreference);
        sliderFPS.value = fpsPreference;
        sliderBrightness.value = brigtnessPreference;
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetInt("qualityPreference", currentQualityIndex);
        PlayerPrefs.SetInt("fpsPreference", fpsPreference);
        PlayerPrefs.SetFloat("mainVolumePreference", mainVolumePreference);
        PlayerPrefs.SetFloat("brightnessPreference", brigtnessPreference);
        Debug.Log("Uloženo");
    }

    public void LoadPreferences()
    {
        currentQualityIndex = PlayerPrefs.GetInt("qualityPreference");
        fpsPreference = PlayerPrefs.GetInt("fpsPreference");
        mainVolumePreference = PlayerPrefs.GetFloat("mainVolumePreference");
        brigtnessPreference = PlayerPrefs.GetFloat("brightnessPreference");
    }

    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MyMainVolume", volume);
        mainVolumePreference = volume;
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetFPS(float fps)
    {
        sliderTextFPS.text = fps.ToString("0");
        fpsPreference = Convert.ToInt32(fps);
        if (Convert.ToInt32(fps) == 241)
        {
            fpsPreference = -1;
            sliderTextFPS.text = "Unlimited";
        }

        Application.targetFrameRate = fpsPreference;
    }

    public void SetBrightness(float lumen)
    {
        sliderTextBrigtness.text = lumen.ToString("0.00");
        brigtnessPreference = lumen;
    }

    private void QualitySwitcher()
    {
        if (clickedRight)
        {
            currentQualityIndex += 1;
        }
        else if (clickedLeft)
        {
            currentQualityIndex -= 1;
        }
        else
        {
            currentQualityIndex = 1;
        }

        QualitySettings.SetQualityLevel(currentQualityIndex);
        qualityOptionsText.text = QualitySettings.names[currentQualityIndex];
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
            string option = resolutions[i].width + "x" + resolutions[i].height + "@" + resolutions[i].refreshRateRatio;
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
    }

    [Header("Video")]
    public TextMeshProUGUI sliderTextBrigtness;
    public Slider sliderBrightness;
    public TextMeshProUGUI sliderTextFPS;
    public Slider sliderFPS;
    public TextMeshProUGUI qualityOptionsText;
    private bool clickedRight;
    private bool clickedLeft;
    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    [Header("Pro ukládání")]
    public int fpsPreference = 60;
    public int currentQualityIndex = 1;
    public float mainVolumePreference;
    public float brigtnessPreference = -0.46f;

    [Header("Audio")]
    public AudioMixer audioMixer;
}