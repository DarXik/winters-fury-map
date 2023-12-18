using System;
using Managers;
using Player;
using UnityEngine;

namespace Heat
{
    public class HeatSource : MonoBehaviour
    {
        [SerializeField] private GameObject campFireExtinguished;
        
        [Header("Properties")] 
        [SerializeField] private float heatRange;

        [HideInInspector] public float burnTime;
        [HideInInspector] public float heatOutput;

        private Vector3 playerPos;
        public static float timeIncrement;
        private bool campfireDestroyed;
        private bool burnPlayer;
        
        private void Start()
        {
            timeIncrement = GameManager.Instance.GetTimeIncrement();
        }

        private void Update()
        {
            if (burnTime > 0)
            {
                playerPos = PlayerController.Instance.GetPlayerPosition();
                
                LowerValues();
                HeatPlayer();
            }
            else
            {
                DestroyCampfire();
            }
        }

        private void HeatPlayer()
        {
            TemperatureManager.HeatFromFire = Vector3.Distance(transform.position, playerPos) < heatRange ? heatOutput : 0;
        }

        private void LowerValues()
        {
            burnTime -= Time.deltaTime * timeIncrement;
        }

        private void DestroyCampfire()
        {
            Instantiate(campFireExtinguished, transform.position, Quaternion.identity);

            TemperatureManager.HeatFromFire = 0;
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                VitalManager.burningPlayer = true;
                StartCoroutine(VitalManager.Instance.BurnPlayer());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                VitalManager.burningPlayer = false;
            }
        }
    }
}