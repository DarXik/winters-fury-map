using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralScript : MonoBehaviour
{
    private bool headBobbingPreference;
    public Toggle toggleHeadBobbing;
    private bool autosavePreference;
    public Toggle toggleAutosave;
    public Toggle toggleShowFPS;
    public bool showFPSpreference = true;
    
    public static GeneralScript Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        LoadPreferences();
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetInt("headBobbingPreference", headBobbingPreference ? 1 : 0);
        PlayerPrefs.SetInt("autosavePreference", autosavePreference ? 1 : 0);
        PlayerPrefs.SetInt("showFPSpreference", showFPSpreference ? 1 : 0);
    }

    public void LoadPreferences()
    {
        headBobbingPreference = PlayerPrefs.HasKey("headBobbingPreference") ? PlayerPrefs.GetInt("headBobbingPreference") == 1 : PlayerPrefs.GetInt("headBobbingPreference") == 0;
        autosavePreference = PlayerPrefs.HasKey("autosavePreference") ? PlayerPrefs.GetInt("autosavePreference") == 1 : PlayerPrefs.GetInt("autosavePreference") == 0;
        showFPSpreference = PlayerPrefs.HasKey("showFPSpreference") ? PlayerPrefs.GetInt("showFPSpreference") == 1 : PlayerPrefs.GetInt("showFPSpreference") == 0;
        SetAutosave(autosavePreference);
        SetHeadBobbing(headBobbingPreference);
        SetShowFPS(showFPSpreference);
    }

    public void SetHeadBobbing(bool headBobbing)
    {
        headBobbingPreference = headBobbing;
        toggleHeadBobbing.isOn = headBobbingPreference;
    }

    public void SetAutosave(bool autosave)
    {
        autosavePreference = autosave;
        toggleAutosave.isOn = autosavePreference;
    }

    public void SetShowFPS(bool fps)
    {
        showFPSpreference = fps;
        toggleShowFPS.isOn = showFPSpreference;
    }
}
