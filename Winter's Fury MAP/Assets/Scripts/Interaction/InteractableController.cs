using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interaction
{
    public class InteractableController : MonoBehaviour
    {
        public InteractionData interactionData;

        private void Start()
        {
            // make a copy of the interactionData so we can edit it
            InteractionData dataCopy = Instantiate(interactionData);

            interactionData = dataCopy;
        }

        public IEnumerator Search()
        {
            if (interactionData.items.Count <= 0) yield break;
            
            for (var i = interactionData.items.Count - 1; i >= 0; i--)
            {
                float chance = Mathf.Round(Random.value * 100);
                
                var interactItem = interactionData.items[i];
                if (chance <= interactItem.chance)
                {
                    ItemData foundItem = interactItem.itemData;

                    InventoryManager.Instance.AddItem(foundItem);
                    
                    PlayerInteraction.Instance.ShowFoundItemInfo(foundItem);

                    yield return StartCoroutine(WaitForNext());
                }
                
                interactionData.items.RemoveAt(i);
            }

            interactionData.interactableName += " (Searched)";
        }

        private IEnumerator WaitForNext()
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
        }
    }
}