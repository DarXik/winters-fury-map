using System;
using Heat;
using Pinwheel.Jupiter;
using Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Weather;
using Weather.Wind;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Main Settings")] public JDayNightCycle cycle;
        public bool autoCycle;
        public bool randomizeSpawn;

        [Header("Spawn Settings")] [SerializeField]
        private Transform player;

        [SerializeField] private float[] times;
        [SerializeField] private Transform[] spawnPoints;

        private float previousTimeIncrement, timeIncrement;
        private float currentTimeIncrement;
        public Volume volume;
        private ColorAdjustments ca;

        public KeyCode toggleInventoryKey;
        public KeyCode togglePassTimeKey;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            volume.profile.TryGet(out ca);
            SetBrightness();
            SetKeyPreference("inventoryKey", out toggleInventoryKey);
            SetKeyPreference("passTimeKey", out togglePassTimeKey);
        }

        private void Start()
        {
            cycle.AutoTimeIncrement = autoCycle;
            previousTimeIncrement = cycle.TimeIncrement;

            if (randomizeSpawn) RandomizeSpawn();
        }

        private void RandomizeSpawn()
        {
            int randomTime = Random.Range(0, times.Length);
            int randomLoc = Random.Range(0, spawnPoints.Length);

            cycle.Time = times[randomTime];

            Vector3 spawnPos = spawnPoints[randomLoc].position;

            player.position = spawnPos;
        }

        private void Update()
        {
            CheckUserInput();
            CheckTimeIncrement();
        }

        private void CheckTimeIncrement()
        {
            var currIncrement = GetTimeIncrement();
            if (previousTimeIncrement != currIncrement)
            {
                VitalManager.timeIncrement = currIncrement;
                HeatSource.timeIncrement = currIncrement;
                WeatherSystem.timeIncrement = currIncrement;
                WindArea.timeIncrement = currIncrement;
                PlayerInteraction.timeIncrement = currIncrement;
                InventoryManager.timeIncrement = currIncrement;

                previousTimeIncrement = currIncrement;
            }
        }

        private void CheckUserInput()
        {
            if (Input.GetKeyDown(toggleInventoryKey) && !FirestartManager.fireWindowOpened && !PassTimeManager.passTimeWindowOpened && !AddFuelManager.addFuelWindowOpened)
            {
                InventoryManager.Instance.ToggleInventory();
            }

            if (Input.GetKeyDown(togglePassTimeKey) && !FirestartManager.fireWindowOpened && !InventoryManager.inventoryOpened && !AddFuelManager.addFuelWindowOpened)
            {
                PassTimeManager.Instance.TogglePassTimeWindow(PassTypes.PassTime);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseMenu.Instance.TogglePauseMenu();
            }
        }

        public void PauseTime()
        {
            currentTimeIncrement = cycle.TimeIncrement;
            cycle.TimeIncrement = 0;
        }

        public void ResumeTime()
        {
            cycle.TimeIncrement = currentTimeIncrement;
        }

        public float GetCurrentTime()
        {
            return cycle.Time;
        }

        public float GetTimeIncrement()
        {
            return cycle.TimeIncrement;
        }

        public void AddMinutes(float minutes)
        {
            cycle.Time += minutes / 60f;
        }

        // --- NAČTENÍ UŽIVATELSKÉHO NASTAVENÍ ---
        private void SetBrightness()
        {
            ca.postExposure.value = PlayerPrefs.GetFloat("brightnessPreference");
            Debug.Log("BrightnessPref načtena");
        }

        private void SetKeyPreference(string key, out KeyCode desiredKey)
        {
            var inventoryKey = PlayerPrefs.GetString(key);
            desiredKey = Enum.TryParse(inventoryKey, out KeyCode kc) ? kc : KeyCode.None;
        }
    }
}