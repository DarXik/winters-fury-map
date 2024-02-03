using System;
using UnityEngine;

namespace Weather
{
    [Serializable]
    public struct WeatherData
    {
        public string name;
        public ParticleSystem particleSystem;
        public float temperatureImpact;
        public bool cloudsEnabled;
        
        [Header("Fog")]
        [Range(0f, 1f)] public float fogIntensity;
        [Range(0f, 1f)] public float fogDensity;

        [Space]
        public float lastsForMin;
        public float lastsForMax;
    }
}