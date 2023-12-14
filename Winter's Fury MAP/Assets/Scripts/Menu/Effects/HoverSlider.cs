using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    public TextMeshProUGUI sliderTextFPS;
    public Slider sliderFPS;

    public void Start()
    {
        ToggleVisibility(false);
        animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ToggleVisibility(true);

        Debug.Log("down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ToggleVisibility(false);

        Debug.Log("up");

        // animator.SetBool("smallDur",false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToggleVisibility(true);

        Debug.Log("enter");
        // animator.Play("anim1");
        animator.SetTrigger("HoverOver");
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
        sliderTextFPS.gameObject.SetActive(visibility);
    }
}