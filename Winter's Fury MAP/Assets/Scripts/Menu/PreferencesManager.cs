using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreferencesManager : MonoBehaviour
{
    public static PreferencesManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void SavePreferences()
    {
        GeneralScript.Instance.SavePreferences();
        VideoScript.Instance.SavePreferences();
        AudioScript.Instance.SavePreferences();
        ControlsScript.Instance.SavePreferences();
    }
}
