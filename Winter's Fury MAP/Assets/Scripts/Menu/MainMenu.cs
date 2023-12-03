using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public GameObject startObj;
    public GameObject optionsObj, optionsGeneralObj, optionsVideoObj, optionsAudioObj, optionsControlsObj;
    public AudioMixer audioMixer; // zobrazí input v unity pro objekt z audiomixeru
    private Resolution[] resolutions; // prázdná array pro rozlišení z pc
    public TMP_Dropdown resolutionDropdown; // zobrazí to input v unity pro připojení objektu

    private bool optionsOpened;
    private bool generalOpened;
    private bool videoOpened;
    private bool audioOpened;
    private bool controlsOpened;

    private void Start()
    {
        GetResolutions();

        startObj.SetActive(true);
        optionsObj.SetActive(false);
    }

    private void GetResolutions()
    {
        // vezme dostupná rozlišení, pro každý pc jiné
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // list pro dostupná rozlišení
        var options = new List<string>();

        int crntResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // projede každým rozlišením a uloží zformátované ve stringu
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
        audioMixer.SetFloat("MainVolume" + "", volume);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OptionsToggle()
    {
        if (!optionsOpened)
        {
            startObj.SetActive(false);
            optionsObj.SetActive(true);
            optionsOpened = true;
            OptionsGeneral();
        }
        else
        {
            startObj.SetActive(true);
            optionsObj.SetActive(false);
            optionsOpened = false;
        }
    }

    public void OptionsGeneral()
    {
        optionsGeneralObj.SetActive(true);
        optionsVideoObj.SetActive(false);
        optionsAudioObj.SetActive(false);
        optionsControlsObj.SetActive(false);
    }

    public void OptionsVideo()
    {
        optionsGeneralObj.SetActive(false);
        optionsVideoObj.SetActive(true);
        optionsAudioObj.SetActive(false);
        optionsControlsObj.SetActive(false);
    }

    public void OptionsAudio()
    {
        optionsGeneralObj.SetActive(false);
        optionsVideoObj.SetActive(false);
        optionsAudioObj.SetActive(true);
        optionsControlsObj.SetActive(false);
    }

    public void OptionsControls()
    {
        optionsGeneralObj.SetActive(false);
        optionsVideoObj.SetActive(false);
        optionsAudioObj.SetActive(false);
        optionsControlsObj.SetActive(true);
    }
}