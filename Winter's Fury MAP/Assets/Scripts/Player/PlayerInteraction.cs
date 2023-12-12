using System;
using System.Collections;
using System.Globalization;
using Heat;
using Interaction;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("UI References")] 
        public TextMeshProUGUI interactText;
        public Image holdCircle;

        [Header("Default Values")] public float maxInteractDistance;
        public float holdInteractTime;
        private float timeElapsed;
        private bool interacting;

        [Header("Found Item Info")] public GameObject foundItemWindow;
        public Image itemIcon;
        public TextMeshProUGUI itemName, itemDesc, itemCondition, itemWeight;

        [Header("Equip Item References")] 
        public Transform itemHolder;
        private ItemData equippedItem;
        private float interactItemTimeElapsed;

        public static HeatSource interactedCampfire;

        private RaycastHit clickHit;
        private RaycastHit hoverHit;

        public static PlayerInteraction Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            holdCircle.fillAmount = 0f;

            foundItemWindow.SetActive(false);
        }

        private void Update()
        {
            if (InventoryManager.inventoryOpened || FirestartManager.fireWindowOpened ||
                AddFuelManager.addFuelWindowOpened || PassTimeManager.passTimeWindowOpened) return;

            CheckHover();
            if (Input.GetMouseButtonDown(0)) CheckHit();
            if (Input.GetMouseButton(0) && !interacting && !foundItemWindow.activeInHierarchy) CheckHold();
            if (Input.GetMouseButtonUp(0))
            {
                holdCircle.fillAmount = 0f;
                timeElapsed = 0f;
                interactItemTimeElapsed = 0f;
            }

            if (equippedItem != null && equippedItem.toolType == ToolType.Lightsource && !equippedItem.isBurning && Input.GetMouseButton(0)) Light();
        }

        private void CheckHover()
        {
            if (Physics.Raycast(transform.position, transform.forward, out hoverHit, maxInteractDistance))
            {
                if (hoverHit.transform.root.TryGetComponent(out ItemController item))
                {
                    interactText.text = item.itemData.itemName;
                }
                else if (hoverHit.transform.TryGetComponent(out InteractableController interactable))
                {
                    interactText.text = interactable.interactionData.interactionText;
                }
                else
                {
                    interactText.text = "";
                }
            }
            else
            {
                interactText.text = "";
            }
        }

        private void CheckHold()
        {
            if (Physics.Raycast(transform.position, transform.forward, out clickHit, maxInteractDistance,
                    LayerMask.GetMask("Searchable")))
            {
                if (timeElapsed < holdInteractTime)
                {
                    holdCircle.fillAmount = Mathf.Lerp(0f, 1f, timeElapsed / holdInteractTime);
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    holdCircle.fillAmount = 0f;
                    Interact();
                    StartCoroutine(TempHoldBlock());
                }
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
                         LayerMask.GetMask("Interactable")))
            {
                Interact();
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

        private void Interact()
        {
            var controller = clickHit.transform.GetComponentInParent<InteractableController>();

            switch (controller.interactionData.interactableType)
            {
                case InteractableType.Door:
                    controller.OpenDoor();
                    break;
                case InteractableType.Bed:
                    interactText.text = "";
                    PassTimeManager.Instance.TogglePassTimeWindow(PassTypes.Sleep, controller.interactionData.warmthBonus);
                    break;
                case InteractableType.Searchable:
                    StartCoroutine(controller.Search());
                    break;
            }
        }

        private void PickupItem()
        {
            // Create a copy of the itemData
            ItemData itemDataCopy = Instantiate(clickHit.transform.root.GetComponent<ItemController>().itemData);

            if (InventoryManager.Instance.currentWeight + itemDataCopy.ItemWeight >
                InventoryManager.Instance.maxWeight) return;

            itemDataCopy.itemCondition -= (int)Random.Range(0, itemDataCopy.conditionVariability);

            InventoryManager.Instance.AddItem(itemDataCopy);

            Destroy(clickHit.transform.root.gameObject);
        }

        private IEnumerator TempHoldBlock()
        {
            interacting = true;

            ;            yield return new WaitForSeconds(2);

            interacting = false;
            timeElapsed = 0f;
        }

        public void ShowFoundItemInfo(ItemData foundItem)
        {
            if (!foundItemWindow.activeInHierarchy)
            {
                foundItemWindow.SetActive(true);
                PlayerLook.Instance.BlockRotation(false);
            }

            itemIcon.sprite = foundItem.itemIcon;
            itemIcon.preserveAspect = true;

            itemName.text = foundItem.itemName;
            itemDesc.text = foundItem.itemDescription;
            itemCondition.text = foundItem.itemCondition + "%";
            itemWeight.text = foundItem.ItemWeight.ToString(CultureInfo.InvariantCulture) + " KG";
        }

        public void HideFoundItemInfo()
        {
            foundItemWindow.SetActive(false);
            PlayerLook.Instance.UnblockRotation();
        }

        public void EquipTool(ItemData itemData)
        {
            InventoryManager.Instance.ToggleInventory();
            
            equippedItem = itemData;
            Instantiate(equippedItem.itemObj, itemHolder, false);
        }

        private void Light()
        {
            if (interactItemTimeElapsed < equippedItem.interactTime)
            {
                holdCircle.fillAmount = Mathf.Lerp(0f, 1f, interactItemTimeElapsed / equippedItem.interactTime);
                interactItemTimeElapsed += Time.deltaTime;
                interactText.text = equippedItem.interactText;
            }
            else
            {
                holdCircle.fillAmount = 0f;

                interactText.text = "";
                
                // light torch
                Destroy(itemHolder.GetChild(0).gameObject);
                Instantiate(equippedItem.burningItemObj, itemHolder, false);
                equippedItem.isBurning = true;
            }
        }
    }
}