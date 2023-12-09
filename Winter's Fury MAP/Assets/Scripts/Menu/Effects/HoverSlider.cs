using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverSlider : MonoBehaviour
{
    public TextMeshProUGUI textValue;

    public void OnHandle()
    {
        textValue.gameObject.SetActive(true);
        textValue.text = GetComponent<Slider>().value.ToString();
    }

    public void ExitHandle()
    {
        textValue.gameObject.SetActive(false);
    }
}
