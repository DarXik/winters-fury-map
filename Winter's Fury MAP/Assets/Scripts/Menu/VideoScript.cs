using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class VideoScript : MonoBehaviour
{
    public static VideoScript Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        LoadPreferences();
        GetResolutions();
        QualitySwitcher();
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetInt("qualityPreference", currentQualityIndex);
        PlayerPrefs.SetInt("fpsPreference", fpsPreference);
        PlayerPrefs.SetFloat("brightnessPreference", brightnessPreference);
        PlayerPrefs.SetInt("fullscreenPreference", fullscreenPreference ? 1 : 0);
        PlayerPrefs.SetInt("vsyncPreference", vsyncPreference ? 1 : 0);
    }

    public void LoadPreferences()
    {
        currentQualityIndex = PlayerPrefs.HasKey("qualityPreference") ? PlayerPrefs.GetInt("qualityPreference") : 0;
        fpsPreference = PlayerPrefs.HasKey("fpsPreference") ? PlayerPrefs.GetInt("fpsPreference") : 60;
        brightnessPreference = PlayerPrefs.HasKey("brightnessPreference") ? PlayerPrefs.GetFloat("brightnessPreference") : -0.5f;
        fullscreenPreference = PlayerPrefs.HasKey("fullscreenPreference") ? PlayerPrefs.GetInt("fullscreenPreference") == 1 : PlayerPrefs.GetInt("fullscreenPreference") == 0;
        vsyncPreference = PlayerPrefs.HasKey("vsyncPreference") ? PlayerPrefs.GetInt("vsyncPreference") == 1 : PlayerPrefs.GetInt("vsyncPreference") == 0;
        SetFPS(fpsPreference);
        SetBrightness(brightnessPreference);
        SetFullScreen(fullscreenPreference);
        SetVsync(vsyncPreference);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        fullscreenPreference = isFullscreen;
        Screen.fullScreen = isFullscreen;
        toggleFullscreen.isOn = fullscreenPreference;
    }

    public void SetVsync(bool vsync)
    {
        vsyncPreference = vsync;
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        toggleVsync.isOn = vsyncPreference;
    }

    public void SetFPS(float fps)
    {
        sliderTextFPS.text = fps.ToString("0");
        fpsPreference = (int) fps;

        sliderFPS.value = fpsPreference >= 0 ? fpsPreference : 241;

        if (fps is 241 or -1)
        {
            fpsPreference = -1;
            sliderTextFPS.text = "Unlimited";
        }

        Application.targetFrameRate = fpsPreference;
    }

    public void SetBrightness(float lumen)
    {
        brightnessPreference = lumen;
        double numToBeShown = 33.333333 * brightnessPreference + (33.3333333 / 2) + 50;
        sliderTextBrigtness.text = numToBeShown.ToString("0") + "%";
        sliderBrightness.value = brightnessPreference;
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
    }

    public void RightClick()
    {
        clickedRight = true;
        clickedLeft = false;
        QualitySwitcher();
        switch (currentQualityIndex)
        {
            case 2:
                btnRight.interactable = false;
                btnLeft.interactable = true;
                break;
            case 1:
                btnRight.interactable = true;
                btnLeft.interactable = true;
                break;
            case 0:
                btnRight.interactable = true;
                btnLeft.interactable = false;
                break;
        }
    }

    public void LeftClick()
    {
        clickedLeft = true;
        clickedRight = false;
        QualitySwitcher();
        switch (currentQualityIndex)
        {
            case 2:
                btnRight.interactable = false;
                btnLeft.interactable = true;
                break;
            case 1:
                btnRight.interactable = true;
                btnLeft.interactable = true;
                break;
            case 0:
                btnRight.interactable = true;
                btnLeft.interactable = false;
                break;
        }
    }

    private void GetResolutions()
    {
        // vezme dostupná rozlišení, pro každý pc jiné
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // list pro dostupná rozlišení
        var availableResolutions = new List<string>();
        Array.Reverse(resolutions);

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
    }

    [Header("Brightness")] public TextMeshProUGUI sliderTextBrigtness;
    public Slider sliderBrightness;

    [Header("FPS")] public TextMeshProUGUI sliderTextFPS;
    public Slider sliderFPS;

    [Header("Kvalita")] public TextMeshProUGUI qualityOptionsText;
    private bool clickedRight;
    private bool clickedLeft;
    public Button btnRight;
    public Button btnLeft;

    [Header("Rozlišení")] private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    [Header("Fullscreen")] public Toggle toggleFullscreen;
    [Header("V-sync")] public Toggle toggleVsync;

    [Header("Pro ukládání")] private int fpsPreference;
    private int currentQualityIndex;
    private float brightnessPreference;
    private bool fullscreenPreference;
    private bool vsyncPreference;
}