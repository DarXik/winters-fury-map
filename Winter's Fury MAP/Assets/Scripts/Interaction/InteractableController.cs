using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Interaction
{
    public class InteractableController : MonoBehaviour
    {
        public InteractionData interactionData;

        public void Search()
        {
            float chance = Mathf.Round(Random.value * 100);
            Debug.Log(chance);

            for (var i = interactionData.items.Count - 1; i >= 0; i--)
            {
                var interactItem = interactionData.items[i];
                if (chance <= interactItem.chance)
                {
                    ItemData foundItem = interactItem.itemData;

                    InventoryManager.Instance.AddItem(foundItem);
                }
            }
        }
    }
}