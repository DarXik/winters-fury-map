using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class AddFuelManager : MonoBehaviour
    {
        [SerializeField] private GameObject UIFuelItem;
        [SerializeField] private Color selectedColor;

        [Header("UI References")] [SerializeField]
        private Transform fuelChooser;

        [SerializeField] private GameObject addFuelWindow;
        [SerializeField] private TextMeshProUGUI fireDurText, heatOutText;

        private List<ItemData> usedFuelItems;
        private int? chosenFuelIndex = null;

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

        private void ChooseFuelItem(int? index)
        {
            for (int i = 0; i < fuelChooser.childCount; i++)
            {
                Image itemImage = fuelChooser.GetChild(i).GetComponent<Image>();

                if (i == index)
                {
                    itemImage.color = selectedColor;
                    chosenFuelIndex = i;
                }
                else
                {
                    itemImage.color = new Color32(255, 255, 255, 0);
                }
            }
        }

        public void OpenAddFuelWindow(float fireDuration, float heatOutput, Transform campfire)
        {
            DeleteFuelChooserItems();
            addFuelWindow.SetActive(true);
            addFuelWindowOpened = true;
            PlayerLook.Instance.BlockRotation();
            StartCoroutine(PlayerLook.Instance.LookAt(campfire));

            var duration = Mathf.RoundToInt(fireDuration * 60f);

            if (duration >= 60)
            {
                fireDurText.text = $"{BurnConverter.GetFuelHours(duration)}H {BurnConverter.GetFuelMinutes(duration)}M";
            }
            else
            {
                fireDurText.text = $"{BurnConverter.GetFuelMinutes(duration)}M";
            }

            heatOutText.text = $"{heatOutput}°C";

            CreateUIFuelItems();
        }

        public void AddFuel()
        {
            int index = Convert.ToInt32(chosenFuelIndex);
            var chosenFuelItem = usedFuelItems[index];

            // add fuel item into HeatSource
            PlayerInteraction.interactedCampfire.burnTime += chosenFuelItem.burnTime / 60f;
            PlayerInteraction.interactedCampfire.heatOutput += chosenFuelItem.temperatureIncrease;

            // update UI text
            var duration = Mathf.RoundToInt(PlayerInteraction.interactedCampfire.burnTime * 60f);

            if (duration >= 60)
            {
                fireDurText.text = $"{BurnConverter.GetFuelHours(duration)}H {BurnConverter.GetFuelMinutes(duration)}M";
            }
            else
            {
                fireDurText.text = $"{BurnConverter.GetFuelMinutes(duration)}M";
            }

            heatOutText.text = $"{PlayerInteraction.interactedCampfire.heatOutput}°C";

            // delete fuel item and update UI
            InventoryManager.Instance.DeleteItem(chosenFuelItem);
            DeleteFuelChooserItems();
            CreateUIFuelItems();
        }

        private void CreateUIFuelItems()
        {
            var fuelItems = InventoryManager.Instance.GetFuelItems();
            var itemCounts = InventoryManager.Instance.GetFuelItemCounts();
            usedFuelItems = new();
            var index = 0;
            
            foreach (var fuelItem in fuelItems)
            {
                var matchingItem = itemCounts.FirstOrDefault(itemCount => itemCount.Item1 == fuelItem.itemName && usedFuelItems.All(u => u.itemName != fuelItem.itemName));

                if (matchingItem != null)
                {
                    Debug.Log("Creating UI Item for: " + fuelItem.itemName);
                    var i = index;

                    var uiFuelItem = Instantiate(UIFuelItem, fuelChooser);

                    uiFuelItem.transform.Find("Fuel Name").GetComponent<TextMeshProUGUI>().text =
                        matchingItem.Item1;
                    uiFuelItem.transform.Find("Fuel Amount").GetComponent<TextMeshProUGUI>().text =
                        $"1 OF {matchingItem.Item2}";
                    uiFuelItem.GetComponent<Button>().onClick.AddListener(delegate { ChooseFuelItem(i); });

                    usedFuelItems.Add(fuelItem);
                    index++;
                }
            }
            ChooseFuelItem(chosenFuelIndex ?? 0);
        }

        public void CloseAddFuelWindow()
        {
            addFuelWindow.SetActive(false);
            addFuelWindowOpened = false;
            PlayerLook.Instance.UnblockRotation();
            chosenFuelIndex = null;
        }

        private void DeleteFuelChooserItems()
        {
            foreach (Transform item in fuelChooser)
            {
                Destroy(item.gameObject);
            }

            fuelChooser.DetachChildren();
        }
    }
}