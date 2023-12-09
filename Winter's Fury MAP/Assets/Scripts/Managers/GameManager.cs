using System;
using Heat;
using Pinwheel.Jupiter;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public JDayNightCycle cycle;

        public bool autoCycle;
        public bool fog;
        
        public static GameManager Instance;

        private float previousTimeIncrement, timeIncrement;
        public Volume volume;
        private ColorAdjustments ca;
        // OptionsScript optionsScript = new OptionsScript();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cycle.AutoTimeIncrement = autoCycle;
            RenderSettings.fog = fog;
            previousTimeIncrement = cycle.TimeIncrement;

            SetBrightness();
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