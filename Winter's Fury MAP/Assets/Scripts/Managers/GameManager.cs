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

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cycle.AutoTimeIncrement = autoCycle;
            RenderSettings.fog = fog;
        }

        private void Update()
        {
            CheckUserInput();
        }

        private void CheckUserInput()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                InventoryManager.Instance.ToggleInventory();
            }

            if (Input.GetKeyDown(KeyCode.T))
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
    }
}