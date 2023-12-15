using System;
using Pinwheel.Jupiter;
using Player;
using UnityEngine;
using Weather;
using Weather.Wind;

namespace Managers
{
    public class TemperatureManager : MonoBehaviour
    {
        public float[] hourlyTemperatures = new float[24];

        private float timeOfDay;

        public float AmbientTemperature { get; private set; }
        public static float IndoorTemperature { get; set; }
        public static float HeatFromFire
        {
            private get;
            set;
        }
        public float FeelsLike => AmbientTemperature - WindArea.Instance.GetWindChill();
        
        public static TemperatureManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            timeOfDay = GameManager.Instance.cycle.Time;

            int currentHour = Mathf.FloorToInt(timeOfDay);

            int nextHour = (currentHour + 1) % 24;

            float currentTemp = hourlyTemperatures[currentHour];
            
            float nextTemp = hourlyTemperatures[nextHour];

            float normalizedTime = timeOfDay - currentHour;
            AmbientTemperature = Mathf.Lerp(currentTemp, nextTemp, normalizedTime);
            AmbientTemperature -= WeatherSystem.selectedWeather.temperatureImpact;
            AmbientTemperature += IndoorTemperature;
            AmbientTemperature += HeatFromFire;
        }
    }
}