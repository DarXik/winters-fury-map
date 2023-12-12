using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public TextMeshProUGUI sliderTextFPS;
    public Slider sliderFPS;
    // private bool sliderHandleEvent = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (eventData.pointerPress == sliderFPS.gameObject)
        // {
        sliderTextFPS.gameObject.SetActive(true);
        // }

        Debug.Log("down");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (eventData.pointerPress == sliderFPS.gameObject)
        // {
        sliderTextFPS.gameObject.SetActive(true);
        // }

        Debug.Log("down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // if (eventData.pointerPress == sliderFPS.gameObject)
        // {
        sliderTextFPS.gameObject.SetActive(false);
        // }

        Debug.Log("up");
    }

    public void Start()
    {
        sliderTextFPS.gameObject.SetActive(false);
    }
}