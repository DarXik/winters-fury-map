using System;
using Heat;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class AddFuelManager : MonoBehaviour
    {
        [Header("UI References")] 
        [SerializeField] private GameObject addFuelWindow;
        [SerializeField] private TextMeshProUGUI fireDurText, heatOutText;

        public static bool addFuelWindowOpened;
        public static AddFuelManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            addFuelWindow.SetActive(false); 
        }

        public void OpenAddFuelWindow(float fireDuration, float heatOutput)
        {
            addFuelWindow.SetActive(true);
            addFuelWindowOpened = true;
        }
    }
}