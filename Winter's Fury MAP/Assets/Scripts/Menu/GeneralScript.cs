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
        Debug.Log("General uloženo");
    }

    public void LoadPreferences()
    {
        headBobbingPreference = PlayerPrefs.HasKey("headBobbingPreference") ? PlayerPrefs.GetInt("headBobbingPreference") == 1 : PlayerPrefs.GetInt("headBobbingPreference") == 0;
        autosavePreference = PlayerPrefs.HasKey("autosavePreference") ? PlayerPrefs.GetInt("autosavePreference") == 1 : PlayerPrefs.GetInt("autosavePreference") == 0;
        SetAutosave(autosavePreference);
        SetHeadBobbing(headBobbingPreference);
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
}
