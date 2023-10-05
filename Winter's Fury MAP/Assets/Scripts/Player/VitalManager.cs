using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class VitalManager : MonoBehaviour
    {
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

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            currentHealth = maxHealth;
            currentHunger = maxHunger;
            currentThirst = maxThirst;
        }
    }
}
