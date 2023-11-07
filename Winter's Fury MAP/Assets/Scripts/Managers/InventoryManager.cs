using System;
using System.Collections.Generic;
using System.Globalization;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private List<ItemData> items = new();

        [Header("UI References")]
        [SerializeField] private GameObject inventory;
        [SerializeField] private GameObject inventoryUIItem;
        [SerializeField] private Transform itemContent;
        [SerializeField] private GameObject itemDetail;

        [Header("Item Detail References")] 
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private TextMeshProUGUI itemCondition;
        [SerializeField] private TextMeshProUGUI itemWeight;

        private Dictionary<string, int> itemCounts;
        private float timeIncrement;
        
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
        }

        private void Update()
        {
            if(items.Count > 0) ReduceItemsCondition();
        }

        public void ToggleInventory()
        {
            if (inventory.activeInHierarchy)
            {
                // Inventory closed
                
                PlayerLook.Instance.UnblockRotation();
                inventory.SetActive(false);
                itemDetail.SetActive(false);
            }
            else
            {
                // Inventory opened
                
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
                }
            }
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
                if(item.itemCondition <= 0f) continue;

                item.itemCondition -= (item.conditionPerDay / 24f) * (Time.deltaTime * timeIncrement);
            }
        }
    }
}