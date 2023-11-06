using System.Collections.Generic;
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

                num++;
            }
        }

        private void ShowItemDetail(string itemName)
        {
            
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
                
                item.itemCondition -= Mathf.Round(item.conditionPerDay / 24f * (Time.deltaTime * timeIncrement));
            }
        }
    }
}