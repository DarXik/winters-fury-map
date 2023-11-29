using Heat;
using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float maxInteractDistance;
        public static HeatSource interactedCampfire;

        private RaycastHit hit;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) CheckHit();
        }

        private void CheckHit()
        {
            if (InventoryManager.inventoryOpened || FirestartManager.fireWindowOpened ||
                AddFuelManager.addFuelWindowOpened) return;
            
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxInteractDistance,
                    LayerMask.GetMask("Item")))
            {
                PickupItem();
            }
            else if (Physics.Raycast(transform.position, transform.forward, out hit, maxInteractDistance,
                         LayerMask.GetMask("Campfire")))
            {
                interactedCampfire = hit.transform.GetComponent<HeatSource>();
                var fireDuration = interactedCampfire.burnTime;
                var heatOutput = interactedCampfire.heatOutput;
                
                AddFuelManager.Instance.OpenAddFuelWindow(fireDuration, heatOutput, hit.transform);
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