using UnityEngine;

namespace Inventory
{
    public enum ItemType
    {
        FoodAndDrink,
        Fuelsource,
        Tool
    }

    public enum ToolType
    {
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
        [Range(0, 100)] public float itemCondition = 100f;
        public Sprite itemIcon;
        public bool inflictsAffliction;

        [Header("Affliction")] 
        [HideInInspector] public Affliction affliction;
        [HideInInspector] [Range(0f, 100f)] public float afflictionChance;
    
        public float ItemWeight
        {
            get
            {

                // Is food
                if (itemType == ItemType.FoodAndDrink && calorieDensity > 0)
                {
                    return Mathf.Round(caloriesIntake / calorieDensity * 100) / 100f;
                }

                // Is water
                if(itemType == ItemType.FoodAndDrink && calorieDensity == 0)
                {
                    return Mathf.Round(waterIntake / 1000f * 100) / 100f;
                }

                if (itemType == ItemType.Fuelsource)
                {
                    return fuelItemWeight;
                }

                if (itemType == ItemType.Tool)
                {
                    return toolWeight;
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

        [Header("Tool Properties")] 
        [HideInInspector] public ToolType toolType;
        [HideInInspector] public float toolWeight;
    
        [Header("Tool - Lightsource")]
        [HideInInspector] public GameObject burningItemObj;
        [HideInInspector] public float burnDensity;
        [HideInInspector] public float heatBonus;
        [HideInInspector] public float interactTime;
        [HideInInspector] public string leftActionText;
        [HideInInspector] public string rightActionText;
        [HideInInspector] public bool isLit;
    
        public float MaxLightSourceBurnTime => (burnDensity / toolWeight * 100) / 100f;
    }
}