using System;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void TogglePauseMenu()
    {
        if (pauseMenuObj.activeSelf)
        {
            pauseMenuObj.SetActive(false);
            pauseMenuNavObj.SetActive(false);
            pauseMenuOptionsObj.SetActive(false);
            PlayerLook.Instance.UnblockRotation();

            GameManager.Instance.ResumeTime();
        }
        else
        {
            pauseMenuObj.SetActive(true);
            pauseMenuNavObj.SetActive(true);
            pauseMenuOptionsObj.SetActive(false);
            PlayerLook.Instance.BlockRotation();

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