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
        public Volume volume;
        private ColorAdjustments ca;

        public KeyCode toggleInventoryKey;
        public KeyCode togglePassTimeKey;

        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
            volume.profile.TryGet(out ca);
        }

        private void Start()
        {
            cycle.AutoTimeIncrement = autoCycle;
            previousTimeIncrement = cycle.TimeIncrement;

            SetBrightness();
            SetKeyPreference("inventoryKey", out toggleInventoryKey);
            SetKeyPreference("passTimeKey", out togglePassTimeKey);

            if (randomizeSpawn) SpawnPlayer();
        }

        private void SpawnPlayer()
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
            var currentIncrement = GetTimeIncrement();
            if (previousTimeIncrement != currentIncrement)
            {
                VitalManager.timeIncrement = currentIncrement;
                HeatSource.timeIncrement = currentIncrement;
                WeatherSystem.timeIncrement = currentIncrement;
                WindArea.timeIncrement = currentIncrement;
                PlayerInteraction.timeIncrement = currentIncrement;
                InventoryManager.timeIncrement = currentIncrement;

                previousTimeIncrement = currentIncrement;
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
            if (Enum.TryParse(inventoryKey, out KeyCode kc))
            {
                desiredKey = kc;
            }
            else
            {
                desiredKey = KeyCode.None;
            }
        }
    }
}