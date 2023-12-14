using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScript : MonoBehaviour
{
    private Dictionary<string, KeyCode> keyCodes = new Dictionary<string, KeyCode>();
    private KeyCode inventoryKeyPreference;

    public Button inventoryKeyButton;
    public TMP_Text inventoryKeyText;
    public Button craftingKeyButton;
    public TMP_Text craftingKeyText;

    public void SavePreferences()
    {
        // PlayerPrefs.SetInt("qualityPreference", currentQualityIndex);
        PlayerPrefs.SetString("inventoryKey", keyCodes["inventoryKey"].ToString());
        Debug.Log("Keys uloženo");
    }

    public void LoadPreferences()
    {
        // currentQualityIndex = PlayerPrefs.HasKey("qualityPreference") ? PlayerPrefs.GetInt("qualityPreference") : 1;
        // keyCodes["inventoryKey"] = PlayerPrefs.HasKey("inventoryKey") ? PlayerPrefs.GetString(Enum.TryParse("inventoryKey", out KeyCode )) : KeyCode.Tab;

        // if (PlayerPrefs.HasKey("inventoryKey"))
        // {
        //     inventoryKeyPreference = PlayerPrefs.GetString("inventoryKey");
        //     if (Enum.TryParse(inventoryKeyPreference, out KeyCode keycode))
        //     {
        //         keyCodes["inventoryKey"] = keycode;
        //     }
        // }
        // else
        // {
        //     inventoryKeyPreference = "Tab";
        // }
    }

    private bool keyPressed;
    public void InventoryKeyHandler()
    {
        keyPressed = true;
        // inventoryKeyPreference = preferredKey;
        // inventoryKeyText.text = inventoryKeyPreference;
        // Debug.Log("Inv. key: " + inventoryKeyText.text);
    }

    public void Update()
    {
        if (keyPressed)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                    {
                        Debug.Log("Key pressed: " + kc.ToString());
                        inventoryKeyPreference = kc;
                        keyPressed = false;
                    }
                }
            }
        }
    }

    public void Start()
    {
        LoadPreferences();
        InventoryKeyHandler();
    }
}
