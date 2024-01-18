using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
        LoadPreferences();
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetString("inventoryKey", inventoryKeyPreference);
        PlayerPrefs.SetString("passTimeKey", passTimeKeyPreference);
        Debug.Log("Keys uloženo");
    }

    private Dictionary<string, string> keys = new();

    private void Update()
    {
        // proč to funguje jen v updatu a při zavolání metody?, ani mi to pak nejde dynamicky v coroutine
        keys["passTimeKey"] = passTimeKeyPreference;
        keys["inventoryKey"] = inventoryKeyPreference;
    }

    public void LoadPreferences()
    {
        inventoryKeyPreference = PlayerPrefs.HasKey("inventoryKey") ? PlayerPrefs.GetString("inventoryKey") : "Tab";
        inventoryKeyText.text = inventoryKeyPreference;
        keys.Add("inventoryKey", inventoryKeyPreference);

        passTimeKeyPreference = PlayerPrefs.HasKey("passTimeKey") ? PlayerPrefs.GetString("passTimeKey") : "T";
        passTimeKeyText.text = passTimeKeyPreference;
        keys.Add("passTimeKey", passTimeKeyPreference);
        // Keys.Add("passTimeKey", Enum.TryParse(passTimeKeyPreference, out KeyCode kc2) ? kc2 : KeyCode.T);

        Debug.Log("Keys načteno");
    }

    public void InventoryKeyHandler()
    {
        // lambda expression - definuje akci, která se stane, pokud je key pressed
        // bere string key a nastaví pref na key a text na key
        StartCoroutine(HandleKeyCoroutine(true, (key) =>
        {
            inventoryKeyPreference = key;
            inventoryKeyText.text = key;
        }, inventoryAnim));
    }

    public void PassTimeKeyHandler()
    {
        StartCoroutine(HandleKeyCoroutine(true, (key) =>
        {
            passTimeKeyPreference = key;
            passTimeKeyText.text = key;
        }, passTimeAnim));
    }

    // delegát Action - co se stane při stisku tlačítka
    // Action<string> tak dovoluje passnout vlastní metodu (lambdu)
    // Action je predefined method, bere string a je void - do ní je passnuta v handleKeyAction lambda
    public Animation passTimeAnim;
    public Animation inventoryAnim;

    private IEnumerator HandleKeyCoroutine(bool keyPressed, Action<string> handleKeyAction, Animation anim)
    {
        while (keyPressed)
        {
            // buttonAnimator.SetTrigger("Pulsate");
            anim.Play("PulseAnim");
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                    {
                        if (keys.ContainsValue(kc.ToString()))
                        {
                            Debug.Log("Key already bound: " + kc);
                            break;
                        }
                        else
                        {
                            Debug.Log("Key pressed: " + kc);
                            handleKeyAction.Invoke(kc.ToString()); // invokne daného delegáta, spustí tak to, co je v lambdě
                            keyPressed = false;
                            anim.Play("DefaultPulse");
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
                    {
                        Debug.Log("Keybinding ukončen");
                        anim.Play("DefaultPulse");
                        keyPressed = false;
                        break;
                    }
                }
            }

            yield return null;
        }
    }
}