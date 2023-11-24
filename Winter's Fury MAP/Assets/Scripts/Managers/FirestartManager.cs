using System;
using System.Collections;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FirestartManager : MonoBehaviour
{
    [SerializeField] private GameObject campfire;
    
    [Header("UI References")] 
    [SerializeField] private GameObject fireStartWindow;
    [SerializeField] private TextMeshProUGUI baseChanceText, chanceOfSuccessText, fireDurText, fuelNameText, fuelAmountText;
    [SerializeField] private Image fuelIcon;
    [SerializeField] private GameObject selector, progress;
    [SerializeField] private Image progressIcon;

    [Header("Values")]
    [SerializeField] private float baseFireStartingChance;
    [Tooltip("Real-world seconds for the fire to start.")]
    [SerializeField] private float realStartingTime;
    [Tooltip("In-game minutes that will be added after attempting to start the fire.")]
    [SerializeField] private float inGameStartingTime;
    
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
        
        chanceOfSuccess = baseFireStartingChance;

        currentItem = fuelItem;

        AssignFuelInfoToUI();

        maxFuelCount = fuelCount;
        
        fuelAmountText.text = $"{chosenFuelCount} of {maxFuelCount}";
    }

    public void CloseFireStartWindow()
    {
        fireStartWindow.SetActive(false);
        selector.SetActive(true);
        progress.SetActive(false);
        
        InventoryManager.Instance.ToggleInventory();
    }

    public void StartFire()
    {
        selector.SetActive(false);
        progress.SetActive(true);

        StartCoroutine(StartFireProgress());
    }

    private IEnumerator StartFireProgress()
    {
        float chance = Mathf.Round(Random.value * 100);
        
        progressIcon.fillAmount = 0f;
        
        var timeElapsed = 0f;

        while (timeElapsed < realStartingTime)
        {
            progressIcon.fillAmount = Mathf.Lerp(0f, 1f, timeElapsed / realStartingTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        progressIcon.fillAmount = 1f;

        if (chance <= chanceOfSuccess)
        {
            InventoryManager.Instance.DeleteItem(currentItem);

            var playerPos = PlayerController.Instance.GetPlayerPosition();

            if (Physics.Raycast(playerPos, Vector3.down, out var hit, 10f))
            {
                Instantiate(campfire, hit.point, Quaternion.identity);
            }
        }
        
        GameManager.Instance.AddMinutes(inGameStartingTime);
        CloseFireStartWindow();
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
        
        chanceOfSuccess = baseFireStartingChance;
        
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
        chanceOfSuccess += currentItem.chanceBonus;
        
        chanceOfSuccessText.text = chanceOfSuccess + "%";
        
        startingFuelDuration = currentItem.burnTime;
        fuelDuration = startingFuelDuration;

        fireDurText.text = $"{GetFuelHours()}H {GetFuelMinutes()}M";
        fuelNameText.text = currentItem.itemName;

        fuelIcon.sprite = currentItem.itemIcon;
        fuelIcon.preserveAspect = true;
        
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
