using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScript : MonoBehaviour
{
    // private Dictionary<string, KeyCode> keyCodes = new();
    private string inventoryKeyPreference;
    private string passTimeKeyPreference;

    private bool inventoryKeyPressed;
    private bool passTimeKeyPressed;

    public Button inventoryKeyButton;
    public TMP_Text inventoryKeyText;
    public Button passTimeKeyButton;
    public TMP_Text passTimeKeyText;

    public void Start()
    {
        LoadPreferences();
    }

    public void SavePreferences()
    {
        // keyCodes["inventoryKey"].ToString()
        PlayerPrefs.SetString("inventoryKey", inventoryKeyPreference);
        PlayerPrefs.SetString("passTimeKey", passTimeKeyPreference);
        Debug.Log("Keys uloženo");
    }

    public void LoadPreferences()
    {
        // keyCodes["inventoryKey"] = PlayerPrefs.HasKey("inventoryKey") ? PlayerPrefs.GetString(Enum.TryParse("inventoryKey", out KeyCode )) : KeyCode.Tab;

        // if (PlayerPrefs.HasKey("inventoryKey"))
        // {
        //     inventoryKeyPreference = PlayerPrefs.GetString("inventoryKey");
        //     // if (Enum.TryParse(inventoryKeyPreference, out KeyCode keycode))
        //     // {
        //     //     keyCodes["inventoryKey"] = keycode;
        //     // }
        // }
        // else
        // {
        //     inventoryKeyPreference = "Tab";
        // }

        inventoryKeyPreference = PlayerPrefs.HasKey("inventoryKey") ? PlayerPrefs.GetString("inventoryKey") : "Tab";
        inventoryKeyText.text = inventoryKeyPreference;

        passTimeKeyPreference = PlayerPrefs.HasKey("passTimeKey") ? PlayerPrefs.GetString("passTimeKey") : "T";
        passTimeKeyText.text = passTimeKeyPreference;

        Debug.Log("Keys načteno");
    }

    public void InventoryKeyHandler()
    {
        // inventoryKeyPressed = true;
        // inventoryKeyPreference = preferredKey;
        // inventoryKeyText.text = inventoryKeyPreference;
        // Debug.Log("Inv. key: " + inventoryKeyText.text);
        // StartCoroutine(KeyHandlerCoroutine(inventoryKeyText, key => inventoryKeyPreference = key, true));

        StartCoroutine(InventoryKeyCoroutine(true));
        SavePreferences();
    }

    public void PassTimeKeyHandler()
    {
        StartCoroutine(PassTimeCoroutine(true));
    }

    private IEnumerator InventoryKeyCoroutine(bool keyPressed)
    {
        while (keyPressed)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                    {
                        Debug.Log("Key pressed: " + kc.ToString());
                        inventoryKeyPreference = kc.ToString();
                        keyPressed = false;
                        inventoryKeyText.text = inventoryKeyPreference;
                    }
                }
            }

            Debug.Log("jedu");
            yield return null;
        }
    }


    private IEnumerator PassTimeCoroutine(bool keyPressed)
    {
        while (keyPressed)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                    {
                        Debug.Log("Key pressed: " + kc.ToString());
                        passTimeKeyPreference = kc.ToString();
                        keyPressed = false;
                        passTimeKeyText.text = passTimeKeyPreference;
                    }
                }
            }


            yield return null;
        }
    }
}