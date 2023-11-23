using System;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirestartManager : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private GameObject fireStartWindow;
    [SerializeField] private TextMeshProUGUI baseChanceText, chanceOfSuccessText, fireDurText, fuelNameText, fuelAmountText;
    [SerializeField] private Image fuelIcon;

    [Header("Values")]
    [SerializeField] private float baseFireStartingChance;
    [SerializeField] private float fireStartingTime; // in-game time to start the fire
    private ItemData currentItem;
    private int maxFuelCount;
    private int chosenFuelCount = 1;
    private float chanceOfSuccess;
    private int startingFuelDuration, fuelDuration;
    
    public static FirestartManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        fireStartWindow.SetActive(false);

        chanceOfSuccess = baseFireStartingChance;
        baseChanceText.text = baseFireStartingChance + "%";
    }
    
    public void OpenFireStartWindow(ItemData fuelItem, int fuelCount)
    {
        PlayerLook.Instance.BlockRotation();
        fireStartWindow.SetActive(true);

        currentItem = fuelItem;

        AssignFuelInfoToUI();

        maxFuelCount = fuelCount;
        
        fuelAmountText.text = $"{chosenFuelCount} of {maxFuelCount}";
    }

    public void CloseFireStartWindow()
    {
        fireStartWindow.SetActive(false);
        
        InventoryManager.Instance.ToggleInventory();
    }

    public void StartFire()
    {
        
    }

    public void AddFuel()
    {
        if (chosenFuelCount < maxFuelCount)
        {
            chosenFuelCount++;
            fuelDuration += startingFuelDuration;
        }
        
        UpdateFuelInfoText();
    }

    public void RemoveFuel()
    {
        if (chosenFuelCount > 1)
        {
            chosenFuelCount--;
            fuelDuration -= startingFuelDuration;
        }
        
        UpdateFuelInfoText();
    }

    public void SwitchFuelSource()
    {
        var fuelItems = InventoryManager.Instance.GetFuelItems();
        var itemCounts = InventoryManager.Instance.GetItemCounts();
        
        foreach (var fuelItem in fuelItems)
        {
            if (fuelItem != currentItem)
            {
                currentItem = fuelItem;
                break;
            }
        }

        foreach (var count in itemCounts)
        {
            if (count.Item1 == currentItem.itemName)
            {
                maxFuelCount = count.Item2;
                break;
            }
        }
        
        AssignFuelInfoToUI();
    }

    private void AssignFuelInfoToUI()
    {
        chanceOfSuccessText.text = chanceOfSuccess + currentItem.chanceBonus + "%";
        
        startingFuelDuration = currentItem.burnTime;
        fuelDuration = startingFuelDuration;

        fireDurText.text = $"{GetFuelHours()}H {GetFuelMinutes()}M";
        fuelNameText.text = currentItem.itemName;

        fuelIcon.sprite = currentItem.itemIcon;
        
        UpdateFuelInfoText();
    }

    private void UpdateFuelInfoText()
    {
        fuelAmountText.text = $"{chosenFuelCount} of {maxFuelCount}";
        fireDurText.text = $"{GetFuelHours()}H {GetFuelMinutes()}M";
    }

    private int GetFuelHours()
    {
        return fuelDuration / 60;
    }

    private int GetFuelMinutes()
    {
        return fuelDuration % 60;
    }
}
