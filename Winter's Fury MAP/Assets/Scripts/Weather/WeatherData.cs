using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weather
{
    [Serializable]
    public struct WeatherData
    {
        public string name;

        public ParticleSystem particleSystem;

        public float fogDensity;
        public float temperatureImpact;
        public bool cloudsEnabled;
        public Texture2D cloudTexture;

        public float lastsForMin, lastsForMax;
    }
}