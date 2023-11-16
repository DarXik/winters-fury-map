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
        [FormerlySerializedAs("itemCondition")] [SerializeField] private TextMeshProUGUI itemConditionText;
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
        private float timeIncrement;
        private float previousTimeIncrement;
        private bool inventoryOpened;
        private string currentDetailedItem;
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
        }

        private void Update()
        {
            if (items.Count > 0) ReduceItemsCondition();
            
            var currentIncrement = GameManager.Instance.cycle.TimeIncrement;
            if (previousTimeIncrement != currentIncrement)
            {
                timeIncrement = currentIncrement;

                previousTimeIncrement = currentIncrement;
            }
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
            HideItemDetail();
            currentWeight = 0f;

            itemCounts = new List<Tuple<string, int, float>>();
            List<Sprite> itemIcons = new();

            foreach (var item in items)
            {
                if (itemCounts.Any(tuple => tuple.Item1 == item.itemName))
                {
                    var existingTuple =
                        itemCounts.FirstOrDefault(tuple => tuple.Item1 == item.itemName && tuple.Item3 == Mathf.Round(item.itemCondition));

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


                currentWeight += item.itemWeight;
            }

            weightBar.value = currentWeight;
            weightBarFill.color = weightGradient.Evaluate(weightBar.value / weightBar.maxValue);

            int num = 0;

            foreach (var count in itemCounts)
            {
                GameObject obj = Instantiate(inventoryUIItem, itemContent);
                var itemCount = obj.transform.Find("ItemInfo").Find("ItemCount").GetComponent<TextMeshProUGUI>();
                var itemConditionText = obj.transform.Find("ItemInfo").Find("ItemCondition").GetComponent<TextMeshProUGUI>();
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                obj.GetComponent<Button>().onClick.AddListener(delegate { ShowItemDetail(count.Item1, count.Item3); });

                itemCount.text = "x" + count.Item2;
                itemConditionText.text = count.Item3 + "%";
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
                if (item.itemName == itemName && Mathf.Round(item.itemCondition) == itemCondition)
                {
                    var itemCountValue = 0;

                    this.itemName.text = item.itemName;
                    itemDescription.text = item.itemDescription;
                    itemIcon.sprite = item.itemIcon;
                    itemIcon.preserveAspect = true;
                    itemConditionText.text = Mathf.Round(item.itemCondition) + "%";

                    // Loop through every item count to find how many items we have
                    foreach (var count in itemCounts)
                    {
                        if (count.Item1 == itemName)
                        {
                            itemCountValue = count.Item2;
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

                ListItems();
            }
            else
            {
                var itemIndex = items.IndexOf(itemData);

                items[itemIndex].caloriesIntake = returnedCalories;
                
                ListItems();
                ShowItemDetail(itemData.itemName, Mathf.Round(itemData.itemCondition));
            }
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