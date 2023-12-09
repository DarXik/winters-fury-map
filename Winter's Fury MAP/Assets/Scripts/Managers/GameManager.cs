using Heat;
using Pinwheel.Jupiter;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Main Settings")]
        public JDayNightCycle cycle;
        public bool autoCycle;

        [Header("Spawn Settings")] 
        [SerializeField] private Transform player;
        [SerializeField] private float[] times;
        [SerializeField] private Transform[] spawnPoints;

        private float previousTimeIncrement, timeIncrement;
        public Volume volume;
        private ColorAdjustments ca;
        // OptionsScript optionsScript = new OptionsScript();
        
        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cycle.AutoTimeIncrement = autoCycle;
            previousTimeIncrement = cycle.TimeIncrement;

            SetBrightness();
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            int randomTime = Random.Range(0, times.Length);
            int randomLoc = Random.Range(0, spawnPoints.Length);

            cycle.Time = times[randomTime];

            Vector3 spawnPos = spawnPoints[randomLoc].position;

            player.position = spawnPos;
        }

        private void SetBrightness()
        {
            volume.profile.TryGet(out ca);
            ca.postExposure.value = PlayerPrefs.GetFloat("brightnessPreference");
            Debug.Log("BrightnessPref načtena");
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

                previousTimeIncrement = currentIncrement;
            }
        }

        private void CheckUserInput()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !FirestartManager.fireWindowOpened && !PassTimeManager.passTimeWindowOpened && !AddFuelManager.addFuelWindowOpened)
            {
                InventoryManager.Instance.ToggleInventory();
            }

            if (Input.GetKeyDown(KeyCode.T) && !FirestartManager.fireWindowOpened && !InventoryManager.inventoryOpened && !AddFuelManager.addFuelWindowOpened)
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
    }
}