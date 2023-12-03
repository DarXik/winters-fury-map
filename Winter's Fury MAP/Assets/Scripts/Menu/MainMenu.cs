using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    // IN UNITY
    public GameObject startObj, optionsObj, optionsGeneralObj, optionsVideoObj, optionsAudioObj, optionsControlsObj, startMenuRest;
    public AudioMixer audioMixer; // zobrazí input v unity pro objekt z audiomixeru
    private Resolution[] resolutions; // prázdná array pro rozlišení z pc
    public TMP_Dropdown resolutionDropdown; // zobrazí to input v unity pro připojení objektu

    // CAMERA
    public Camera mainCamera;
    public Transform target;

    // public GameObject targetPosition;
    public int speed = 20;
    public float baseFOV;
    private bool cameraMoveEnabled;

    // OPTIONS
    private bool optionsOpened;
    private bool generalOpened;
    private bool videoOpened;
    private bool audioOpened;
    private bool controlsOpened;

    private void Start() // Start is called before the first frame update
    {
        GetResolutions();
        startObj.SetActive(true);
        // startMenuRest.SetActive(false);
        optionsObj.SetActive(false);
        // StartCoroutine(Wait());
        startMenuRest.SetActive(true);
        mainCamera.fieldOfView = baseFOV;
    }

    private void Update() // Update is called once per frame
    {
        if (cameraMoveEnabled)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, target.position, speed * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, target.rotation, speed * Time.deltaTime * 5);
            mainCamera.fieldOfView = 40f;
        }
    }

    public void OptionsToggle()
    {
        if (!optionsOpened)
        {
            startObj.SetActive(false);
            optionsObj.SetActive(true);
            optionsOpened = true;
            OptionsGeneral();

            cameraMoveEnabled = true;
        }

        else
        {
            startObj.SetActive(true);
            optionsObj.SetActive(false);
            optionsOpened = false;

            cameraMoveEnabled = false;
        }
    }

    // private void OnEnable()
    // {
    //     TypeWriter.CompleteTextRevealed += ShowMenu;
    // }
    //
    // private void OnDisable()
    // {
    //     TypeWriter.CompleteTextRevealed -= ShowMenu;
    // }
    //
    // private void ShowMenu()
    // {
    //
    // }

    // private IEnumerator Wait()
    // {
    //     yield return new WaitForSeconds(2f);
    // }
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
}