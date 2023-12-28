using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScript : MonoBehaviour
{
    private string inventoryKeyPreference;
    private string passTimeKeyPreference;

    private bool inventoryKeyPressed;
    private bool passTimeKeyPressed;

    public Button inventoryKeyButton;
    public TMP_Text inventoryKeyText;
    public Button passTimeKeyButton;
    public TMP_Text passTimeKeyText;

    public static ControlsScript Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadPreferences();
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetString("inventoryKey", inventoryKeyPreference);
        PlayerPrefs.SetString("passTimeKey", passTimeKeyPreference);
        Debug.Log("Keys uloženo");
    }

    private void LoadPreferences()
    {
        inventoryKeyPreference = PlayerPrefs.HasKey("inventoryKey") ? PlayerPrefs.GetString("inventoryKey") : "Tab";
        inventoryKeyText.text = inventoryKeyPreference;

        passTimeKeyPreference = PlayerPrefs.HasKey("passTimeKey") ? PlayerPrefs.GetString("passTimeKey") : "T";
        passTimeKeyText.text = passTimeKeyPreference;

        Debug.Log("Keys načteno");
    }

    public void InventoryKeyHandler()
    {
        // lambda expression - definuje akci, která se stane, pokud je key pressed
        // bere string key a nastaví pref na key a text na key
        StartCoroutine(HandleKeyCoroutine(true, (key) => { inventoryKeyPreference = key; inventoryKeyText.text = key; }));
    }

    public void PassTimeKeyHandler()
    {
        StartCoroutine(HandleKeyCoroutine(true, (key) => { passTimeKeyPreference = key; passTimeKeyText.text = key; }));
    }

    // delegát Action - co se stane při stisku tlačítka
    // Action<string> tak dovoluje passnout vlastní metodu (lambdu)
    // Action je predefined method, bere string a je void - do ní je passnuta v handleKeyAction lambda
    private IEnumerator HandleKeyCoroutine(bool keyPressed, Action<string> handleKeyAction)
    {
        while (keyPressed)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                    {
                        Debug.Log("Key pressed: " + kc);
                        handleKeyAction.Invoke(kc.ToString()); // invokne daného delegáta, spustí tak to, co je v lambdě
                        keyPressed = false;
                    }
                }
            }

            yield return null;
        }
    }
}