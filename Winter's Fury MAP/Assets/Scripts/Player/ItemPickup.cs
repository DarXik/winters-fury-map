using System;
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log(hit.transform.GetComponent<ItemController>().itemData.itemName);
        
        Destroy(hit.transform.gameObject);
    }
}
