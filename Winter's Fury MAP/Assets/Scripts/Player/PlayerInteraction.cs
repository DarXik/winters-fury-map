using System;
using Heat;
using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        public LayerMask[] layers;
        public float maxInteractDistance;

        private RaycastHit hit;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) CheckHit();
        }

        private void CheckHit()
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxInteractDistance,
                    LayerMask.GetMask("Item")))
            {
                PickupItem();
            }
            else if (Physics.Raycast(transform.position, transform.forward, out hit, maxInteractDistance,
                         LayerMask.GetMask("Campfire")))
            {
                var fireDuration = hit.transform.GetComponent<HeatSource>().burnTime;
                var heatOutput = hit.transform.GetComponent<HeatSource>().heatOutput;
                
                AddFuelManager.Instance.OpenAddFuelWindow(fireDuration, heatOutput);
            }
        }
        
        private void PickupItem()
        {
            // Create a copy of the itemData
            ItemData itemDataCopy = Instantiate(hit.transform.root.GetComponent<ItemController>().itemData);

            if (InventoryManager.Instance.currentWeight + itemDataCopy.itemWeight >
                InventoryManager.Instance.maxWeight) return;
        
            InventoryManager.Instance.AddItem(itemDataCopy);
        
            Destroy(hit.transform.root.gameObject);
        }
    }
}