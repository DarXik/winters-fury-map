using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
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
    public Image warmthIcon;
    public Image fatigueIcon;
    public Image thirstIcon;
    public Image hungerIcon;

    private float healthPercent;
    private float temperaturePercent;
    private float fatiguePercent;
    private float thirstPercent;
    private float hungerPercent;

    private void Update()
    {
        GetPercents();
        UpdateNeedsUI();
    }

    private void UpdateNeedsUI()
    {
        healthMeter.value = healthPercent;
        healthBarFill.color = lowValueGradient.Evaluate(healthPercent);
        
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
        temperaturePercent = VitalManager.Instance.TemperaturePercent;
        fatiguePercent = VitalManager.Instance.FatiguePercent;
        thirstPercent = VitalManager.Instance.ThirstPercent;
        hungerPercent = VitalManager.Instance.HungerPercent;
    }
}
