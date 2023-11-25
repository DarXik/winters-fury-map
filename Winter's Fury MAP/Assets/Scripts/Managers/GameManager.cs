using Heat;
using Pinwheel.Jupiter;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public JDayNightCycle cycle;

        public bool autoCycle;
        public bool fog;
        
        public static GameManager Instance;

        private float previousTimeIncrement, timeIncrement;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cycle.AutoTimeIncrement = autoCycle;
            RenderSettings.fog = fog;

            previousTimeIncrement = cycle.TimeIncrement;
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
            if (Input.GetKeyDown(KeyCode.I) && !FirestartManager.fireWindowOpened && !PassTimeManager.passTimeWindowOpened)
            {
                InventoryManager.Instance.ToggleInventory();
            }

            if (Input.GetKeyDown(KeyCode.T) && !FirestartManager.fireWindowOpened && !InventoryManager.inventoryOpened)
            {
                PassTimeManager.Instance.TogglePassTimeWindow();
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