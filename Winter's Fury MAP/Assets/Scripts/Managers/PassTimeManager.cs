using System;
using System.Collections;
using Managers;
using Player;
using TMPro;
using UnityEngine;

public class PassTimeManager : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private GameObject passTimeWindow;
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
        if (windowOpened) UpdateWindowUI();
    }

    public void TogglePassTimeWindow()
    {
        if (!windowOpened)
        {
            passTimeWindow.SetActive(true);
            PlayerLook.Instance.BlockRotation();

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
        var normalTimeIncrement = GameManager.Instance.GetTimeIncrement();

        while (GameManager.Instance.GetCurrentTime() < finalTime)
        {
            GameManager.Instance.cycle.TimeIncrement = passingTimeIncrement;

            yield return new WaitForSeconds(0);
        }

        GameManager.Instance.cycle.TimeIncrement = normalTimeIncrement;
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
