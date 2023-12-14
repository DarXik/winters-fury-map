using System;
using UnityEngine;

namespace Weather
{
    [Serializable]
    public struct WeatherData
    {
        public string name;

        public ParticleSystem particleSystem;

        public bool foggySkyEnabled;
        
        public float fogDensity;
        public float temperatureImpact;
        public bool cloudsEnabled;

        public float lastsForMin, lastsForMax;
    }
}