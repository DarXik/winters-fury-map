using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private List<ItemData> items = new();

        private float timeIncrement;
        
        public static InventoryManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            timeIncrement = GameManager.Instance.cycle.TimeIncrement;
        }

        private void Update()
        {
            if(items.Count != 0) ReduceItemsCondition();
        }

        public void AddItem(ItemData itemData)
        {
            items.Add(itemData);
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