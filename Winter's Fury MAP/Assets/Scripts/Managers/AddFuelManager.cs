using System;
using System.Collections.Generic;
using Heat;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class AddFuelManager : MonoBehaviour
    {
        [SerializeField] private GameObject UIFuelItem;
        [FormerlySerializedAs("selectedItem")] [SerializeField] private Color selectedColor;

        [Header("UI References")] 
        [SerializeField] private Transform fuelChooser;
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
            DeleteFuelChooserItems();
            
            addFuelWindow.SetActive(true);
            addFuelWindowOpened = true;
            PlayerLook.Instance.BlockRotation();

            var duration = Mathf.RoundToInt(fireDuration * 60f);

            if (duration > 60)
            {
                fireDurText.text = $"{BurnConverter.GetFuelHours(duration)}H {BurnConverter.GetFuelMinutes(duration)}M";
            }
            else
            {
                fireDurText.text = $"{BurnConverter.GetFuelMinutes(duration)}M";
            }
            heatOutText.text = $"{heatOutput}°C";
            
            // create Fuel Items
            var fuelItems = InventoryManager.Instance.GetFuelItems();
            var itemCounts = InventoryManager.Instance.GetItemCounts();
            List<string> usedItems = new();

            foreach (var fuelItem in fuelItems)
            {
                foreach (var itemCount in itemCounts)
                {
                    if (itemCount.Item1 == fuelItem.itemName)
                    {
                        if (!usedItems.Contains(itemCount.Item1))
                        {
                            var uiFuelItem = Instantiate(UIFuelItem, fuelChooser);

                            uiFuelItem.transform.Find("Fuel Name").GetComponent<TextMeshProUGUI>().text = itemCount.Item1;
                            uiFuelItem.transform.Find("Fuel Amount").GetComponent<TextMeshProUGUI>().text =
                                $"1 OF {itemCount.Item2}";
                            
                            usedItems.Add(itemCount.Item1);
                        }
                    }
                }
            }
        }

        public void CloseAddFuelWindow()
        {
            addFuelWindow.SetActive(false);
            addFuelWindowOpened = false;
            PlayerLook.Instance.UnblockRotation();
        }

        private void DeleteFuelChooserItems()
        {
            foreach (Transform item in fuelChooser)
            {
               Destroy(item.gameObject); 
            }
        }
    }
}