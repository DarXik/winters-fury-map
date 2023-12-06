using System.Collections;
using Lighting;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public enum PassTypes
    {
        Sleep,
        PassTime
    }
    
    public class PassTimeManager : MonoBehaviour
    {
        [Header("UI References")] 
        [SerializeField] private GameObject passTimeWindow;
        [SerializeField] private Button sleepTypeButton, passTypeButton;
        [SerializeField] private TextMeshProUGUI header, subheader, hoursToText, passButtonText;
        [SerializeField] private GameObject bedInfo;
        [SerializeField] private TextMeshProUGUI calorieStore, caloriesBurned, feelsLike, bedWarmth;
        [SerializeField] private Button passButton;
        [SerializeField] private GameObject passButtonObj;
        [SerializeField] private GameObject leftArrow, rightArrow;
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
            sleepTypeButton.onClick.AddListener(() => AssignUI(PassTypes.Sleep));
            passTypeButton.onClick.AddListener(() => AssignUI(PassTypes.PassTime));
            
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

        public void TogglePassTimeWindow(PassTypes passType, float? bedWarmthBonus = null)
        {
            if (!passTimeWindowOpened)
            {
                passTimeWindow.SetActive(true);
                passButtonObj.SetActive(true);
                leftArrow.SetActive(true);
                rightArrow.SetActive(true);
                PlayerLook.Instance.BlockRotation();

                Clock.Instance.SetClock();

                passTimeWindowOpened = true;

                AssignUI(passType, bedWarmthBonus);
            }
            else
            {
                ClosePassWindow();
            }
        }

        public void ClosePassWindow()
        {
            StopAllCoroutines();
            GameManager.Instance.cycle.TimeIncrement = normalTimeIncrement;
            passTimeWindow.SetActive(false);
            PlayerLook.Instance.UnblockRotation();
            PlayerController.Instance.currentActivity = PlayerActivity.Standing;
            UpdateLighting.Instance.ForceUpdateEnvironmentLighting();

            passTimeWindowOpened = false;
            hoursToPass = 1;
        }

        private void AssignUI(PassTypes passType, float? bedWarmthBonus = null)
        {
            passButton.onClick.RemoveAllListeners();
            
            switch (passType)
            {
                case PassTypes.Sleep:
                    header.text = "Sleep";
                    subheader.text =
                        "Advance time with the benefits of sleeping. Reduces fatigue and gains condition if healthy.";
                    hoursToText.text = "Hours to sleep";
                    passButtonText.text = "Sleep";
                    passButton.onClick.AddListener(TrySleep);
                    sleepTypeButton.interactable = false;
                    passTypeButton.interactable = true;
                    
                    bedInfo.SetActive(true);
                    calorieStore.text = Mathf.RoundToInt(VitalManager.Instance.GetCurrentCalories()).ToString();
                    caloriesBurned.text = (VitalManager.Instance.sleepingBurnRate * hoursToPass).ToString();
                    feelsLike.text = $"{Mathf.RoundToInt(VitalManager.Instance.feelsLikeTemp)}°C";
                    bedWarmth.text = $"+{bedWarmthBonus}°C";
                    break;
                case PassTypes.PassTime:
                    header.text = "Pass Time";
                    subheader.text =
                        "Advance time without the need to sleep.";
                    hoursToText.text = "Hours to pass";
                    passButtonText.text = "Pass time";
                    passButton.onClick.AddListener(TryPassTime);
                    sleepTypeButton.interactable = true;
                    passTypeButton.interactable = false;
                    bedInfo.SetActive(false);
                    break;
            }
        }

        public void TrySleep()
        {
            StartCoroutine(Sleep());
        }

        public void TryPassTime()
        {
            StartCoroutine(PassTime());
        }

        private IEnumerator Sleep()
        {
            passButtonObj.SetActive(false);
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);

            GameManager.Instance.cycle.TimeIncrement = passingTimeIncrement;
            PlayerController.Instance.currentActivity = PlayerActivity.Sleeping;

            yield return new WaitForSeconds(hoursToPass / passingTimeIncrement);
            
            ClosePassWindow();
        }

        private IEnumerator PassTime()
        {
            passButtonObj.SetActive(false);
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);

            GameManager.Instance.cycle.TimeIncrement = passingTimeIncrement;

            yield return new WaitForSeconds(hoursToPass / passingTimeIncrement);

            GameManager.Instance.cycle.TimeIncrement = normalTimeIncrement;
            UpdateLighting.Instance.ForceUpdateEnvironmentLighting();
            hoursToPass = 1;
            passButtonObj.SetActive(true);
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
            caloriesBurned.text = (VitalManager.Instance.sleepingBurnRate * hoursToPass).ToString();
        }
    }
}