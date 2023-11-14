using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Player;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private List<ItemData> items = new();

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

        [Header("Item Detail References")] [SerializeField]
        private TextMeshProUGUI itemName;

        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private Image itemIcon;
        [SerializeField] private GameObject needs;
        [SerializeField] private GameObject detailNeedItem;
        [SerializeField] private TextMeshProUGUI itemCondition;
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

        private Dictionary<string, List<Tuple<int, float>>> itemCounts;
        private float timeIncrement;
        private bool inventoryOpened;
        private string currentDetailedItem;

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

                PlayerLook.Instance.BlockRotation();
                inventory.SetActive(true);

                ListItems();
            }
        }

        public void AddItem(ItemData itemData)
        {
            items.Add(itemData);
        }

        private void ListItems()
        {
            DeleteInventoryContents();
            DeleteNeedContents();
            currentWeight = 0f;

            HideItemDetail();

            itemCounts = new Dictionary<string, List<Tuple<int, float>>>();
            List<Sprite> itemIcons = new List<Sprite>();

            foreach (var item in items)
            {
                if (itemCounts.ContainsKey(item.itemName))
                {
                    var existingList = itemCounts[item.itemName];

                    // Check if there is an existing tuple with the same condition
                    var existingTuple = existingList.FirstOrDefault(tuple => tuple.Item2 == Mathf.Round(item.itemCondition));

                    if (existingTuple != null)
                    {
                        // Increment the item count for the existing tuple
                        var index = existingList.IndexOf(existingTuple);
                        var updatedTuple = Tuple.Create(existingTuple.Item1 + 1, existingTuple.Item2);
                        existingList[index] = updatedTuple;
                    }
                    else
                    {
                        // Add a new tuple for a different condition
                        existingList.Add(new Tuple<int, float>(1, Mathf.Round(item.itemCondition)));
                    }
                }
                else
                {
                    itemCounts[item.itemName] = new List<Tuple<int, float>> { new Tuple<int, float>(1, Mathf.Round(item.itemCondition)) };
                    itemIcons.Add(item.itemIcon);
                }

                currentWeight += item.itemWeight;
            }
            
            weightBar.value = currentWeight;
            weightBarFill.color = weightGradient.Evaluate(weightBar.value / weightBar.maxValue);

            int num = 0;

            foreach (var kvp in itemCounts)
            {
                string itemName = kvp.Key;
                List<Tuple<int, float>> itemDataList = kvp.Value;

                foreach (var itemData in itemDataList)
                {
                    GameObject obj = Instantiate(inventoryUIItem, itemContent);
                    var itemCount = obj.transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
                    var itemConditionText = obj.transform.Find("ItemCondition").GetComponent<TextMeshProUGUI>();
                    var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                    obj.GetComponent<Button>().onClick.AddListener(delegate { ShowItemDetail(itemName); });

                    itemCount.text = "x" + itemData.Item1;
                    itemConditionText.text = itemData.Item2 + "%";
                    itemIcon.sprite = itemIcons[num];
                    itemIcon.preserveAspect = true;
                }

                num++;
            }
        }

        private void ShowItemDetail(string itemName)
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
                if (item.itemName == itemName)
                {
                    var itemCountValue = 0;

                    this.itemName.text = item.itemName;
                    itemDescription.text = item.itemDescription;
                    itemIcon.sprite = item.itemIcon;
                    itemIcon.preserveAspect = true;
                    itemCondition.text = Mathf.Round(item.itemCondition) + "%";

                    // Loop through every item count to find how many items we have
                    foreach (var count in itemCounts)
                    {
                        if (count.Key == itemName)
                        {
                            itemCountValue = count.Value.Sum(tuple => tuple.Item1);
                        }
                    }

                    itemWeight.text = (itemCountValue * item.itemWeight).ToString(CultureInfo.InvariantCulture) + "kg";

                    dropItemButton.onClick.AddListener(() => OpenDropWindow(itemName));

                    switch (item.itemType)
                    {
                        case ItemType.Food:
                            var needItemFood = Instantiate(detailNeedItem, needs.transform);
                            needItemFood.transform.Find("Image").GetComponent<Image>().sprite = stomach;
                            needItemFood.transform.Find("Value").GetComponent<TextMeshProUGUI>().text =
                                item.caloriesIntake.ToString(CultureInfo.InvariantCulture);

                            actionButtonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Eat";
                            actionButtonObj.SetActive(true);
                            actionBtn.onClick.AddListener(() => { TryEat(item.caloriesIntake, item); });
                            break;
                    }

                    break;
                }
            }
        }

        private void TryDrink(float waterIntake, ItemData itemData)
        {
            switch (waterIntake)
            {
                case 0:
                    return;
                case > 0:
                    break;
                case < 0:
                    break;
            }
        }

        private void TryEat(float caloriesIntake, ItemData itemData)
        {
            var returnedCalories = VitalManager.Instance.Eat(caloriesIntake);

            if (returnedCalories == 0)
            {
                DeleteItem(itemData);
                
                HideItemDetail();
            }
            else
            {
                var itemIndex = items.IndexOf(itemData);

                items[itemIndex].caloriesIntake = returnedCalories;
            }

            ListItems();
            ShowItemDetail(itemData.itemName);
        }

        private void DeleteItem(ItemData itemData)
        {
            var itemIndex = items.IndexOf(itemData);

            items.RemoveAt(itemIndex);

            ListItems();
            HideItemDetail();
        }

        private void OpenDropWindow(string itemName)
        {
            dropItemWindow.SetActive(true);

            alertHeader.text = $"Discard item: {itemName}";
            dropCounterText.text = "1x";

            Debug.Log("Opening drop window for: " + itemName);

            // Loop through the items in dictionary
            foreach (var item in itemCounts)
            {
                if (item.Key == itemName)
                {
                    currentDetailedItem = itemName;

                    dropItemSlider.value = 1;
                    dropItemSlider.minValue = 1;
                    dropItemSlider.maxValue = item.Value.Sum(tuple => tuple.Item1);
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

            ShowItemDetail(currentDetailedItem);
        }

        private void HideItemDetail()
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
                        var droppedItem = Instantiate(item.itemObj, hit.point, Quaternion.Euler(-90f, 0f, 0f));

                        droppedItem.GetComponent<ItemController>().itemData = item;

                        offset += droppedItem.transform.localScale.x / 2f;
                    }

                    currentWeight -= item.itemWeight;

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
                        var droppedItem = Instantiate(item.itemObj, hit.point, Quaternion.Euler(-90f, 0f, 0f));

                        // assign itemData from inventory to replace the new one
                        droppedItem.GetComponent<ItemController>().itemData = item;

                        offset += droppedItem.transform.localScale.x / 2f;
                    }

                    currentWeight -= item.itemWeight;

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
    }
}