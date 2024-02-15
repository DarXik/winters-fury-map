using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HoverSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private Animator animator;
    [FormerlySerializedAs("sliderTextFPS")] public GameObject sliderText;
    [FormerlySerializedAs("sliderFPS")] public Slider slider;

    private void Awake()
    {
        animator = sliderText.GetComponent<Animator>();
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
        sliderText.SetActive(visibility);
    }
}