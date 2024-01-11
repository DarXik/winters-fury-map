using System;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuObj, pauseMenuNavObj, pauseMenuOptionsObj;

    public static PauseMenu PM;
    // Start is called before the first frame update
    private void Awake()
    {
        PM = this;
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
        SceneManager.LoadScene(1);
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuObj.activeSelf)
        {
            pauseMenuObj.SetActive(false);
            pauseMenuNavObj.SetActive(false);
            pauseMenuOptionsObj.SetActive(false);
        }
        else
        {
            pauseMenuObj.SetActive(true);
            pauseMenuNavObj.SetActive(true);
            pauseMenuOptionsObj.SetActive(false);
        }
        InventoryManager.Instance.ToggleInventory();
    }

    public void ToggleOptions()
    {
        if (pauseMenuOptionsObj.activeSelf)
        {
            pauseMenuOptionsObj.SetActive(false);
            pauseMenuNavObj.SetActive(true);
            PauseMenuOpitions.PSoptions.OptionsGeneral();
        }
        else
        {
            pauseMenuNavObj.SetActive(false);
            pauseMenuOptionsObj.SetActive(true);
            PauseMenuOpitions.PSoptions.OptionsGeneral();
        }
    }
}
