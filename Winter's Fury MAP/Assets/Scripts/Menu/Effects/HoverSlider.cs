using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        Debug.Log("enter");
        // animator.Play("anim1");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleVisibility(false);

        Debug.Log("leave");
        // animator.Play("anim2");
        animator.SetTrigger("HoverOut");
    }

    public void ToggleVisibility(bool visibility)
    {
        sliderTextFPS.SetActive(visibility);
    }
}