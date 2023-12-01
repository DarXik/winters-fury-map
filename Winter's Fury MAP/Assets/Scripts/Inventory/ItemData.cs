using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum ItemType
{
    Food,
    Drink,
    Fuelsource
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
    public float conditionPerDay;
    public Sprite itemIcon;
    
    public float ItemWeight
    {
        get
        {
            if (itemType == ItemType.Food)
            {
                return caloriesIntake / calorieDensity;
            }

            if (itemType == ItemType.Drink)
            {
                return waterIntake / 1000;
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

    [Header("Fuel Properties")] 
    [HideInInspector] public float fuelItemWeight;
    [HideInInspector] public float temperatureIncrease;
    [HideInInspector] public int burnTime;
    [HideInInspector] public float chanceBonus;
}

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    private SerializedProperty calorieDensityProp;
    private SerializedProperty waterIntakeProp;
    private SerializedProperty caloriesIntakeProp;
    private SerializedProperty fatigueReduceProp;

    private SerializedProperty fuelWeightProp;
    private SerializedProperty temperatureIncreaseProp;
    private SerializedProperty burnTimeProp;
    private SerializedProperty chanceProp;

    void OnEnable()
    {
        calorieDensityProp = serializedObject.FindProperty("calorieDensity");
        waterIntakeProp = serializedObject.FindProperty("waterIntake");
        caloriesIntakeProp = serializedObject.FindProperty("caloriesIntake");
        fatigueReduceProp = serializedObject.FindProperty("fatigueReduce");

        fuelWeightProp = serializedObject.FindProperty("fuelItemWeight");
        temperatureIncreaseProp = serializedObject.FindProperty("temperatureIncrease");
        burnTimeProp = serializedObject.FindProperty("burnTime");
        chanceProp = serializedObject.FindProperty("chanceBonus");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ItemData itemData = (ItemData)target;

        // Display default properties
        DrawDefaultInspector();

        // Display additional fields only if itemType is ItemType.Food
        if (itemData.itemType == ItemType.Food || itemData.itemType == ItemType.Drink)
        {
            EditorGUILayout.PropertyField(calorieDensityProp);
            EditorGUILayout.PropertyField(waterIntakeProp);
            EditorGUILayout.PropertyField(caloriesIntakeProp);
            EditorGUILayout.PropertyField(fatigueReduceProp);
        }
        else if (itemData.itemType == ItemType.Fuelsource)
        {
            EditorGUILayout.PropertyField(fuelWeightProp);
            EditorGUILayout.PropertyField(temperatureIncreaseProp);
            EditorGUILayout.PropertyField(burnTimeProp);
            EditorGUILayout.PropertyField(chanceProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
