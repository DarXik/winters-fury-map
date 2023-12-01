﻿using System;
using Managers;
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
                DamagePlayerIfClose();
            }
            else
            {
                DestroyCampfire();
            }
        }

        private void HeatPlayer()
        {
            if (VitalManager.Instance.tempFromFire > 0) VitalManager.Instance.tempFromFire = 0;

            if (Vector3.Distance(transform.position, playerPos) > heatRange) return;

            VitalManager.Instance.tempFromFire = heatOutput;
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

        private void DamagePlayerIfClose()
        {
            if (Physics.Raycast(transform.position, transform.up, 5f, LayerMask.GetMask("Player")))
            {
                VitalManager.Instance.BurnPlayer();
            }
        }
    }
}