using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer; // zobrazí input v unity pro objekt z audiomixeru
    public Resolution[] resolutions; // prázdná array pro rozlišení z pc
    public TMP_Dropdown resolutionDropdown; // zobrazí to input v unity pro připojení objektu

    void Start()
    {
        // vezme dostupná rozlišení, pro každý pc jiné
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // list pro dostupná rozlišení
        var options = new List<string>();

        int crntResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // projedek každým rozlišením a uloží zformátované ve stringu
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                crntResolutionIndex = i;
            }
        }

        // vezme zformátovaná rozlišení a dá je do dropdownu
        resolutionDropdown.AddOptions(options);
        // nastaví rozlišení na dropdown na naše aktuální
        resolutionDropdown.value = crntResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void UpdateResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Hra ukončena");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume" +
                            "", volume);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}