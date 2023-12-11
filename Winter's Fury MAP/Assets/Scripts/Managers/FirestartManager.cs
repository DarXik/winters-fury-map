using System;
using System.Collections;
using Heat;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wind;
using Random = UnityEngine.Random;

public class FirestartManager : MonoBehaviour
{
    public static bool fireWindowOpened;

    [SerializeField] private GameObject campfire;

    [Header("UI References")] [SerializeField]
    private GameObject fireStartWindow;

    [SerializeField] private TextMeshProUGUI baseChanceText,
        chanceOfSuccessText,
        bonusChanceText,
        fireDurText,
        fuelNameText,
        fuelAmountText;

    [SerializeField] private Image fuelIcon;
    [SerializeField] private GameObject selector, progress;
    [SerializeField] private Image progressIcon;

    [Header("Values")] 
    [Tooltip("Distance from player at which the campfire spawns.")]
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float baseFireStartingChance;

    [Tooltip("Real-world seconds for the fire to start.")] 
    [SerializeField] private float realStartingTime;

    [Tooltip("In-game minutes of attempting to start the fire.")] 
    [SerializeField] private float inGameStartingTime;

    private ItemData currentItem;
    private int currentItemIndex;
    private int maxFuelCount;
    private int chosenFuelCount = 1;
    private float chanceOfSuccess;
    private int startingBurnTime, burnTime;
    private float startingTempIncrease, temperatureIncrease;

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
        fireWindowOpened = true;

        chanceOfSuccess = baseFireStartingChance;

        var fuelItems = InventoryManager.Instance.GetFuelItems();

        for (int i = 0; i < fuelItems.Count; i++)
        {
            if (fuelItems[i] == fuelItem)
            {
                currentItem = fuelItem;
                currentItemIndex = i;
            }
        }
        
        AssignFuelInfo();

        maxFuelCount = fuelCount;

        fuelAmountText.text = $"{chosenFuelCount} of {maxFuelCount}";
    }

    public void CloseFireStartWindow()
    {
        fireWindowOpened = false;

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
            for (int i = 0; i < chosenFuelCount; i++)
            {
                InventoryManager.Instance.DeleteItem(currentItem);
            }

            var playerPos = PlayerController.Instance.GetPlayerPosition();
            var playerTransform = PlayerController.Instance.GetPlayerTransform();

            if (Physics.Raycast(playerPos, playerTransform.forward * distanceFromPlayer + Vector3.down, out var hit, 10f))
            {
                var campfire = Instantiate(this.campfire, hit.point, Quaternion.identity);
                campfire.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                var fireParticles = campfire.transform.Find("Fire").transform.Find("FireParticles").GetComponent<ParticleSystem>();
                var fireForce = fireParticles.forceOverLifetime;
                
                var smokeParticles = campfire.transform.Find("Fire").transform.Find("SmokeParticles").GetComponent<ParticleSystem>();
                var smokeForce = smokeParticles.forceOverLifetime;

                var windDir = WindArea.Instance.GetWindDirection();

                fireForce.x = windDir.x;
                fireForce.y = windDir.y;
                fireForce.z = windDir.z;
                smokeForce.x = windDir.x;
                smokeForce.y = windDir.y;
                smokeForce.z = windDir.z;
                
                var heatSource = campfire.GetComponent<HeatSource>();

                // add burnTime to fire minus the 5 minutes of the inGameStartingTime
                heatSource.burnTime += (burnTime / 60f) - (inGameStartingTime / 60f);
                heatSource.heatOutput += temperatureIncrease;
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
            burnTime += startingBurnTime;
            temperatureIncrease += startingTempIncrease;
        }

        UpdateFuelInfoText();
    }

    public void RemoveFuel()
    {
        if (chosenFuelCount > 1)
        {
            chosenFuelCount--;
            burnTime -= startingBurnTime;
            temperatureIncrease -= startingTempIncrease;
        }

        UpdateFuelInfoText();
    }

    public void SwitchFuelSource()
    {
        var fuelItems = InventoryManager.Instance.GetFuelItems();
        var itemCounts = InventoryManager.Instance.GetItemCounts();

        chanceOfSuccess = baseFireStartingChance;
        chosenFuelCount = 1;

        if (currentItemIndex < fuelItems.Count - 1)
        {
            currentItemIndex += 1;
            
            currentItemIndex = fuelItems.FindLastIndex(item => item == fuelItems[currentItemIndex]);
        }
        else
        {
            currentItemIndex = fuelItems.FindLastIndex(item => item == fuelItems[0]);
        }

        currentItem = fuelItems[currentItemIndex];

        foreach (var count in itemCounts)
        {
            if (count.Item1 == currentItem.itemName)
            {
                maxFuelCount = count.Item2;
                break;
            }
        }

        AssignFuelInfo();
    }

    private void AssignFuelInfo()
    {
        chanceOfSuccess += currentItem.chanceBonus;

        chanceOfSuccessText.text = chanceOfSuccess + "%";
        bonusChanceText.text = $"+{currentItem.chanceBonus}%";

        startingBurnTime = currentItem.burnTime;
        burnTime = startingBurnTime;

        fireDurText.text = $"{BurnConverter.GetFuelHours(burnTime)}H {BurnConverter.GetFuelMinutes(burnTime)}M";
        fuelNameText.text = currentItem.itemName;

        fuelIcon.sprite = currentItem.itemIcon;
        fuelIcon.preserveAspect = true;

        startingTempIncrease = currentItem.temperatureIncrease;
        temperatureIncrease = startingTempIncrease;

        UpdateFuelInfoText();
    }

    private void UpdateFuelInfoText()
    {
        fuelAmountText.text = $"{chosenFuelCount} of {maxFuelCount}";
        fireDurText.text = $"{BurnConverter.GetFuelHours(burnTime)}H {BurnConverter.GetFuelMinutes(burnTime)}M";
    }
}