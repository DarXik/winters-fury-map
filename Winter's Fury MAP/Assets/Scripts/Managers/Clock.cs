using System;
using UnityEngine;

namespace Managers
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private RectTransform dome;
        [SerializeField] private RectTransform uiSun, uiMoon;
        
        public static Clock Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void RotateClock()
        {
            uiSun.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            uiMoon.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            
            float rotation = 180f / (24f * .5f);
        
            dome.Rotate(0,0, -(Time.deltaTime * GameManager.Instance.GetTimeIncrement()) * rotation);
        }

        public void SetClock()
        {
            float rotation = CalculateStartRotation(GameManager.Instance.GetCurrentTime());

            dome.rotation = Quaternion.Euler(0f, 0f, rotation);
            uiSun.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            uiMoon.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
        
        private float CalculateStartRotation(float time)
        {
            float rotation = Mathf.Lerp(180f, -180f, time / 24f);

            return rotation;
        }
    }
}