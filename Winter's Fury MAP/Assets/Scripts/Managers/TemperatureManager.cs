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

            if (PlayerInteraction.equippedItem != null && PlayerInteraction.equippedItem.isLit) ambientTemperature += PlayerInteraction.equippedItem.heatBonus;
            ambientTemperature -= WindArea.currentWind.windChill;
            ambientTemperature -= WeatherSystem.selectedWeather.temperatureImpact;
            ambientTemperature += indoorTemperature;
        }
    }
}