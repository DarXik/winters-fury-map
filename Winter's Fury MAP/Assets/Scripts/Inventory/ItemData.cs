using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum ItemType
{
    Food,
    Drink,
    Fuelsource,
    Lightsource
}

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Scriptables/Item", order = 1)]
public class ItemData : ScriptableObject
{
    public GameObject itemObj;
    public string itemName;
    [TextArea]
    public string itemDescription;
    public ItemType itemType;
    [Range(0, 100)] public float itemCondition;
    public Sprite itemIcon;
    
    public float ItemWeight
    {
        get
        {
            if (itemType == ItemType.Food)
            {
                return Mathf.Round(caloriesIntake / calorieDensity * 100) / 100f;
            }

            if (itemType == ItemType.Drink)
            {
                return Mathf.Round(waterIntake / 1000f * 100) / 100f;
            }

            if (itemType == ItemType.Fuelsource)
            {
                return fuelItemWeight;
            }

            return 0;
        }
    }

    [Header("Food Properties")]
    [HideInInspector] public float calorieDensity;
    [HideInInspector] public float waterIntake;
    [HideInInspector] public float caloriesIntake;
    [HideInInspector] public float fatigueReduce;
    [HideInInspector] public float conditionPerDay;
    [HideInInspector] public float conditionVariability;

    [Header("Fuel Properties")] 
    [HideInInspector] public float fuelItemWeight;
    [HideInInspector] public float temperatureIncrease;
    [HideInInspector] public int burnTime;
    [HideInInspector] public float chanceBonus;
}
