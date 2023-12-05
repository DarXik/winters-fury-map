using UnityEditor;

namespace Inventory.Editor
{
    [CustomEditor(typeof(ItemData))]
public class ItemDataEditor : UnityEditor.Editor
{
    private SerializedProperty calorieDensityProp;
    private SerializedProperty waterIntakeProp;
    private SerializedProperty caloriesIntakeProp;
    private SerializedProperty fatigueReduceProp;
    private SerializedProperty conditionPerDayProp;
    private SerializedProperty conditionVariabilityProp;

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
        conditionPerDayProp = serializedObject.FindProperty("conditionPerDay");
        conditionVariabilityProp = serializedObject.FindProperty("conditionVariability");

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
            EditorGUILayout.PropertyField(conditionPerDayProp);
            EditorGUILayout.PropertyField(conditionVariabilityProp);
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

}