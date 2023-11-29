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
        [SerializeField] private Color selectedColor;

        [Header("UI References")] 
        [SerializeField] private Transform fuelChooser;
        [SerializeField] private GameObject addFuelWindow;
        [SerializeField] private TextMeshProUGUI fireDurText, heatOutText;

        private int chosenFuelIndex;

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

        private void ChooseFuelItem(int index)
        {
            for (int i = 0; i < fuelChooser.childCount; i++)
            {
                Image itemImage = fuelChooser.GetChild(i).GetComponent<Image>();
                
                if (i == index)
                {
                    itemImage.color = selectedColor;
                }
                else
                {
                    itemImage.color = new Color32(255, 255, 255, 0);
                }
            }
        }

        public void OpenAddFuelWindow(float fireDuration, float heatOutput)
        {
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
                for (var i = 0; i < itemCounts.Count; i++)
                {
                    var itemCount = itemCounts[i];
                    var index = i;
                    
                    if (itemCount.Item1 == fuelItem.itemName)
                    {
                        if (!usedItems.Contains(itemCount.Item1))
                        {
                            var uiFuelItem = Instantiate(UIFuelItem, fuelChooser);

                            uiFuelItem.transform.Find("Fuel Name").GetComponent<TextMeshProUGUI>().text =
                                itemCount.Item1;
                            uiFuelItem.transform.Find("Fuel Amount").GetComponent<TextMeshProUGUI>().text =
                                $"1 OF {itemCount.Item2}";
                            uiFuelItem.GetComponent<Button>().onClick.AddListener(delegate
                            {
                                ChooseFuelItem(index);
                            });

                            usedItems.Add(itemCount.Item1);
                        }
                    }
                }
            }
            
            // choose immediately the first item
            ChooseFuelItem(0);
        }

        public void CloseAddFuelWindow()
        {
            DeleteFuelChooserItems();
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