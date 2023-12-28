using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        pitch = 0f;
        yaw = 80.55f;
        ShowUI();
        mainCamera.fieldOfView = baseFOV;
        myUIGroup.alpha = 0;
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

        // if (optionsObj.activeSelf)
        // {
        //     background.color = new(0, 0, 0, 0.8352941f);
        // }
        //
        // if (startObj.activeSelf)
        // {
        //     background.color = new(0, 0, 0, 0.5607843f);
        // }

        // if (startObj.activeSelf)
        // {
        CameraShake();
        // }

        generalButton.interactable = !optionsGeneralObj.activeSelf;
        videoButton.interactable = !optionsVideoObj.activeSelf;
        audioButton.interactable = !optionsAudioObj.activeSelf;
        controlsButton.interactable = !optionsControlsObj.activeSelf;
        controlsButton.interactable = !optionsControlsObj.activeSelf;
    }

    public void OptionsToggle()
    {
        if (!optionsOpened) // do nastavení
        {
            startObj.SetActive(false);
            optionsObj.SetActive(true);
            optionsOpened = true;
            OptionsGeneral();

            StartCoroutine(MoveCamera(target, 40));

            yaw = 122;
        }
        else // do main menu
        {
            startObj.SetActive(true);
            optionsObj.SetActive(false);
            optionsOpened = false;
            VideoScript.Instance.SavePreferences();
            ControlsScript.Instance.SavePreferences();
            AudioScript.Instance.SavePreferences();
            GeneralScript.Instance.SavePreferences();
            StartCoroutine(MoveCamera(start, 55));

            yaw = 80.55f;
        }
    }

    private void CameraShake()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        mainCamera.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
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


    private void ShowUI()
    {
        startObj.SetActive(true);
        optionsObj.SetActive(false);
        // StartCoroutine(Wait());
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

    private static IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);

        // startMenuRest.SetActive(true);
    }

    [Header("Game Objecty")] public GameObject startObj;
    public GameObject optionsObj;
    public GameObject optionsGeneralObj;
    public GameObject optionsVideoObj;
    public GameObject optionsAudioObj;
    public GameObject optionsControlsObj;
    public Button generalButton;
    public Button videoButton;
    public Button audioButton;
    public Button controlsButton;

    [Header("Kamera")] public Camera mainCamera;
    public Transform target, start;
    public float speedH;
    public float speedV;
    private float yaw;
    private float pitch;

    // [Tooltip("Time in seconds to move the camera to the desired position.")]
    [Header("Nastavení meníčka")] private readonly float timeToMoveCamera = 0.2f; // méně -> rychlejší
    private readonly float baseFOV = 55f;
    private bool cameraMoveEnabled;

    [Header("Sekce Options")] private bool optionsOpened;
    private bool generalOpened;
    private bool videoOpened;
    private bool audioOpened;
    private bool controlsOpened;

    [Header("Efekty")] [SerializeField] private CanvasGroup myUIGroup;
    public Image background;
    private bool fadeIn;
}