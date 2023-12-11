using System;
using System.Collections;
using System.Linq;
using Managers;
using UnityEngine;
using Weather;
using Random = UnityEngine.Random;

namespace Wind
{
    [Serializable]
    public struct WindTypes
    {
        public string name;
        public float windChill;

        public Vector3 windDirection;
    }

    public class WindArea : MonoBehaviour
    {
        public WindTypes[] windTypes;
        public static WindTypes currentWind;
        private Vector3 windDirection;

        public float windChangeMin, windChangeMax;
        private float windChange, windTimer;

        public static float timeIncrement;

        public static WindArea Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            ChangeWindType();
        }

        private void Start()
        {
            timeIncrement = GameManager.Instance.GetTimeIncrement();
        }

        private void Update()
        {
            if (WeatherSystem.isBlizzard) return;
            
            if (windTimer == 0)
            {
                windChange = Random.Range(windChangeMin, windChangeMax);
            }

            windTimer += Time.deltaTime * timeIncrement;

            if (windTimer >= windChange)
            {
                ChangeWindType();
            }
        }

        public void ChangeWindType(bool blizzard = false)
        {
            if (!blizzard)
            {
                WindTypes[] availableTypes = windTypes.Where(wind => wind.name != "Blizzard").ToArray();

                currentWind = availableTypes[Random.Range(0, availableTypes.Length)];
            }
            else
            {
                currentWind = windTypes.First(wind => wind.name == "Blizzard");
            }
            
            if(currentWind.name == "Calm") windDirection = currentWind.windDirection;
            else windDirection = new Vector3((currentWind.windDirection.x + Random.Range(0f, 2f)) * GetRandomOne(), 0, currentWind.windDirection.z + Random.Range(0f, 2f)) * GetRandomOne();
            
            windTimer = 0;

            Debug.Log("Current wind: " + currentWind.name);
            Debug.Log("Wind direction: " + windDirection);
        }

        public Vector3 GetWindDirection()
        {
            return windDirection;
        }

        public bool IsWindHigh()
        {
            return currentWind.name.Equals("High");
        }

        private int GetRandomOne()
        {
            int randomInt = Random.Range(0, 2);

            return (randomInt == 0) ? -1 : 1;
        }
    }
}