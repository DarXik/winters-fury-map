using UnityEditor;
using UnityEngine;

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
    public float itemWeight;
    [Range(0, 100)] public float itemCondition;
    public float conditionPerDay;
    public Sprite itemIcon;
    [Header("Needs Impact")]
    [HideInInspector] public float waterIntake;
    [HideInInspector] public float caloriesIntake;
    [HideInInspector] public float fatigueReduce;

    [Header("Heatsource Data")] 
    [HideInInspector] public float temperatureIncrease;
    [HideInInspector] public int burnTime;
    [HideInInspector] public float chanceBonus;
}

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    private SerializedProperty waterIntakeProp;
    private SerializedProperty caloriesIntakeProp;
    private SerializedProperty fatigueReduceProp;
    private SerializedProperty temperatureIncreaseProp;
    private SerializedProperty burnTimeProp;
    private SerializedProperty chanceProp;

    void OnEnable()
    {
        waterIntakeProp = serializedObject.FindProperty("waterIntake");
        caloriesIntakeProp = serializedObject.FindProperty("caloriesIntake");
        fatigueReduceProp = serializedObject.FindProperty("fatigueReduce");

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
            EditorGUILayout.PropertyField(waterIntakeProp);
            EditorGUILayout.PropertyField(caloriesIntakeProp);
            EditorGUILayout.PropertyField(fatigueReduceProp);
        }
        else if (itemData.itemType == ItemType.Fuelsource)
        {
            EditorGUILayout.PropertyField(temperatureIncreaseProp);
            EditorGUILayout.PropertyField(burnTimeProp);
            EditorGUILayout.PropertyField(chanceProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
