using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static VideoScript Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadPreferences();
        GetResolutions();
        QualitySwitcher();
        SetFPS(fpsPreference);
        SetBrightness(brightnessPreference);
        SetFullScreen(fullscreenPreference);
    }

    private void Update()
    {
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

    public void SavePreferences()
    {
        PlayerPrefs.SetInt("qualityPreference", currentQualityIndex);
        PlayerPrefs.SetInt("fpsPreference", fpsPreference);
        PlayerPrefs.SetFloat("brightnessPreference", brightnessPreference);
        PlayerPrefs.SetInt("fullscreenPreference", fullscreenPreference ? 1 : 0);
        Debug.Log("Uloženo");
    }

    private void LoadPreferences()
    {
        currentQualityIndex = PlayerPrefs.HasKey("qualityPreference") ? PlayerPrefs.GetInt("qualityPreference") : 1;
        fpsPreference = PlayerPrefs.HasKey("fpsPreference") ? PlayerPrefs.GetInt("fpsPreference") : 60;
        brightnessPreference = PlayerPrefs.HasKey("brightnessPreference") ? PlayerPrefs.GetFloat("brightnessPreference") : -0.5f;
        fullscreenPreference = PlayerPrefs.HasKey("fullscreenPreference") ? PlayerPrefs.GetInt("fullscreenPreference") == 1 : PlayerPrefs.GetInt("fullscreenPreference") == 0;
        Debug.Log("Načteno");
    }

    public void SetFullScreen(bool isFullscreen)
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
        brightnessPreference = lumen;
        double numToBeShown = 33.333333 * brightnessPreference + (33.3333333 / 2) + 50;
        sliderTextBrigtness.text = numToBeShown.ToString("0") + "%";
        sliderBrightness.value = brightnessPreference;
        Debug.Log("Brig. " + brightnessPreference);

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

    [Header("Pro ukládání")] private int fpsPreference;
    private int currentQualityIndex;
    private float brightnessPreference;
    private bool fullscreenPreference;
}
