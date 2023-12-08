using System;
using TMPro;
using UnityEngine;

public class OptionsScript : MonoBehaviour
{
    [Header("Video")]
    public TextMeshProUGUI sliderTextBrigtness;
    public TextMeshProUGUI sliderTextFPS;
    public TextMeshProUGUI qualityOptionsText;
    // public Exposure exposure;

    [Header("Buttony Kvalita")]
    public bool clickedRight;
    public bool clickedLeft;
    private int currentQuality = 1;

    public void Start()
    {
        QualitySwitcher();
    }

    public void SetFPS(float fps)
    {
        sliderTextFPS.text = fps.ToString("0");
        Application.targetFrameRate = Convert.ToInt32(fps);
        if (fps == 241) // zeptat zbyňi
        {
            Application.targetFrameRate = -1;
            sliderTextFPS.text = "Unlimited";
        }
    }

    public void SetBrightness(float lumen)
    {
        sliderTextBrigtness.text = lumen.ToString("00");
    }

    private void QualitySwitcher()
    {
        var qualityOptions = QualitySettings.names;

        if (clickedRight)
        {
            currentQuality += 1;
            Debug.Log(currentQuality);
        }
        else if (clickedLeft)
        {
            currentQuality -= 1;
            Debug.Log(currentQuality);
        }
        else
        {
            currentQuality = 1;
            Debug.Log(currentQuality);
        }

        qualityOptionsText.text = qualityOptions[currentQuality];
    }

    public void RightClick()
    {
        clickedRight = true;
        clickedLeft = false;
        QualitySwitcher();
    }

    public void LeftClick()
    {
        clickedLeft = true;
        clickedRight = false;
        QualitySwitcher();
    }
}