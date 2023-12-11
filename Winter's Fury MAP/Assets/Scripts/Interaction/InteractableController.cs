using System.Collections;
using Managers;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interaction
{
    public class InteractableController : MonoBehaviour
    {
        public InteractionData interactionData;
        [SerializeField] private Animator anim;

        private bool opened;

        private void Start()
        {
            // make a copy of the interactionData so we can edit it
            InteractionData dataCopy = Instantiate(interactionData);

            interactionData = dataCopy;
        }

        public IEnumerator Search()
        {
            if (interactionData.searched) yield break;

            if (interactionData.items.Count > 0)
            {
                foreach (var interactItem in interactionData.items)
                {
                    float chance = Mathf.Round(Random.value * 100);

                    if (chance <= interactItem.chance)
                    {
                        ItemData foundItem = Instantiate(interactItem.itemData);

                        InventoryManager.Instance.AddItem(foundItem);
                    
                        PlayerInteraction.Instance.ShowFoundItemInfo(foundItem);

                        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                    }
                
                    yield return null;
                }
            }

            interactionData.interactionText = "[Searched] " + interactionData.interactionText;
            interactionData.searched = true;
            PlayerInteraction.Instance.HideFoundItemInfo();
        }

        public void OpenDoor()
        {
            opened = !opened;

            anim.SetTrigger(opened ? "OpenDoor" : "CloseDoor");
            interactionData.interactionText = opened ? "Close Door" : "Open Door";
        }
    }
}