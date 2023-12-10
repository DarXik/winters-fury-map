using System.Collections;
using Managers;
using Pinwheel.Jupiter;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weather
{
    public class WeatherSystem : MonoBehaviour
    {
        public JSkyProfile skyProfile;
        
        public WeatherData[] weatherData;
        public static WeatherData selectedWeather;

        public float weatherFadeInTime;
        private float weatherChangeMin, weatherChangeMax;

        private float weatherChange;
        private float weatherTimer;

        public static float timeIncrement;

        private void Awake()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0;
        
            StartCoroutine(SelectWeather(Random.Range(0, weatherData.Length)));
            //StartCoroutine(SelectWeather(7));
        }

        private void Start()
        {
            timeIncrement = GameManager.Instance.GetTimeIncrement();
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
            Debug.Log("Selected weather: " + selectedWeather.name);
        
            var timeElapsed = 0f;
            var previousFogDensity = RenderSettings.fogDensity;

            weatherChangeMin = selectedWeather.lastsForMin;
            weatherChangeMax = selectedWeather.lastsForMax;

            if (selectedWeather.particleSystem != null)
            {
                var particleSystemEmission = selectedWeather.particleSystem.emission;
                particleSystemEmission.enabled = true;
            }

            skyProfile.EnableOverheadCloud = selectedWeather.cloudsEnabled;
            if (selectedWeather.cloudsEnabled) skyProfile.CustomCloudTexture = selectedWeather.cloudTexture;
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
                    var particleSystemEmission = weatherData[i].particleSystem.emission;
                    particleSystemEmission.enabled = false;
                }
            }
        }
    }
}
