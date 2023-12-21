using System.Collections;
using Managers;
using Pinwheel.Jupiter;
using UnityEngine;
using Weather.Wind;
using Random = UnityEngine.Random;

namespace Weather
{
    public class WeatherSystem : MonoBehaviour
    {
        public JSkyProfile skyProfile;
        public JDayNightCycle DNC;
        public JDayNightCycleProfile normalProfile, foggyProfile;
        public Gradient fogGradient;
        
        public WeatherData[] weatherData;
        public static WeatherData selectedWeather;

        public float weatherFadeInTime;
        private float weatherChangeMin, weatherChangeMax;

        private float weatherChange;
        private float weatherTimer;

        public static float timeIncrement;
        public static bool isBlizzard;

        private void Awake()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0;
        }

        private void Start()
        {
            timeIncrement = GameManager.Instance.GetTimeIncrement();
            StartCoroutine(SelectWeather(Random.Range(0, weatherData.Length - 1)));
            //StartCoroutine(SelectWeather(5));
        }

        private void Update()
        {
            if (weatherTimer == 0)
            {
                weatherChange = Random.Range(weatherChangeMin, weatherChangeMax);
            }
        
            weatherTimer += Time.deltaTime * timeIncrement;

            if (weatherTimer >= weatherChange)
            {
                int randomWeather = Random.Range(0, weatherData.Length);

                ResetWeather();
                StartCoroutine(SelectWeather(randomWeather));

                weatherTimer = 0f;
            }
        }

        private IEnumerator SelectWeather(int weatherIndex)
        {
            selectedWeather = weatherData[weatherIndex];

            if (selectedWeather.name.Equals("Blizzard"))
            {
                WindArea.Instance.ChangeWindType(true);
                isBlizzard = true;
            }
            else
            {
                if (isBlizzard)
                {
                    WindArea.Instance.ChangeWindType();
                    isBlizzard = false;
                }
            }
            
            var timeElapsed = 0f;
            var previousFogDensity = RenderSettings.fogDensity;

            weatherChangeMin = selectedWeather.lastsForMin;
            weatherChangeMax = selectedWeather.lastsForMax;

            // Snow, Blizzard
            if (selectedWeather.particleSystem != null)
            {
                var emission = selectedWeather.particleSystem.emission;
                var forceOverLifetime = selectedWeather.particleSystem.forceOverLifetime;
                var windDir = WindArea.Instance.GetWindDirection();
                
                emission.enabled = true;
                forceOverLifetime.enabled = true;
                forceOverLifetime.x = windDir.x;
                forceOverLifetime.y = windDir.y;
                forceOverLifetime.z = windDir.z;
            }

            DNC.Profile = selectedWeather.foggySkyEnabled ? foggyProfile : normalProfile;
            
            // Sunny
            skyProfile.EnableOverheadCloud = selectedWeather.cloudsEnabled;
            skyProfile.UpdateMaterialProperties();
            
            // animate weather changes
            while (timeElapsed < weatherFadeInTime)
            {
                RenderSettings.fogDensity = Mathf.Lerp(previousFogDensity, selectedWeather.fogDensity,
                    timeElapsed / weatherFadeInTime);

                timeElapsed += Time.deltaTime;

                yield return null;
            }

            RenderSettings.fogDensity = selectedWeather.fogDensity;
        }

        private void ResetWeather()
        {
            for (int i = 0; i < weatherData.Length; i++)
            {
                if (weatherData[i].particleSystem != null)
                {
                    var emission = weatherData[i].particleSystem.emission;
                    var forceOverLifetime = weatherData[i].particleSystem.forceOverLifetime;
                    
                    emission.enabled = false;
                    forceOverLifetime.enabled = false;
                }
            }
        }
    }
}
