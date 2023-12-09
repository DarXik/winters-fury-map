using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        ShowUI();
        mainCamera.fieldOfView = baseFOV;
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

    private void ShowUI()
    {
        myUIGroup.alpha = 0;
        startObj.SetActive(true);
        optionsObj.SetActive(false);
        StartCoroutine(Wait());
        fadeIn = true;
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

            OptionsScript optionsScript = startObj.AddComponent<OptionsScript>();
            optionsScript.SavePreferences();
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

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Hra ukončena");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }



    [Header("Game Objecty")]
    public GameObject startObj;
    public GameObject optionsObj;
    public GameObject optionsGeneralObj;
    public GameObject optionsVideoObj;
    public GameObject optionsAudioObj;
    public GameObject optionsControlsObj;
    // public GameObject startMenuRest;

    [Header("Kamera")]
    public Camera mainCamera;
    public Transform target, start;

    // [Tooltip("Time in seconds to move the camera to the desired position.")]
    [Header("Nastavení meníčka")]
    private readonly float timeToMoveCamera = 0.2f; // méně -> rychlejší
    private readonly float baseFOV = 55f;
    private bool cameraMoveEnabled;

    [Header("Sekce Options")]
    private bool optionsOpened;
    private bool generalOpened;
    private bool videoOpened;
    private bool audioOpened;
    private bool controlsOpened;

    [Header("Efekty")] [SerializeField] private CanvasGroup myUIGroup;
    private bool fadeIn;
}