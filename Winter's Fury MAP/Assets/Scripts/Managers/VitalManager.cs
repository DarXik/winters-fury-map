using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class VitalManager : MonoBehaviour
    {
        [FormerlySerializedAs("tempChevrons")] [Header("UI References")]
        public GameObject reduceTempChevrons;

        public GameObject increaseTempChevrons;
        public GameObject fatigueChevrons;
        public GameObject thirstChevrons;
        public GameObject hungerChevrons;

        [Header("Health")] 
        public float maxHealth;
        public float healthRecoveryRate;    
        public float warmthDrainRate;
        public float fatigueDrainRate;
        public float thirstDrainRate;
        public float hungerDrainRate;
        public float burnDamage;
        private float currentHealth;

        [Header("Temperature")] public float maxTempBar;

        [FormerlySerializedAs("chevronThresholds")]
        public float[] reduceThresholds = new float[3];

        public float[] increaseThresholds = new float[3];
        [HideInInspector] public float tempFromFire;
        private float feelsLikeTemp;
        private float currentTemp;
        private int reduceTempChevronsToReveal, increaseTempChevronsToReveal;

        [Header("Fatigue")] public float maxFatigueBar;
        public float smallDecreaseRate, mediumDecreaseRate, highDecreaseRate;
        private float currentFatigue;
        private int fatigueChevronsToReveal;

        [Header("Hunger (Calories)")] public float maxCalories;
        public float standingBurnRate, walkingBurnRate, runningBurnRate;
        private float currentCalories;
        private PlayerActivity currentActivity;
        private int hungerChevronsToReveal;

        [Header("Thirst (mL)")] public float maxThirst;
        public float awakeDepletionRate, asleepDepletionRate;
        private float currentThirst;
        private PlayerAwakeness currentAwakeness;
        private int thirstChevronsToReveal;

        // Fill Amounts
        public float HealthPercent => currentHealth / maxHealth;
        public float FatiguePercent => currentFatigue / maxFatigueBar;
        public float HungerPercent => currentCalories / maxCalories;
        public float ThirstPercent => currentThirst / maxThirst;
        public float TemperaturePercent => currentTemp / maxTempBar;

        public static VitalManager Instance { get; private set; }

        public static float timeIncrement;

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

            timeIncrement = GameManager.Instance.GetTimeIncrement();

            currentAwakeness = PlayerController.Instance.currentAwakeness;

            HideIncreaseChevrons();
            HideReduceChevrons();
        }

        private void Update()
        {
            feelsLikeTemp = TemperatureManager.ambientTemperature + tempFromFire;

            ReduceHunger();
            ReduceThirst();
            IncreaseTemperature();
            ReduceTemperature();
            ReduceFatigue();
            ReduceHealth();
            
            // if all four needs are above 0, recover health
            if (currentCalories > 0 && currentFatigue > 0 && currentThirst > 0 && currentTemp > 0)
            {
                RecoverHealth();   
            }
            
            currentActivity = PlayerController.Instance.currentActivity;
        }

        public void BurnPlayer()
        {
            currentHealth -= burnDamage * Time.deltaTime;
            
            // add visual effect
        }

        private void RecoverHealth()
        {
            if (currentHealth >= maxHealth) return;

            currentHealth += healthRecoveryRate * (Time.deltaTime * timeIncrement);
        }

        private void ReduceHealth()
        {
            if (TemperaturePercent <= 0)
            {
                currentHealth -= warmthDrainRate * (Time.deltaTime * timeIncrement);
            }

            if (FatiguePercent <= 0)
            {
                currentHealth -= fatigueDrainRate * (Time.deltaTime * timeIncrement);
            }

            if (ThirstPercent <= 0)
            {
                currentHealth -= thirstDrainRate * (Time.deltaTime * timeIncrement);
            }

            if (HungerPercent <= 0)
            {
                currentHealth -= hungerDrainRate * (Time.deltaTime * timeIncrement);
            }
        }

        private void ReduceFatigue()
        {
            if (currentFatigue <= 0)
            {
                if (fatigueChevrons.activeInHierarchy) fatigueChevrons.SetActive(false);

                return;
            }

            if (!fatigueChevrons.activeInHierarchy) fatigueChevrons.SetActive(true);

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
                fatigueChevrons.transform.GetChild(i).gameObject.SetActive(i < fatigueChevronsToReveal);
            }
        }

        public float AddHunger(float caloriesIntake)
        {
            float totalCalories = currentCalories + caloriesIntake;
            float excessCalories = 0;

            if (totalCalories >= maxCalories)
            {
                excessCalories = totalCalories - maxCalories;
                currentCalories = maxCalories;
            }
            else
            {
                currentCalories = totalCalories;
            }

            return Mathf.Round(excessCalories);
        }


        private void ReduceHunger()
        {
            if (currentCalories <= 0)
            {
                if (hungerChevrons.activeInHierarchy) hungerChevrons.SetActive(false);

                return;
            }

            if (!hungerChevrons.activeInHierarchy) hungerChevrons.SetActive(true);

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
                hungerChevrons.transform.GetChild(i).gameObject.SetActive(i < hungerChevronsToReveal);
            }
        }

        public float AddThirst(float waterIntake)
        {
            float totalWater = currentThirst + waterIntake;
            float excessWater = 0;

            if (totalWater >= maxThirst)
            {
                excessWater = totalWater - maxThirst;
                currentThirst = maxThirst;
            }
            else
            {
                currentThirst = totalWater;
            }

            return Mathf.Round(excessWater);
        }

        private void ReduceThirst()
        {
            if (currentThirst <= 0)
            {
                if (thirstChevrons.activeInHierarchy) thirstChevrons.SetActive(false);

                return;
            }

            if (!thirstChevrons.activeInHierarchy) thirstChevrons.SetActive(true);

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
                thirstChevrons.transform.GetChild(i).gameObject.SetActive(i < thirstChevronsToReveal);
            }
        }

        private void IncreaseTemperature()
        {
            if (feelsLikeTemp <= 0) return;
            
            if (currentTemp >= maxTempBar)
            {
                if (increaseTempChevrons.activeInHierarchy) increaseTempChevrons.SetActive(false);

                return;
            }

            HideReduceChevrons();

            if (feelsLikeTemp >= increaseThresholds[0] && feelsLikeTemp <= increaseThresholds[1])
            {
                currentTemp += maxTempBar / 15f * (Time.deltaTime * timeIncrement);

                increaseTempChevronsToReveal = 1;
            }
            else if (feelsLikeTemp >= increaseThresholds[1] && feelsLikeTemp <= increaseThresholds[2])
            {
                currentTemp += maxTempBar / 1f * (Time.deltaTime * timeIncrement);

                increaseTempChevronsToReveal = 2;
            }
            else if (feelsLikeTemp >= increaseThresholds[2])
            {
                currentTemp += maxTempBar / 0.5f * (Time.deltaTime * timeIncrement);

                increaseTempChevronsToReveal = 3;
            }

            for (int i = 0; i < increaseTempChevrons.transform.childCount; i++)
            {
                increaseTempChevrons.transform.GetChild(i).gameObject.SetActive(i < increaseTempChevronsToReveal);
            }
        }

        private void ReduceTemperature()
        {
            if (feelsLikeTemp > 0) return;

            if (currentTemp <= 0)
            {
                if (reduceTempChevrons.activeInHierarchy) reduceTempChevrons.SetActive(false);

                return;
            }

            if (!reduceTempChevrons.activeInHierarchy) reduceTempChevrons.SetActive(true);

            HideIncreaseChevrons();

            if (feelsLikeTemp <= reduceThresholds[0] && feelsLikeTemp >= reduceThresholds[1])
            {
                currentTemp -= maxTempBar / 15f * (Time.deltaTime * timeIncrement);

                reduceTempChevronsToReveal = 1;
            }
            else if (feelsLikeTemp <= reduceThresholds[1] && feelsLikeTemp >= reduceThresholds[2])
            {
                currentTemp -= maxTempBar / 1f * (Time.deltaTime * timeIncrement);

                reduceTempChevronsToReveal = 2;
            }
            else if (feelsLikeTemp <= reduceThresholds[2])
            {
                currentTemp -= maxTempBar / 0.5f * (Time.deltaTime * timeIncrement);

                reduceTempChevronsToReveal = 3;
            }

            for (int i = 0; i < reduceTempChevrons.transform.childCount; i++)
            {
                reduceTempChevrons.transform.GetChild(i).gameObject.SetActive(i < reduceTempChevronsToReveal);
            }
        }

        private void HideReduceChevrons()
        {
            reduceTempChevronsToReveal = 0;

            for (int i = 0; i < reduceTempChevrons.transform.childCount; i++)
            {
                reduceTempChevrons.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void HideIncreaseChevrons()
        {
            increaseTempChevronsToReveal = 0;

            for (int i = 0; i < increaseTempChevrons.transform.childCount; i++)
            {
                increaseTempChevrons.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}