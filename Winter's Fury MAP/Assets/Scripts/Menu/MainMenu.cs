using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Game Objecty")]
    public GameObject startObj;
    public GameObject optionsObj;
    public GameObject optionsGeneralObj;
    public GameObject optionsVideoObj;
    public GameObject optionsAudioObj;
    public GameObject optionsControlsObj;
    // public GameObject startMenuRest;

    [Header("Jiné typy")] public AudioMixer audioMixer; // zobrazí input v unity pro objekt z audiomixeru
    private Resolution[] resolutions; // prázdná array pro rozlišení z pc
    public TMP_Dropdown resolutionDropdown; // zobrazí to input v unity pro připojení objektu

    [Header("Kamera")] public Camera mainCamera;
    public Transform target, start;

    [Tooltip("Time in seconds to move the camera to the desired position.")] [Header("Nastavení meníčka")]
    private readonly float timeToMoveCamera = 0.2f; // méně -> rychlejší

    private readonly float baseFOV = 55f;
    private bool cameraMoveEnabled;

    [Header("Sekce Options")] private bool optionsOpened;
    private bool generalOpened;
    private bool videoOpened;
    private bool audioOpened;
    private bool controlsOpened;


    [Header("Efekty")] [SerializeField] private CanvasGroup myUIGroup;
    private bool fadeIn = false;
    // [SerializeField] private bool fadeOut = false;

    public void ShowUI()
    {
        myUIGroup.alpha = 0;
        startObj.SetActive(true);
        optionsObj.SetActive(false);
        StartCoroutine(Wait());
        fadeIn = true;
    }

    private void Update()
    {
        if (fadeIn)
        {
            if (myUIGroup.alpha < 1)
            {
                myUIGroup.alpha += Time.deltaTime;
                if (myUIGroup.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }
    }

    private void Start() // Start is called before the first frame update
    {
        ShowUI();
        GetResolutions();
        mainCamera.fieldOfView = baseFOV;
    }

    private IEnumerator MoveCamera(Transform target, float targetFOV)
    {
        var elapsedTime = 0f;

        var currentPos = mainCamera.transform.position;
        var currentRotation = mainCamera.transform.rotation;
        var currentFov = mainCamera.fieldOfView;

        while (elapsedTime < timeToMoveCamera)
        {
            mainCamera.transform.position =
                Vector3.Lerp(currentPos, target.position, elapsedTime / timeToMoveCamera);
            mainCamera.transform.rotation =
                Quaternion.Lerp(currentRotation, target.rotation, elapsedTime / timeToMoveCamera);
            mainCamera.fieldOfView = Mathf.Lerp(currentFov, targetFOV, elapsedTime / timeToMoveCamera);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        mainCamera.transform.position = target.position;
        mainCamera.transform.rotation = target.rotation;
        mainCamera.fieldOfView = targetFOV;
    }

    public void OptionsToggle()
    {
        if (!optionsOpened)
        {
            startObj.SetActive(false);
            optionsObj.SetActive(true);
            optionsOpened = true;
            OptionsGeneral();

            StartCoroutine(MoveCamera(target, 40));
        }

        else
        {
            startObj.SetActive(true);
            optionsObj.SetActive(false);
            optionsOpened = false;

            StartCoroutine(MoveCamera(start, 55));
        }
    }

    private static IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);

        // startMenuRest.SetActive(true);
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

    private void GetResolutions()
    {
        // vezme dostupná rozlišení, pro každý pc jiné
        resolutions = Screen.resolutions;
        // rates = Screen.
        resolutionDropdown.ClearOptions();

        // list pro dostupná rozlišení
        var options = new List<string>();

        int crntResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // projede každým rozlišením a uloží zformátované ve stringu
            string option = resolutions[i].width + "x" + resolutions[i].height + "@" + resolutions[i].refreshRateRatio;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
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


    public void UpdateResolution(int resolutionIndex) // v unity pro update, když uživatel změní, tak unity předá info a index
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ExitGame()
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