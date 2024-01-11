using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuOpitions : MonoBehaviour
{
    public GameObject optionsGeneralObj;
    public GameObject optionsVideoObj;
    public GameObject optionsAudioObj;
    public GameObject optionsControlsObj;
    public Button generalButton;
    public Button videoButton;
    public Button audioButton;
    public Button controlsButton;

    public static PauseMenuOpitions PSoptions { get; set; }
    void Awake()
    {
        PSoptions = this;
    }

    void Update()
    {
        generalButton.interactable = !optionsGeneralObj.activeSelf;
        videoButton.interactable = !optionsVideoObj.activeSelf;
        audioButton.interactable = !optionsAudioObj.activeSelf;
        controlsButton.interactable = !optionsControlsObj.activeSelf;
        controlsButton.interactable = !optionsControlsObj.activeSelf;
    }
    public void DefaultOptions()
    {
        PlayerPrefs.DeleteAll();
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
