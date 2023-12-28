using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private Animator animator;
    public GameObject sliderTextFPS;
    public Slider sliderFPS;

    private void Awake()
    {
        animator = sliderTextFPS.GetComponent<Animator>();
    }

    public void Start()
    {
        ToggleVisibility(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToggleVisibility(true);
        // animator.Play("anim1");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ToggleVisibility(true);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ToggleVisibility(false);
    }

public void OnPointerExit(PointerEventData eventData)
    {
        if (!Input.GetMouseButton(0))
        {
            ToggleVisibility(false);
        }
        // animator.SetTrigger("HoverOut");
    }

    private void ToggleVisibility(bool visibility)
    {
        sliderTextFPS.SetActive(visibility);
    }
}