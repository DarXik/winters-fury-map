using Pinwheel.Jupiter;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class VitalManager : MonoBehaviour
    {
        // Skybox Reference
        public JDayNightCycle skybox;
        
        [Header("UI References")] 
        public Slider healthBar;
        public Slider hungerBar;
        public Slider thirstBar;
    
        [Header("Health")] 
        public float maxHealth;
        private float currentHealth;
        
        [Header("Temperature")]

        [Header("Hunger (Calories)")] 
        public float maxCalories;
        public float standingBurnRate, walkingBurnRate, runningBurnRate;
        private float currentCalories;
        private PlayerActivity currentActivity;

        [Header("Thirst (mL)")] 
        public float maxThirst;
        public float awakeDepletionRate, asleepDepletionRate;
        private float currentThirst;
        private PlayerAwakeness currentAwakeness;
    
        // Fill Amounts
        private float HealthPercent => currentHealth / maxHealth;
        private float HungerPercent => currentCalories / maxCalories;
        private float ThirstPercent => currentThirst / maxThirst;

        public static VitalManager Instance;
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

            timeIncrement = skybox.TimeIncrement;
            currentAwakeness = PlayerController.Instance.currentAwakeness;
        }

        private void Update()
        {
            ReduceHunger();
            ReduceThirst();

            HandleUI();

            currentActivity = PlayerController.Instance.currentActivity;
        }

        private void ReduceHunger()
        {
            if (currentCalories <= 0) return;

            switch (currentActivity)
            {
                case PlayerActivity.Standing:
                    currentCalories -= standingBurnRate * (Time.deltaTime * timeIncrement);
                    break;
                case PlayerActivity.Walking:
                    currentCalories -= walkingBurnRate * (Time.deltaTime * timeIncrement);
                    break;
                case PlayerActivity.Running:
                    currentCalories -= runningBurnRate * (Time.deltaTime * timeIncrement);
                    break;
            }
        }

        private void ReduceThirst()
        {
            if (currentThirst <= 0) return;

            switch (currentAwakeness)
            {
                case PlayerAwakeness.Awake:
                    currentThirst -= awakeDepletionRate * (Time.deltaTime * timeIncrement);
                    break;
                case PlayerAwakeness.Asleep:
                    currentThirst -= asleepDepletionRate * (Time.deltaTime * timeIncrement);
                    break;
            }
        }

        private void HandleUI()
        {
            
        }
    }
}
