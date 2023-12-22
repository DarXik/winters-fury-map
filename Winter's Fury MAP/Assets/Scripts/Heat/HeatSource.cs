using System;
using System.Collections;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Weather.Wind;
using Random = UnityEngine.Random;

namespace Heat
{
    public class HeatSource : MonoBehaviour
    {
        [SerializeField] private GameObject campFireExtinguished;
        
        [Header("Properties")] 
        [SerializeField] private float heatRange;

        [HideInInspector] public float burnTime;
        private float heatOutput;
        public float HeatOutput
        {
            get => heatOutput;
            set => heatOutput = Mathf.Clamp(value, 0, 80);
        }

        private Vector3 playerPos;
        public static float timeIncrement;
        private bool campfireDestroyed;
        private bool burnPlayer;
        private bool windblowingFire;
        
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

                if (WindArea.Instance.IsWindHigh() && !windblowingFire && !PlayerController.isWindProtected)
                {
                    windblowingFire = true;

                    StartCoroutine(WindBlowFire());
                }
                else if (!WindArea.Instance.IsWindHigh() && windblowingFire)
                {
                    windblowingFire = false;
                }
            }
            else
            {
                DestroyCampfire();
            }
        }

        private IEnumerator WindBlowFire()
        {
            while (windblowingFire)
            {
                float chance = Mathf.Round(Random.value * 100);

                if (chance <= 50f)
                {
                    windblowingFire = false;
                    
                    DestroyCampfire();
                }

                yield return new WaitForSeconds(10f);
            }
        }

        private void HeatPlayer()
        {
            TemperatureManager.HeatFromFire = Vector3.Distance(transform.position, playerPos) < heatRange ? HeatOutput : 0;
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