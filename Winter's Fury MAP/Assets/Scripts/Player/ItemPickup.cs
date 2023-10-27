using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemPickup : MonoBehaviour
{
    private RaycastHit hit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckItem();
        }
    }

    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, LayerMask.GetMask("Item")))
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        var item = hit.transform.GetComponent<ItemController>().itemData;
        item.itemCondition -= Random.Range(0, item.randomizeFactor);

        Debug.Log("You've picked up: " + item.itemName);
    }
}
