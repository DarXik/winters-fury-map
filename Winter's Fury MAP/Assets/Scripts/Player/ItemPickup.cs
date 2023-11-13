using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private RaycastHit hit;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) CheckHit();
    }

    private void CheckHit()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, LayerMask.GetMask("Item")))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        // Create a copy of the itemData
        ItemData itemDataCopy = Instantiate(hit.transform.GetComponent<ItemController>().itemData);

        if (InventoryManager.Instance.currentWeight + itemDataCopy.itemWeight >
            InventoryManager.Instance.maxWeight) return;
        
        InventoryManager.Instance.AddItem(itemDataCopy);
        
        Destroy(hit.transform.gameObject);
    }
}
