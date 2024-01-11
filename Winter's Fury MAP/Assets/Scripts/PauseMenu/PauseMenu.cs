using System;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitScript : MonoBehaviour
{
    public GameObject pauseMenuObj;

    public static QuitScript qs;
    // Start is called before the first frame update
    private void Awake()
    {
        qs = this;
    }

    private void Start()
    {
        pauseMenuObj.SetActive(false);
    }

    // Update is called once per frame
    // void Update()
    // {
    //
    // }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Hra ukončena");
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void ToggleGame()
    {
        pauseMenuObj.SetActive(!pauseMenuObj.activeSelf); // do hry
        InventoryManager.Instance.ToggleInventory();
    }
}
