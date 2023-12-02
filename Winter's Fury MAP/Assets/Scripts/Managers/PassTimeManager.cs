using System;
using System.Collections;
using Lighting;
using Managers;
using Player;
using TMPro;
using UnityEngine;

public class PassTimeManager : MonoBehaviour
{
    [Header("UI References")] [SerializeField]
    private GameObject passTimeWindow;

    [SerializeField] private GameObject passButton, leftArrow, rightArrow;
    [SerializeField] private TextMeshProUGUI hoursText;

    [Header("Setup")] public int maxPassHours;
    public float passingTimeIncrement;
    private float normalTimeIncrement;
    private int hoursToPass;

    public static bool passTimeWindowOpened;

    public static PassTimeManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        passTimeWindow.SetActive(false);

        normalTimeIncrement = GameManager.Instance.GetTimeIncrement();

        hoursToPass = 1;
    }

    private void Update()
    {
        if (passTimeWindowOpened)
        {
            UpdateWindowUI();
            Clock.Instance.RotateClock();
        }
    }

    public void TogglePassTimeWindow()
    {
        if (!passTimeWindowOpened)
        {
            passTimeWindow.SetActive(true);
            passButton.SetActive(true);
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
            PlayerLook.Instance.BlockRotation();

            Clock.Instance.SetClock();

            passTimeWindowOpened = true;
        }
        else
        {
            StopAllCoroutines();
            GameManager.Instance.cycle.TimeIncrement = normalTimeIncrement;
            passTimeWindow.SetActive(false);
            PlayerLook.Instance.UnblockRotation();

            passTimeWindowOpened = false;
            hoursToPass = 1;
        }
    }

    public void TryPassTime()
    {
        StartCoroutine(PassTime());
    }

    private IEnumerator PassTime()
    {
        passButton.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);

        GameManager.Instance.cycle.TimeIncrement = passingTimeIncrement;

        yield return new WaitForSeconds(hoursToPass / passingTimeIncrement);

        GameManager.Instance.cycle.TimeIncrement = normalTimeIncrement;
        UpdateLighting.Instance.ForceUpdateEnvironmentLighting();
        hoursToPass = 1;
        passButton.SetActive(true);
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }

    /*private IEnumerator PassTime()
    {
        var finalTime = GameManager.Instance.GetCurrentTime() + hoursToPass;
        finalTime %= 24f;

        var normalTimeIncrement = GameManager.Instance.GetTimeIncrement();

        passButton.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);

        while (Math.Abs(GameManager.Instance.GetCurrentTime() - finalTime) > 0.01f)
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
    }*/

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