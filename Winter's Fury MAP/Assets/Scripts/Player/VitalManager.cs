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

        [Header("Hunger")] 
        public float maxHunger;
        public float hungerDepletionRate;
        private float currentHunger;

        [Header("Thirst")] 
        public float maxThirst;
        public float thirstDepletionRate;
        private float currentThirst;
    
        // Fill Amounts
        private float HealthPercent => currentHealth / maxHealth;
        private float HungerPercent => currentHunger / maxHunger;
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
            currentHunger = maxHunger;
            currentThirst = maxThirst;

            timeIncrement = skybox.TimeIncrement;
        }

        private void Update()
        {
            ReduceHunger();
            ReduceThirst();

            HandleUI();
        }

        private void ReduceHunger()
        {
            if (currentHunger <= 0) return;

            currentHunger -= hungerDepletionRate * (Time.deltaTime * timeIncrement);
        }

        private void ReduceThirst()
        {
            if (currentThirst <= 0) return;

            currentThirst -= thirstDepletionRate * (Time.deltaTime * timeIncrement);
        }

        private void HandleUI()
        {
            
        }
    }
}
