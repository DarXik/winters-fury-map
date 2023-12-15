using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class VitalUI : MonoBehaviour
{
    public Gradient lowValueGradient;

    [Header("Meters")] 
    public Slider healthMeter;
    public Image healthBarFill;
    public Image warmthMeter;
    public Image fatigueMeter;
    public Image thirstMeter;
    public Image hungerMeter;

    [Header("Icons")] 
    public Image healthIcon;
    public Image warmthIcon;
    public Image fatigueIcon;
    public Image thirstIcon;
    public Image hungerIcon;
    public GameObject cautionIcon;

    private float healthPercent;
    private float temperaturePercent;
    private float fatiguePercent;
    private float thirstPercent;
    private float hungerPercent;

    private bool cautionDisplayed;

    private void Start()
    {
        cautionIcon.SetActive(false);
    }

    private void Update()
    {
        GetPercents();
        UpdateNeedsUI();

        if (temperaturePercent <= 0 || fatiguePercent <= 0 || thirstPercent <= 0 || hungerPercent <= 0)
            DisplayCaution();
        
        if (temperaturePercent > 0 && fatiguePercent > 0 && thirstPercent > 0 && hungerPercent > 0)
            HideCaution();
    }

    private void DisplayCaution()
    {
        if (cautionDisplayed) return;
        
        cautionIcon.SetActive(true);
        cautionDisplayed = true;
    }

    private void HideCaution()
    {
        if (!cautionDisplayed) return;
        
        cautionIcon.SetActive(false);
        cautionDisplayed = false;
    }

    private void UpdateNeedsUI()
    {
        healthMeter.value = healthPercent;
        healthBarFill.color = lowValueGradient.Evaluate(healthPercent);
        healthIcon.color = lowValueGradient.Evaluate(healthPercent);
        
        warmthMeter.fillAmount = temperaturePercent;
        warmthMeter.color = lowValueGradient.Evaluate(temperaturePercent);
        warmthIcon.color = lowValueGradient.Evaluate(temperaturePercent);
        
        fatigueMeter.fillAmount = fatiguePercent;
        fatigueMeter.color = lowValueGradient.Evaluate(fatiguePercent);
        fatigueIcon.color = lowValueGradient.Evaluate(fatiguePercent);
        
        thirstMeter.fillAmount = thirstPercent;
        thirstMeter.color = lowValueGradient.Evaluate(thirstPercent);
        thirstIcon.color = lowValueGradient.Evaluate(thirstPercent);
        
        hungerMeter.fillAmount = hungerPercent;
        hungerMeter.color = lowValueGradient.Evaluate(hungerPercent);
        hungerIcon.color = lowValueGradient.Evaluate(hungerPercent);
    }
    

    private void GetPercents()
    {
        healthPercent = VitalManager.Instance.HealthPercent;
        temperaturePercent = VitalManager.Instance.WarmthPercent;
        fatiguePercent = VitalManager.Instance.FatiguePercent;
        thirstPercent = VitalManager.Instance.ThirstPercent;
        hungerPercent = VitalManager.Instance.HungerPercent;
    }
}
