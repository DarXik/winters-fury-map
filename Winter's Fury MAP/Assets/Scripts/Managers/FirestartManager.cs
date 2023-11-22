using System;
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

        chanceOfSuccessText.text = chanceOfSuccess + fuelItem.chanceBonus + "%";

        startingFuelDuration = fuelItem.burnTime;
        fuelDuration = startingFuelDuration;

        fireDurText.text = $"{GetFuelHours()}H {GetFuelMinutes()}M";
        fuelNameText.text = fuelItem.itemName;

        maxFuelCount = fuelCount;
        
        fuelAmountText.text = $"{chosenFuelCount} of {maxFuelCount}";
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
