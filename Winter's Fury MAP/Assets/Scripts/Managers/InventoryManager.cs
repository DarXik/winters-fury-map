using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Inventory;
using Player;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Managers
{
    public class InventoryManager : MonoBehaviour
    {
        public List<ItemData> items = new();

        [Header("Core Values")] public float maxWeight;
        [SerializeField] private Gradient weightGradient;
        [HideInInspector] public float currentWeight;

        [Header("UI References")] [SerializeField]
        private GameObject inventory;

        [SerializeField] private GameObject inventoryUIItem;
        [SerializeField] private Transform itemContent;
        [SerializeField] private GameObject itemDetail;
        [SerializeField] private Slider weightBar;
        [SerializeField] private Image weightBarFill;
        [SerializeField] private TextMeshProUGUI weightValues;

        [Header("Inventory Filter")] [SerializeField]
        private TextMeshProUGUI inventoryFilterName;

        [SerializeField] private Button filterAll, filterFirstAid, filterFuelSource, filterFood, filterTools;

        [Header("Item Detail References")] [SerializeField]
        private TextMeshProUGUI itemName;

        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private Image itemIcon;
        [SerializeField] private GameObject needs;
        [SerializeField] private GameObject detailNeedItem;
        [SerializeField] private TextMeshProUGUI itemConditionText;
        [SerializeField] private Gradient conditionGradient;
        [SerializeField] private TextMeshProUGUI itemWeight;
        [SerializeField] private Button dropItemButton;
        [SerializeField] private Sprite stomach, water;
        [SerializeField] private GameObject actionButtonObj;
        [SerializeField] private Button actionBtn;

        [Header("Drop Window References")] [SerializeField]
        private GameObject dropItemWindow;

        [SerializeField] private TextMeshProUGUI alertHeader;
        [SerializeField] private Slider dropItemSlider;
        [SerializeField] private TextMeshProUGUI dropCounterText;
        [SerializeField] private Button dropSelectedButton;
        [SerializeField] private Button dropAllButton;

        private List<Tuple<string, int, float>> itemCounts;
        public static float timeIncrement;
        public static bool inventoryOpened;
        private string currentDetailedItem, currentFilter;
        private float currentDetailedCondition;

        public static InventoryManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            timeIncrement = GameManager.Instance.cycle.TimeIncrement;

            inventory.SetActive(false);
            HideItemDetail();
            dropItemWindow.SetActive(false);
            actionButtonObj.SetActive(false);

            weightBar.maxValue = maxWeight;
            weightBar.value = 0;
        }

        private void Update()
        {
            if (items.Count > 0) ReduceItemsCondition();
        }

        public void ToggleInventory()
        {
            if (inventoryOpened)
            {
                // Close inventory
                inventoryOpened = false;

                PlayerLook.Instance.UnblockRotation();
                inventory.SetActive(false);
                HideItemDetail();
            }
            else
            {
                // Open inventory
                inventoryOpened = true;
                DisplayFilterUI("All");
                InventoryUI.Instance.UpdateConditionUI();

                if (PlayerInteraction.equippedItem != null) UpdateItemData(PlayerInteraction.equippedItem);

                PlayerLook.Instance.BlockRotation();
                inventory.SetActive(true);

                ListItems();
            }
        }

        public void AddItem(ItemData itemData)
        {
            items.Add(itemData);
        }

        public void DisplayFilterUI(string filter)
        {
            inventoryFilterName.text = filter;

            switch (filter)
            {
                case "All":
                    filterAll.interactable = false;
                    filterFirstAid.interactable = true;
                    filterFuelSource.interactable = true;
                    filterFood.interactable = true;
                    filterTools.interactable = true;
                    break;
                case "First Aid":
                    filterAll.interactable = true;
                    filterFirstAid.interactable = false;
                    filterFuelSource.interactable = true;
                    filterFood.interactable = true;
                    filterTools.interactable = true;
                    break;
                case "Fuel":
                    filterAll.interactable = true;
                    filterFirstAid.interactable = true;
                    filterFuelSource.interactable = false;
                    filterFood.interactable = true;
                    filterTools.interactable = true;
                    break;
                case "Food and Drink":
                    filterAll.interactable = true;
                    filterFirstAid.interactable = true;
                    filterFuelSource.interactable = true;
                    filterFood.interactable = false;
                    filterTools.interactable = true;
                    break;
                case "Tools":
                    filterAll.interactable = true;
                    filterFirstAid.interactable = true;
                    filterFuelSource.interactable = true;
                    filterFood.interactable = true;
                    filterTools.interactable = false;
                    break;
            }
        }

        public void ListItems(string filter = "")
        {
            currentFilter = filter;

            DeleteInventoryContents();
            DeleteNeedContents();
            HideItemDetail();
            currentWeight = 0f;

            itemCounts = new List<Tuple<string, int, float>>();
            List<Sprite> itemIcons = new();

            foreach (var item in items)
            {
                currentWeight += item.ItemWeight;
                
                if (filter != "" && item.itemType.ToString() != filter)
                {
                    continue;
                }

                if (itemCounts.Any(tuple => tuple.Item1 == item.itemName))
                {
                    var existingTuple =
                        itemCounts.FirstOrDefault(tuple =>
                            tuple.Item1 == item.itemName && tuple.Item3 == Mathf.Round(item.itemCondition));

                    if (existingTuple != null)
                    {
                        var index = itemCounts.IndexOf(existingTuple);
                        var updatedTuple = Tuple.Create(existingTuple.Item1, existingTuple.Item2 + 1,
                            existingTuple.Item3);
                        itemCounts[index] = updatedTuple;
                    }
                    else
                    {
                        itemCounts.Add(new Tuple<string, int, float>(item.itemName, 1,
                            Mathf.Round(item.itemCondition)));
                        itemIcons.Add(item.itemIcon);
                    }
                }
                else
                {
                    itemCounts.Add(new Tuple<string, int, float>(item.itemName, 1, Mathf.Round(item.itemCondition)));
                    itemIcons.Add(item.itemIcon);
                }
            }

            weightBar.value = currentWeight;
            weightBarFill.color = weightGradient.Evaluate(weightBar.value / weightBar.maxValue);

            int num = 0;

            foreach (var count in itemCounts)
            {
                GameObject obj = Instantiate(inventoryUIItem, itemContent);
                var itemCount = obj.transform.Find("ItemInfo").Find("ItemCount").GetComponent<TextMeshProUGUI>();
                var itemConditionText =
                    obj.transform.Find("ItemInfo").Find("ItemCondition").GetComponent<TextMeshProUGUI>();
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                obj.GetComponent<Button>().onClick.AddListener(delegate { ShowItemDetail(count.Item1, count.Item3); });

                itemCount.text = "x" + count.Item2;
                itemConditionText.text = count.Item3 + "%";
                itemConditionText.color = conditionGradient.Evaluate(count.Item3 / 100f);
                itemIcon.sprite = itemIcons[num];
                itemIcon.preserveAspect = true;

                num++;
            }
        }

        private void ShowItemDetail(string itemName, float itemCondition)
        {
            DeleteNeedContents();

            itemDetail.SetActive(true);

            dropItemSlider.onValueChanged.RemoveAllListeners();
            dropItemButton.onClick.RemoveAllListeners();
            dropSelectedButton.onClick.RemoveAllListeners();
            dropAllButton.onClick.RemoveAllListeners();
            actionBtn.onClick.RemoveAllListeners();

            // Loop through every item and find a match according to the parameter
            foreach (var item in items)
            {
                if (item.itemName == itemName && Mathf.RoundToInt(item.itemCondition) == itemCondition)
                {
                    this.itemName.text = item.itemName;
                    itemDescription.text = item.itemDescription;
                    itemIcon.sprite = item.itemIcon;
                    itemIcon.preserveAspect = true;
                    itemConditionText.text = Mathf.Round(item.itemCondition) + "%";
                    itemConditionText.color = conditionGradient.Evaluate(item.itemCondition / 100f);

                    var totalWeight = items.Where(thisItem => thisItem.itemName == item.itemName)
                        .Sum(thisItem => thisItem.ItemWeight);
                    var totalWaterIntake = items.Where(thisItem => thisItem.itemName == item.itemName)
                        .Sum(thisItem => thisItem.waterIntake);

                    var itemCountValue = 0;

                    // Loop through every item count to find how many items we have
                    foreach (var count in itemCounts.Where(count => count.Item1 == itemName))
                    {
                        itemCountValue = count.Item2;
                    }

                    itemWeight.text = totalWeight.ToString(CultureInfo.InvariantCulture) + "KG";

                    dropItemButton.onClick.AddListener(() => OpenDropWindow(itemName));

                    switch (item.itemType)
                    {
                        case ItemType.FoodAndDrink:
                            var needItem = Instantiate(detailNeedItem, needs.transform);
                            if (item.calorieDensity == 0)
                            {
                                needItem.transform.Find("Image").GetComponent<Image>().sprite = water;
                                needItem.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
                                    $"{(totalWaterIntake / 1000).ToString("F2", CultureInfo.InvariantCulture)} L";

                                actionButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Drink";
                                actionBtn.onClick.AddListener(() =>
                                {
                                    TryDrink(item.waterIntake, item.caloriesIntake, item);
                                });
                            }
                            else
                            {
                                needItem.transform.Find("Image").GetComponent<Image>().sprite = stomach;
                                needItem.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
                                    item.caloriesIntake.ToString(CultureInfo.InvariantCulture);

                                actionButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Eat";
                                actionBtn.onClick.AddListener(() => { TryEat(item.caloriesIntake, item); });
                            }

                            actionButtonObj.SetActive(true);

                            break;
                        case ItemType.Fuelsource:
                            actionButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Start fire";
                            actionButtonObj.SetActive(true);
                            actionBtn.onClick.AddListener(() => { TryStartFire(item, itemCountValue); });
                            break;
                        case ItemType.Tool:
                            if (PlayerInteraction.equippedItem != null &&
                                PlayerInteraction.equippedItem.itemName == item.itemName)
                            {
                                actionButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
                                actionButtonObj.SetActive(true);
                                actionBtn.onClick.AddListener(() => { PlayerInteraction.Instance.UnEquipTool(); });
                            }
                            else
                            {
                                actionButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                                actionButtonObj.SetActive(true);
                                actionBtn.onClick.AddListener(() => { PlayerInteraction.Instance.EquipTool(item); });
                            }

                            break;
                        case ItemType.FirstAid:
                            actionButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
                            actionButtonObj.SetActive(true);
                            actionBtn.onClick.AddListener(() =>
                            {
                                InventoryUI.Instance.DisplayTreatmentChooser(item, itemCountValue);
                            });

                            break;
                    }

                    break;
                }
            }
        }

        private void TryStartFire(ItemData itemData, int itemCount)
        {
            ToggleInventory();
            FirestartManager.Instance.OpenFireStartWindow(itemData, itemCount);
        }

        private void TryDrink(float waterIntake, float calorieIntake, ItemData itemData)
        {
            var returnedWater = VitalManager.Instance.AddThirst(waterIntake);
            var calories = 0f;

            if (calorieIntake > 0)
            {
                var intakeRatio = (waterIntake - returnedWater) / calorieIntake;

                calories = calorieIntake - (calorieIntake * intakeRatio);
                VitalManager.Instance.AddHunger(calorieIntake * intakeRatio);
            }

            if (returnedWater == 0)
            {
                DeleteItem(itemData);

                ListItems(currentFilter);
            }
            else
            {
                var itemIndex = items.IndexOf(itemData);

                items[itemIndex].waterIntake = returnedWater;
                items[itemIndex].caloriesIntake = calories;

                ListItems(currentFilter);
                ShowItemDetail(itemData.itemName, Mathf.Round(itemData.itemCondition));
            }
        }

        private void TryEat(float caloriesIntake, ItemData itemData)
        {
            var returnedCalories = VitalManager.Instance.AddHunger(caloriesIntake);

            if (itemData.inflictsAffliction)
            {
                var chance = Mathf.Round(Random.value * 100);

                if (chance <= itemData.afflictionChance)
                {
                    VitalManager.Instance.InflictAffliction(itemData.affliction);
                }
            }

            if (returnedCalories == 0)
            {
                DeleteItem(itemData);

                ListItems(currentFilter);
            }
            else
            {
                var itemIndex = items.IndexOf(itemData);

                items[itemIndex].caloriesIntake = returnedCalories;

                ListItems(currentFilter);
                ShowItemDetail(itemData.itemName, Mathf.Round(itemData.itemCondition));
            }
        }

        private void OpenDropWindow(string itemName)
        {
            dropItemWindow.SetActive(true);

            alertHeader.text = $"Discard item: {itemName}";
            dropCounterText.text = "1x";

            Debug.Log("Opening drop window for: " + itemName);

            // Loop through the items in dictionary
            foreach (var count in itemCounts)
            {
                if (count.Item1 == itemName)
                {
                    currentDetailedItem = itemName;
                    currentDetailedCondition = count.Item3;

                    dropItemSlider.value = 1;
                    dropItemSlider.minValue = 1;
                    dropItemSlider.maxValue = count.Item2;
                    dropItemSlider.onValueChanged.AddListener(delegate
                    {
                        UpdateDropCounterText(dropItemSlider.value);
                    });

                    dropSelectedButton.onClick.AddListener(() => { DropSelected(itemName, dropItemSlider.value); });
                    dropAllButton.onClick.AddListener(() => DropAll(itemName));

                    break;
                }
            }
        }

        public void CloseDropWindow()
        {
            dropItemWindow.SetActive(false);
            ListItems();

            ShowItemDetail(currentDetailedItem, currentDetailedCondition);
        }

        public void HideItemDetail()
        {
            itemDetail.SetActive(false);
        }

        // Drop selected according to the slider value
        private void DropSelected(string itemName, float countToDrop)
        {
            Debug.Log("Dropping selected count for: " + itemName);
            Debug.Log("Count is: " + countToDrop);

            var playerPos = PlayerController.Instance.GetPlayerPosition();
            float offset = 0f;
            float totalCount = 0f;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];

                if (item.itemName == itemName && totalCount < countToDrop)
                {
                    if (Physics.Raycast(playerPos + new Vector3(0, 0, offset), Vector3.down, out var hit, 10f))
                    {
                        var droppedItem = Instantiate(item.itemObj, hit.point, item.itemObj.transform.rotation);

                        droppedItem.GetComponent<ItemController>().itemData = item;

                        offset += droppedItem.transform.localScale.x / 2f;
                    }

                    currentWeight -= item.ItemWeight;

                    if (item == PlayerInteraction.equippedItem)
                    {
                        PlayerInteraction.Instance.UnEquipTool();
                    }

                    items.RemoveAt(i);
                    totalCount++;
                }
            }

            dropItemWindow.SetActive(false);
            ListItems();
        }

        // Drop all items of same name
        private void DropAll(string itemName)
        {
            Debug.Log("Dropping all items of: " + itemName);

            var playerPos = PlayerController.Instance.GetPlayerPosition();
            float offset = 0f;

            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item.itemName == itemName)
                {
                    if (Physics.Raycast(playerPos + new Vector3(0, 0, offset), Vector3.down, out var hit, 10f))
                    {
                        var droppedItem = Instantiate(item.itemObj, hit.point, item.itemObj.transform.rotation);

                        // assign itemData from inventory to replace the new one
                        droppedItem.GetComponent<ItemController>().itemData = item;

                        offset += droppedItem.transform.localScale.x / 2f;
                    }

                    currentWeight -= item.ItemWeight;

                    if (item == PlayerInteraction.equippedItem)
                    {
                        PlayerInteraction.Instance.UnEquipTool();
                    }

                    items.RemoveAt(i);
                }
            }

            dropItemWindow.SetActive(false);
            ListItems();
        }

        private void UpdateDropCounterText(float sliderValue)
        {
            dropCounterText.text = $"{sliderValue}x";
        }

        private void DeleteInventoryContents()
        {
            foreach (Transform content in itemContent)
            {
                Destroy(content.gameObject);
            }
        }

        private void DeleteNeedContents()
        {
            foreach (Transform needItem in needs.transform)
            {
                Destroy(needItem.gameObject);
            }
        }

        private void ReduceItemsCondition()
        {
            foreach (var item in items)
            {
                if (item.itemCondition <= 0f) continue;

                item.itemCondition -= (item.conditionPerDay / 24f) * (Time.deltaTime * timeIncrement);
            }
        }

        public void UpdateWeightValues()
        {
            weightValues.text = currentWeight.ToString("0.0", CultureInfo.InvariantCulture) + " / " +
                                maxWeight.ToString("0.0", CultureInfo.InvariantCulture) + " kg";
        }

        public void DeleteItem(ItemData itemToDelete)
        {
            var itemIndex = items.IndexOf(itemToDelete);

            if (itemIndex == -1) return;

            items.RemoveAt(itemIndex);

            ListItems();
            HideItemDetail();
        }

        public void DeleteItemByName(string itemName)
        {
            var index = items.FindIndex(item => item.itemName == itemName);

            items.RemoveAt(index);

            ListItems(currentFilter);
            HideItemDetail();
        }

        public List<ItemData> GetFuelItems()
        {
            return items.FindAll(item => item.itemType == ItemType.Fuelsource);
        }


        public List<Tuple<string, int, float>> GetItemCounts()
        {
            return itemCounts;
        }

        public List<Tuple<string, int, float>> GetFuelItemCounts()
        {
            return items.Where(item => item.itemType == ItemType.Fuelsource)
                .Select(item => itemCounts.Find(tuple => tuple.Item1 == item.itemName)).ToList();
        }

        public void UpdateItemData(ItemData itemToEdit)
        {
            items.FirstOrDefault(item => item.itemName == itemToEdit.itemName)!.itemCondition =
                itemToEdit.itemCondition;
        }

        public string GetCurrentFilter()
        {
            return currentFilter;
        }
    }
}