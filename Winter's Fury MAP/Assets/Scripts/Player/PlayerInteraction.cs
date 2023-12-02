using Heat;
using Managers;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        public TextMeshProUGUI itemText;
        
        public float maxInteractDistance;
        public static HeatSource interactedCampfire;

        private RaycastHit clickHit;
        private RaycastHit hoverHit;

        private void Update()
        {
            if (InventoryManager.inventoryOpened || FirestartManager.fireWindowOpened ||
                AddFuelManager.addFuelWindowOpened || PassTimeManager.passTimeWindowOpened) return;
            
            CheckHover();
            if (Input.GetMouseButtonDown(0)) CheckHit();
        }

        private void CheckHover()
        {
            if (Physics.Raycast(transform.position, transform.forward, out hoverHit, maxInteractDistance))
            {
                itemText.text = hoverHit.transform.root.TryGetComponent(out ItemController item) ? item.itemData.itemName : "";
            }
            else
            {
                itemText.text = "";
            }
        }

        private void CheckHit()
        {
            if (Physics.Raycast(transform.position, transform.forward, out clickHit, maxInteractDistance,
                    LayerMask.GetMask("Item")))
            {
                PickupItem();
            }
            else if (Physics.Raycast(transform.position, transform.forward, out clickHit, maxInteractDistance,
                         LayerMask.GetMask("Campfire")))
            {
                interactedCampfire = clickHit.transform.GetComponent<HeatSource>();
                var fireDuration = interactedCampfire.burnTime;
                var heatOutput = interactedCampfire.heatOutput;
                
                AddFuelManager.Instance.OpenAddFuelWindow(fireDuration, heatOutput, clickHit.transform);
            }
        }
        
        private void PickupItem()
        {
            // Create a copy of the itemData
            ItemData itemDataCopy = Instantiate(clickHit.transform.root.GetComponent<ItemController>().itemData);

            if (InventoryManager.Instance.currentWeight + itemDataCopy.ItemWeight >
                InventoryManager.Instance.maxWeight) return;
        
            InventoryManager.Instance.AddItem(itemDataCopy);
        
            Destroy(clickHit.transform.root.gameObject);
        }
    }
}