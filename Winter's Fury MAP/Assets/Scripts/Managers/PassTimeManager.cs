using System;
using System.Collections;
using Lighting;
using Managers;
using Player;
using TMPro;
using UnityEngine;

public class PassTimeManager : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private GameObject passTimeWindow;
    [SerializeField] private GameObject passButton, leftArrow, rightArrow;
    [SerializeField] private TextMeshProUGUI hoursText;

    [Header("Setup")] 
    public int maxPassHours;
    public float passingTimeIncrement;
    private int hoursToPass;

    private bool windowOpened;
    
    public static PassTimeManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        passTimeWindow.SetActive(false);

        hoursToPass = 1;
    }

    private void Update()
    {
        if (windowOpened)
        {
            UpdateWindowUI();
            Clock.Instance.RotateClock();
        }
    }

    public void TogglePassTimeWindow()
    {
        if (!windowOpened)
        {
            passTimeWindow.SetActive(true);
            passButton.SetActive(true);
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
            PlayerLook.Instance.BlockRotation();
            
            Clock.Instance.SetClock();
            
            windowOpened = true;
        }
        else
        {
            passTimeWindow.SetActive(false);
            PlayerLook.Instance.UnblockRotation();

            windowOpened = false;
            hoursToPass = 1;
        }
    }

    public void TryPassTime()
    {
        StartCoroutine(PassTime());
    }

    private IEnumerator PassTime()
    {
        var finalTime = GameManager.Instance.GetCurrentTime() + hoursToPass;
        finalTime %= 24f;
        
        var normalTimeIncrement = GameManager.Instance.GetTimeIncrement();
        
        passButton.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        
        while (GameManager.Instance.GetCurrentTime() < finalTime && windowOpened)
        {
            float remainingHours = finalTime - GameManager.Instance.GetCurrentTime();
            hoursToPass = (int)Mathf.Max(1, remainingHours + 1);
            
            GameManager.Instance.cycle.TimeIncrement = passingTimeIncrement;

            yield return null;
        }

        GameManager.Instance.cycle.TimeIncrement = normalTimeIncrement;
        UpdateLighting.Instance.ForceUpdateEnvironmentLighting();
        passButton.SetActive(true);
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    } 

    public void LowerHour()
    {
        if (hoursToPass > 1)
        {
            hoursToPass--;
        }
    }

    public void HeightenHour()
    {
        if (hoursToPass < maxPassHours)
        {
            hoursToPass++;
        }
    }

    private void UpdateWindowUI()
    {
        hoursText.text = hoursToPass.ToString();
    }
}
