using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Player;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class VitalManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        [Header("UI References")] public GameObject reduceTempChevrons;

        public GameObject increaseTempChevrons;
        public GameObject fatigueChevrons;
        public GameObject thirstChevrons;
        public GameObject hungerChevrons;

        [Header("Health")] public float maxHealth;
        public float healthRecoveryRate;
        public float warmthDrainRate;
        public float fatigueDrainRate;
        public float thirstDrainRate;
        public float hungerDrainRate;
        public float burnDamage;
        private float currentHealth;
        public float CurrentHealth => currentHealth;

        [Header("Temperature")] public float maxTempBar;
        public float[] reduceThresholds = new float[3];
        public float[] increaseThresholds = new float[3];
        public float totalTimeToHypothermia;
        public float totalTimeToHealHypothermia;
        private float currentTimeToHypothermia, currentTimeToHealHypothermia;
        private float feelsLikeTemp;
        private float currentTemp;
        private int reduceTempChevronsToReveal, increaseTempChevronsToReveal;

        [Header("Fatigue")] public float maxFatigueBar;
        public float smallDecreaseRate, mediumDecreaseRate;
        public float sleepRecoveryRate;
        private float currentFatigue;
        private int fatigueChevronsToReveal;

        [Header("Hunger (Calories)")] public float maxCalories;
        public float sleepingBurnRate, standingBurnRate, walkingBurnRate, runningBurnRate;
        private float currentCalories;
        private PlayerActivity currentActivity;
        private int hungerChevronsToReveal;

        [Header("Thirst (mL)")] public float maxThirst;
        public float awakeDepletionRate, asleepDepletionRate;
        private float currentThirst;
        private int thirstChevronsToReveal;

        [Header("Death Window")] [SerializeField]
        private GameObject deathWindow;

        private CanvasGroup windowGroup;
        [SerializeField] private TextMeshProUGUI timeAlive;

        // Fill Amounts
        public float HealthPercent => currentHealth / maxHealth;
        public float FatiguePercent => currentFatigue / maxFatigueBar;
        public float HungerPercent => currentCalories / maxCalories;
        public float ThirstPercent => currentThirst / maxThirst;
        public float WarmthPercent => currentTemp / maxTempBar;

        public static bool burningPlayer;

        public static bool playerDead;

        // Afflictions
        private List<Affliction> currentAfflictions = new();

        public static VitalManager Instance { get; private set; }

        public static float timeIncrement;

        private void Awake()
        {
            Instance = this;

            windowGroup = deathWindow.GetComponent<CanvasGroup>();
            windowGroup.alpha = 0;

            deathWindow.SetActive(false);
        }

        private void Start()
        {
            currentHealth = maxHealth;
            currentCalories = maxCalories;
            currentThirst = maxThirst;
            currentTemp = maxTempBar;
            currentFatigue = maxFatigueBar;

            currentTimeToHypothermia = totalTimeToHypothermia;
            currentTimeToHealHypothermia = totalTimeToHealHypothermia;

            timeIncrement = GameManager.Instance.GetTimeIncrement();

            HideRecoverTempChevrons();
            HideReduceTempChevrons();
        }

        private void Update()
        {
            feelsLikeTemp = TemperatureManager.Instance.FeelsLike;
            currentActivity = PlayerController.Instance.currentActivity;

            ReduceHunger();
            ReduceThirst();

            switch (feelsLikeTemp)
            {
                case >= 0.5f:
                    RecoverTemperature();
                    HideReduceTempChevrons();
                    break;
                case <= 0.49f:
                    ReduceTemperature();
                    HideRecoverTempChevrons();
                    break;
            }

            switch (currentActivity)
            {
                case PlayerActivity.Sleeping:
                    RecoverFatigue();
                    break;
                case PlayerActivity.Standing or PlayerActivity.Walking or PlayerActivity.Running:
                    ReduceFatigue();
                    break;
            }

            if (currentTemp <= 0 || currentFatigue <= 0 || currentThirst <= 0 || currentCalories <= 0)
            {
                ReduceHealth();
            }

            if (currentCalories > 0 && currentFatigue > 0 && currentThirst > 0 && currentTemp > 0)
            {
                RecoverHealth();
            }

            // check afflictions
            if (currentAfflictions.Count > 0)
            {
                CheckAfflictions();
                ReduceAfflictionDuration();
            }

            // if currentTemp is below 0 and we don't have it, inflict it after a set amount of hours
            if (currentTemp <= 0 &&
                currentAfflictions.All(afflictions => afflictions.afflictionType != AfflictionType.Hypothermia))
            {
                CheckForHypothermia();
            }

            // if currentTemp is above 0 and we have Hypothermia, heal it
            if (currentTemp > 0 &&
                currentAfflictions.All(afflictions => afflictions.afflictionType == AfflictionType.Hypothermia))
            {
                HealHypothermia();
            }
        }

        public IEnumerator BurnPlayer()
        {
            while (burningPlayer)
            {
                yield return new WaitForSeconds(1f);

                currentHealth -= burnDamage;
            }
        }

        private void RecoverHealth()
        {
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                return;
            }

            switch (currentActivity)
            {
                case PlayerActivity.Sleeping:

                    float sleepHealthRecoveryRate =
                        CalculateSleepRecoveryRate(PassTimeManager.Instance.GetHoursToPass());
                    currentHealth += sleepHealthRecoveryRate * (Time.deltaTime * timeIncrement);

                    break;
                case PlayerActivity.Standing or PlayerActivity.Walking or PlayerActivity.Running:
                    currentHealth += healthRecoveryRate * (Time.deltaTime * timeIncrement);
                    break;
            }
        }

        private float CalculateSleepRecoveryRate(int hoursToSleep)
        {
            float totalConditionRecovery = 0;

            for (int i = 1; i <= hoursToSleep; i++)
            {
                totalConditionRecovery += i + 1;
            }

            return totalConditionRecovery / hoursToSleep;
        }

        private void ReduceHealth()
        {
            if (playerDead) return;
            
            if (currentHealth <= 0)
            {
                Die();

                return;
            }
            
            if (WarmthPercent <= 0)
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

        private void RecoverFatigue()
        {
            if (currentFatigue >= maxFatigueBar)
            {
                currentFatigue = maxFatigueBar;
                PassTimeManager.Instance.ClosePassWindow();

                return;
            }

            currentFatigue += sleepRecoveryRate * (Time.deltaTime * timeIncrement);
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
                case PlayerActivity.Sleeping:
                    currentCalories -= sleepingBurnRate * (Time.deltaTime * timeIncrement);
                    hungerChevronsToReveal = 1;
                    break;
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

            switch (currentActivity)
            {
                case PlayerActivity.Walking or PlayerActivity.Running or PlayerActivity.Standing:
                    currentThirst -= awakeDepletionRate * (Time.deltaTime * timeIncrement);
                    thirstChevronsToReveal = 2;
                    break;
                case PlayerActivity.Sleeping:
                    currentThirst -= asleepDepletionRate * (Time.deltaTime * timeIncrement);
                    thirstChevronsToReveal = 1;
                    break;
            }

            for (int i = 0; i < thirstChevrons.transform.childCount; i++)
            {
                thirstChevrons.transform.GetChild(i).gameObject.SetActive(i < thirstChevronsToReveal);
            }
        }

        private void RecoverTemperature()
        {
            if (currentTemp >= maxTempBar)
            {
                if (increaseTempChevrons.activeInHierarchy) increaseTempChevrons.SetActive(false);
                currentTemp = maxTempBar;

                return;
            }

            if (!increaseTempChevrons.activeInHierarchy) increaseTempChevrons.SetActive(true);

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
            if (currentTemp <= 0)
            {
                if (reduceTempChevrons.activeInHierarchy) reduceTempChevrons.SetActive(false);

                return;
            }

            if (!reduceTempChevrons.activeInHierarchy) reduceTempChevrons.SetActive(true);

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

        private void CheckForHypothermia()
        {
            currentTimeToHypothermia -= Time.deltaTime * timeIncrement;

            if (currentTimeToHypothermia <= 0)
            {
                InflictAffliction(Resources.Load<Affliction>("Scriptable Objects/Afflictions/Hypothermia"));

                currentTimeToHypothermia = totalTimeToHypothermia;
            }
        }

        private void HealHypothermia()
        {
            currentTimeToHealHypothermia -= Time.deltaTime * timeIncrement;

            if (currentTimeToHealHypothermia <= 0)
            {
                currentAfflictions.RemoveAll(afflictions => afflictions.afflictionType == AfflictionType.Hypothermia);

                currentTimeToHealHypothermia = totalTimeToHealHypothermia;
            }
        }

        private void ReduceAfflictionDuration()
        {
            for (var i = currentAfflictions.Count - 1; i >= 0; i--)
            {
                var affliction = currentAfflictions[i];

                if (!affliction.hasSetDuration) continue;

                if (affliction.currentDuration <= 0)
                {
                    currentAfflictions.Remove(affliction);

                    continue;
                }

                affliction.currentDuration -= Time.deltaTime * timeIncrement;
            }
        }

        private void CheckAfflictions()
        {
            foreach (var affliction in currentAfflictions)
            {
                switch (affliction.afflictionType)
                {
                    case AfflictionType.FoodPoisoning:
                        FoodPoisoning(affliction.wasTreated);
                        break;
                    case AfflictionType.Hypothermia:
                        Hypothermia();
                        break;
                }
            }
        }

        public void InflictAffliction(Affliction affliction)
        {
            if (currentAfflictions.Any(a => a.afflictionName == affliction.afflictionName))
            {
                var index = currentAfflictions.FindIndex(a => a.afflictionName == affliction.afflictionName);

                currentAfflictions[index].currentDuration = currentAfflictions[index].totalDuration;
            }
            else
            {
                Affliction afflictionCopy = Instantiate(affliction);
                afflictionCopy.totalDuration =
                    Mathf.Round(Random.Range(affliction.untreatedMin, affliction.untreatedMax));
                afflictionCopy.currentDuration = afflictionCopy.totalDuration;

                currentAfflictions.Add(afflictionCopy);

                StartCoroutine(InventoryUI.Instance.DisplayAfflictionAlert(afflictionCopy.afflictionName,
                    afflictionCopy.afflictionIcon));
            }
        }

        public void TreatAffliction(ItemData itemData, int itemCount, Affliction afflictionToTreat)
        {
            int index = currentAfflictions.IndexOf(afflictionToTreat);

            currentAfflictions[index].totalDuration = currentAfflictions[index].treated;
            currentAfflictions[index].currentDuration = currentAfflictions[index].totalDuration;
            currentAfflictions[index].wasTreated = true;

            InventoryUI.Instance.HideTreatmentChooser();

            for (int i = 0; i < itemCount; i++)
            {
                InventoryManager.Instance.DeleteItemByName(itemData.itemName);
            }
        }

        private void Hypothermia()
        {
            // lose condition and fatigue twice

            currentHealth -= warmthDrainRate * (Time.deltaTime * timeIncrement);
            currentHealth -= fatigueDrainRate * (Time.deltaTime * timeIncrement);
        }

        private void FoodPoisoning(bool treated)
        {
            // 10% of condition per hour if not below 15%, not sleeping and not treated
            if (currentHealth > 15f && PlayerController.Instance.currentActivity != PlayerActivity.Sleeping && !treated)
            {
                currentHealth -= 10f * (Time.deltaTime * timeIncrement);
            }

            // 30% of fatigue per hour
            currentFatigue -= 30f * (Time.deltaTime * timeIncrement);
        }

        private void HideReduceTempChevrons()
        {
            reduceTempChevronsToReveal = 0;

            for (int i = 0; i < reduceTempChevrons.transform.childCount; i++)
            {
                reduceTempChevrons.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void HideRecoverTempChevrons()
        {
            increaseTempChevronsToReveal = 0;

            for (int i = 0; i < increaseTempChevrons.transform.childCount; i++)
            {
                increaseTempChevrons.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void Die()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            playerDead = true;

            // UI 
            float totalMinutes = GameManager.TotalTime * 60f;

            int days = Mathf.FloorToInt(totalMinutes / 24f / 60f);
            int hours = Mathf.FloorToInt(totalMinutes / 60f % 24f);
            int minutes = Mathf.FloorToInt(totalMinutes % 60f);

            timeAlive.text = $"{days} DAYS {hours} HOURS {minutes} MINUTES";
            
            deathWindow.SetActive(true);
            windowGroup.LeanAlpha(1, 3.5f).setEaseInOutQuart();

            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<HarvestTrees>().enabled = false;

            var rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationY;

            var capsule = player.AddComponent<CapsuleCollider>();
            capsule.height = 2;
        }

        public float GetCurrentCalories()
        {
            return currentCalories;
        }

        public List<Affliction> GetCurrentAfflictions()
        {
            return currentAfflictions;
        }
    }
}