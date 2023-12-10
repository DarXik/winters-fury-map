using System;
using Pinwheel.Jupiter;
using UnityEngine;
using Weather;

namespace Managers
{
    public class TemperatureManager : MonoBehaviour
    {
        public float[] hourlyTemperatures = new float[24];

        private float timeOfDay;

        public static float ambientTemperature;
        public static float indoorTemperature;

        private void Update()
        {
            timeOfDay = GameManager.Instance.cycle.Time;

            int currentHour = Mathf.FloorToInt(timeOfDay);

            int nextHour = (currentHour + 1) % 24;

            float currentTemp = hourlyTemperatures[currentHour];
            
            float nextTemp = hourlyTemperatures[nextHour];

            float normalizedTime = timeOfDay - currentHour;
            ambientTemperature = Mathf.Lerp(currentTemp, nextTemp, normalizedTime);

            ambientTemperature -= WeatherSystem.selectedWeather.temperatureImpact;
            ambientTemperature += indoorTemperature;
        }
    }
}