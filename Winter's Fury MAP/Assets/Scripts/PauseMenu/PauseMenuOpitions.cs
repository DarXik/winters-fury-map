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

    public static PauseMenuOpitions Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
    }
    public void OptionsGeneral()
    {
        optionsGeneralObj.SetActive(true);
        optionsVideoObj.SetActive(false);
        optionsAudioObj.SetActive(false);
        optionsControlsObj.SetActive(false);

        // generalButton.interactable = false;
        // videoButton.interactable = true;
        // audioButton.interactable = true;
        // controlsButton.interactable = true;
    }

    public void OptionsVideo()
    {
        optionsGeneralObj.SetActive(false);
        optionsVideoObj.SetActive(true);
        optionsAudioObj.SetActive(false);
        optionsControlsObj.SetActive(false);

        // generalButton.interactable = true;
        // videoButton.interactable = false;
        // audioButton.interactable = true;
        // controlsButton.interactable = true;
    }

    public void OptionsAudio()
    {
        optionsGeneralObj.SetActive(false);
        optionsVideoObj.SetActive(false);
        optionsAudioObj.SetActive(true);
        optionsControlsObj.SetActive(false);

        // generalButton.interactable = true;
        // videoButton.interactable = true;
        // audioButton.interactable = false;
        // controlsButton.interactable = true;
    }

    public void OptionsControls()
    {
        optionsGeneralObj.SetActive(false);
        optionsVideoObj.SetActive(false);
        optionsAudioObj.SetActive(false);
        optionsControlsObj.SetActive(true);

        // generalButton.interactable = true;
        // videoButton.interactable = true;
        // audioButton.interactable = true;
        // controlsButton.interactable = false;
    }
}
