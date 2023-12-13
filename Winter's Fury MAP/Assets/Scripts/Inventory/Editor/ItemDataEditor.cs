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

    private SerializedProperty toolTypeProp;
    private SerializedProperty toolWeightProp;
    private SerializedProperty burningItemObjProp;
    private SerializedProperty burnDensityProp;
    private SerializedProperty heatBonusProp;
    private SerializedProperty interactTime;
    private SerializedProperty interactText;

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

        toolTypeProp = serializedObject.FindProperty("toolType");
        toolWeightProp = serializedObject.FindProperty("toolWeight");
        
        burningItemObjProp = serializedObject.FindProperty("burningItemObj");
        burnDensityProp = serializedObject.FindProperty("burnDensity");
        heatBonusProp = serializedObject.FindProperty("heatBonus");
        interactTime = serializedObject.FindProperty("interactTime");
        interactText = serializedObject.FindProperty("interactText");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ItemData itemData = (ItemData)target;

        // Display default properties
        DrawDefaultInspector();

        // Display additional fields only if itemType is ItemType.Food
        if (itemData.itemType == ItemType.FoodAndDrink)
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
        else if (itemData.itemType == ItemType.Tool)
        {
            EditorGUILayout.PropertyField(toolTypeProp);
            EditorGUILayout.PropertyField(toolWeightProp);
            
            if (itemData.toolType == ToolType.Lightsource)
            {
                EditorGUILayout.PropertyField(burningItemObjProp);
                EditorGUILayout.PropertyField(burnDensityProp);
                EditorGUILayout.PropertyField(heatBonusProp);
                EditorGUILayout.PropertyField(interactTime);
                EditorGUILayout.PropertyField(interactText);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

}