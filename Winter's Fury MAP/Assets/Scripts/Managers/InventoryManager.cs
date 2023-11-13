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
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private TextMeshProUGUI itemCondition;
        [SerializeField] private TextMeshProUGUI itemWeight;
        [SerializeField] private Button dropItemButton;
        [SerializeField] private GameObject eatActionButton;

        [Header("Drop Window References")] [SerializeField]
        private GameObject dropItemWindow;

        [SerializeField] private TextMeshProUGUI alertHeader;
        [SerializeField] private Slider dropItemSlider;
        [SerializeField] private TextMeshProUGUI dropCounterText;
        [SerializeField] private Button dropSelectedButton;
        [SerializeField] private Button dropAllButton;

        private Dictionary<string, int> itemCounts;
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
            itemDetail.SetActive(false);
            dropItemWindow.SetActive(false);

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
                itemDetail.SetActive(false);
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

            currentWeight += itemData.itemWeight;
            weightBar.value = currentWeight;
            weightBarFill.color = weightGradient.Evaluate(weightBar.value / weightBar.maxValue);
        }

        private void ListItems()
        {
            DeleteInventoryContents();

            itemDetail.SetActive(false);

            itemCounts = new Dictionary<string, int>();
            List<Sprite> itemIcons = new();

            foreach (var item in items)
            {
                if (itemCounts.ContainsKey(item.itemName))
                {
                    itemCounts[item.itemName]++;
                }
                else
                {
                    itemCounts[item.itemName] = 1;

                    itemIcons.Add(item.itemIcon);
                }
            }

            int num = 0;

            foreach (var item in itemCounts)
            {
                GameObject obj = Instantiate(inventoryUIItem, itemContent);
                var itemCount = obj.transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
                obj.GetComponent<Button>().onClick.AddListener(delegate { ShowItemDetail(item.Key); });

                itemCount.text = "x " + item.Value;
                itemIcon.sprite = itemIcons[num];
                itemIcon.preserveAspect = true;

                num++;
            }
        }

        private void ShowItemDetail(string itemName)
        {
            itemDetail.SetActive(true);

            dropItemSlider.onValueChanged.RemoveAllListeners();
            dropItemButton.onClick.RemoveAllListeners();
            dropSelectedButton.onClick.RemoveAllListeners();
            dropAllButton.onClick.RemoveAllListeners();

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
                            itemCountValue = count.Value;
                        }
                    }

                    itemCount.text = "x " + itemCountValue;
                    itemWeight.text = (itemCountValue * item.itemWeight).ToString(CultureInfo.InvariantCulture) + "kg";

                    dropItemButton.onClick.AddListener(() => OpenDropWindow(itemName));

                    switch (item.itemType)
                    {
                        case ItemType.Food:
                            eatActionButton.SetActive(true);
                            break;
                    }

                    break;
                }
            }
        }

        private void OpenDropWindow(string itemName)
        {
            dropItemWindow.SetActive(true);

            alertHeader.text = $"Discard item: {itemName}";

            Debug.Log("Opening drop window for: " + itemName);


            // Loop through the items in dictionary
            foreach (var item in itemCounts)
            {
                if (item.Key == itemName)
                {
                    currentDetailedItem = itemName;

                    dropItemSlider.value = 1;
                    dropItemSlider.minValue = 1;
                    dropItemSlider.maxValue = item.Value;
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

            ShowItemDetail(currentDetailedItem);
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