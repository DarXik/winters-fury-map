using System;
using System.Collections.Generic;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weather.Wind;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuObj, pauseMenuNavObj, pauseMenuOptionsObj;

    public static PauseMenu Instance { get; private set; }

    public TMP_Text mainSign;

    private void Awake()
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
        Application.Quit();
        Debug.Log("Hra ukončena");
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
        // uložilo by hru
    }

    public GameObject SurvivalOverlay;
    public GameObject FpsCounter;


    public void TogglePauseMenu()
    {
        if (pauseMenuObj.activeSelf)
        {
            pauseMenuObj.SetActive(false);
            pauseMenuNavObj.SetActive(false);
            pauseMenuOptionsObj.SetActive(false);
            PlayerLook.Instance.UnblockRotation();

            SurvivalOverlay.SetActive(true);
            FpsCounter.SetActive(true); // později toggle v nastavení, 4.výstup
            WindUI.Instance.DisplayWindIcon();

            PreferencesManager.Instance.SavePreferences();
            GameManager.Instance.ResumeTime();
        }
        else // do pause menu
        {
            pauseMenuObj.SetActive(true);
            pauseMenuNavObj.SetActive(true);
            pauseMenuOptionsObj.SetActive(false);
            PlayerLook.Instance.BlockRotation();

            SurvivalOverlay.SetActive(false);
            FpsCounter.SetActive(false);
            WindUI.Instance.HideWindIcon();
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