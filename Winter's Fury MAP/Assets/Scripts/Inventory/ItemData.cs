using UnityEngine;

public enum ItemType
{
    Food
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
    [Range(-100, 100)] public float waterIntake = 0;
    public float caloriesIntake;
    public float fatigueReduce;
}
