using System;
using Pinwheel.Jupiter;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public JDayNightCycle cycle;

        public bool autoCycle = true;
        
        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cycle.AutoTimeIncrement = autoCycle;
        }
    }
}