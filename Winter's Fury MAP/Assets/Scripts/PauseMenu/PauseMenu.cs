using System;
using System.Collections.Generic;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Weather.Wind;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuObj, pauseMenuNavObj, pauseMenuOptionsObj;

    public static PauseMenu Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pauseMenuObj.SetActive(false);
        pauseMenuNavObj.SetActive(false);
        pauseMenuOptionsObj.SetActive(false);
    }

    public void ExitGame()
    {
        PreferencesManager.Instance.SavePreferences();
        Application.Quit();
    }

    public void ExitToMenu()
    {
        PreferencesManager.Instance.SavePreferences();
        SceneManager.LoadScene(0);
        // uložilo by hru
    }

    [FormerlySerializedAs("SurvivalOverlay")] public GameObject survivalOverlay;
    [FormerlySerializedAs("FpsCounter")] public GameObject fpsCounter;
    [FormerlySerializedAs("Crosshair")] public GameObject crosshair;

    public void TogglePauseMenu()
    {

        // při kliku z podnabídky se odbarví tlačítko [general, video ...]

        if (pauseMenuObj.activeSelf)
        {
            pauseMenuObj.SetActive(false);
            pauseMenuNavObj.SetActive(false);
            pauseMenuOptionsObj.SetActive(false);
            PlayerLook.Instance.UnblockRotation();

            survivalOverlay.SetActive(true);
            fpsCounter.SetActive(GeneralScript.Instance.showFPSpreference);
            WindUI.Instance.DisplayWindIcon();
            crosshair.SetActive(true);

            PreferencesManager.Instance.SavePreferences();
            GameManager.Instance.KeySetup();
            GameManager.Instance.SetBrightness();
            VideoScript.Instance.LoadPreferences();
            GeneralScript.Instance.LoadPreferences();
            GameManager.Instance.ResumeTime();
            PlayerLook.Instance.SetSensitivity();
        }
        else // do pause menu
        {
            pauseMenuObj.SetActive(true);
            pauseMenuNavObj.SetActive(true);
            pauseMenuOptionsObj.SetActive(false);
            PlayerLook.Instance.BlockRotation();

            crosshair.SetActive(false);
            survivalOverlay.SetActive(false);
            fpsCounter.SetActive(false);
            WindUI.Instance.HideWindIcon();
            fpsCounter.SetActive(false);
            // PassTimeManager.Instance.ClosePassWindow();
            // InventoryManager.Instance.ToggleInventory(true);
            // FirestartManager.Instance.CloseFireStartWindow();

            GameManager.Instance.PauseTime();
        }
    }

    public void ToggleOptions()
    {
        if (pauseMenuOptionsObj.activeSelf) // z nastavení
        {
            pauseMenuOptionsObj.SetActive(false);
            pauseMenuNavObj.SetActive(true);
            PauseMenuOpitions.Instance.OptionsGeneral();
        }
        else // do nastavení
        {
            pauseMenuNavObj.SetActive(false);
            pauseMenuOptionsObj.SetActive(true);
            PauseMenuOpitions.Instance.OptionsGeneral();
        }
    }
}