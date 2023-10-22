using Pinwheel.Jupiter;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class VitalManager : MonoBehaviour
    {
        [Header("UI References")] 
        public GameObject tempChevrons;
        public GameObject fatigueChevrons;
        public GameObject thirstChevrons;
        public GameObject hungerChevrons;

        [Header("Health")] 
        public float maxHealth;
        private float currentHealth;

        [Header("Temperature")] 
        public float maxTempBar;
        public float[] chevronThresholds = new float[3];
        private float feelsLikeTemp;
        private float ambientTemp;
        private float currentTemp;
        private int tempChevronsToReveal;

        [Header("Fatigue")] 
        public float maxFatigueBar;
        public float smallDecreaseRate, mediumDecreaseRate, highDecreaseRate;
        private float currentFatigue;
        private int fatigueChevronsToReveal;

        [Header("Hunger (Calories)")] 
        public float maxCalories;
        public float standingBurnRate, walkingBurnRate, runningBurnRate;
        private float currentCalories;
        private PlayerActivity currentActivity;
        private int hungerChevronsToReveal;

        [Header("Thirst (mL)")] 
        public float maxThirst;
        public float awakeDepletionRate, asleepDepletionRate;
        private float currentThirst;
        private PlayerAwakeness currentAwakeness;
        private int thirstChevronsToReveal;
    
        // Fill Amounts
        private float HealthPercent => currentHealth / maxHealth;
        public float FatiguePercent => currentFatigue / maxFatigueBar;
        public float HungerPercent => currentCalories / maxCalories;
        public float ThirstPercent => currentThirst / maxThirst;
        public float TemperaturePercent => currentTemp / maxTempBar;

        public static VitalManager Instance { get; private set; }
        private float timeIncrement;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            currentHealth = maxHealth;
            currentCalories = maxCalories;
            currentThirst = maxThirst;
            currentTemp = maxTempBar;
            currentFatigue = maxFatigueBar;

            timeIncrement = GameManager.Instance.cycle.TimeIncrement;
            currentAwakeness = PlayerController.Instance.currentAwakeness;
        }

        private void Update()
        {
            ambientTemp = TemperatureManager.ambientTemperature;
            feelsLikeTemp = ambientTemp;
            
            ReduceHunger();
            ReduceThirst();
            ReduceTemperature();
            ReduceFatigue();

            currentActivity = PlayerController.Instance.currentActivity;
        }

        private void ReduceFatigue()
        {
            if (currentFatigue <= 0) return;

            switch (currentActivity)
            {
                case PlayerActivity.Standing or PlayerActivity.Walking:
                    currentFatigue -= smallDecreaseRate * (Time.deltaTime * timeIncrement);
                    fatigueChevronsToReveal = 1;
                    break;
                case PlayerActivity.Running:
                    currentFatigue -= mediumDecreaseRate * (Time.deltaTime * timeIncrement);
                    fatigueChevronsToReveal = 2;
                    break;
            }

            for (int i = 0; i < fatigueChevrons.transform.childCount; i++)
            {
                //fatigueChevrons.transform.GetChild(i).gameObject.SetActive(i < fatigueChevronsToReveal);
            }
        }

        private void ReduceHunger()
        {
            if (currentCalories <= 0) return;

            switch (currentActivity)
            {
                case PlayerActivity.Standing:
                    currentCalories -= standingBurnRate * (Time.deltaTime * timeIncrement);
                    hungerChevronsToReveal = 1;
                    break;
                case PlayerActivity.Walking:
                    currentCalories -= walkingBurnRate * (Time.deltaTime * timeIncrement);
                    hungerChevronsToReveal = 2;
                    break;
                case PlayerActivity.Running:
                    currentCalories -= runningBurnRate * (Time.deltaTime * timeIncrement);
                    hungerChevronsToReveal = 3;
                    break;
            }

            for (int i = 0; i < hungerChevrons.transform.childCount; i++)
            {
                //hungerChevrons.transform.GetChild(i).gameObject.SetActive(i < hungerChevronsToReveal);
            }
        }

        private void ReduceThirst()
        {
            if (currentThirst <= 0) return;

            switch (currentAwakeness)
            {
                case PlayerAwakeness.Awake:
                    currentThirst -= awakeDepletionRate * (Time.deltaTime * timeIncrement);
                    thirstChevronsToReveal = 2;
                    break;
                case PlayerAwakeness.Asleep:
                    currentThirst -= asleepDepletionRate * (Time.deltaTime * timeIncrement);
                    thirstChevronsToReveal = 1;
                    break;
            }

            for (int i = 0; i < thirstChevrons.transform.childCount; i++)
            {
                //thirstChevrons.transform.GetChild(i).gameObject.SetActive(i < thirstChevronsToReveal);
            }
        }

        private void ReduceTemperature()
        {
            if (currentTemp <= 0) return;
            
            if (feelsLikeTemp <= chevronThresholds[0] && feelsLikeTemp >= chevronThresholds[1])
            {
                currentTemp -= maxTempBar/15f * (Time.deltaTime * timeIncrement);

                tempChevronsToReveal = 1;
            }
            else if (feelsLikeTemp <= chevronThresholds[1] && feelsLikeTemp >= chevronThresholds[2])
            {
                currentTemp -= maxTempBar/1f * (Time.deltaTime * timeIncrement);
                
                tempChevronsToReveal = 2;
            }
            else if (feelsLikeTemp <= chevronThresholds[2])
            {
                currentTemp -= maxTempBar/0.5f * (Time.deltaTime * timeIncrement);

                tempChevronsToReveal = 3;
            }

            for (int i = 0; i < tempChevrons.transform.childCount; i++)
            {
                //tempChevrons.transform.GetChild(i).gameObject.SetActive(i < tempChevronsToReveal);
            }
        }
    }
}
