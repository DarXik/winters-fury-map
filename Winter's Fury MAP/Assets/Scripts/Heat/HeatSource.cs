using System;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Heat
{
    public class HeatSource : MonoBehaviour
    {
        [SerializeField] private GameObject campFireExtinguished;
        
        [Header("Properties")] 
        [SerializeField] private float heatRange;

        // hide in inspector
        public float burnTime;
        public float temperature;

        private Vector3 playerPos;
        public static float timeIncrement;
        private bool campfireDestroyed;
        
        private void Start()
        {
            playerPos = PlayerController.Instance.GetPlayerPosition();

            timeIncrement = GameManager.Instance.GetTimeIncrement();
        }

        private void Update()
        {
            if (burnTime > 0)
            {
                LowerValues();
            }
            else
            {
                DestroyCampfire();
            }
        }

        private void LowerValues()
        {
            burnTime -= Time.deltaTime * timeIncrement;
        }

        private void DestroyCampfire()
        {
            Instantiate(campFireExtinguished, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }
}