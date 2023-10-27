using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 1)]
public class ItemData : ScriptableObject
{
    public GameObject itemObj;
    public string itemName;
    [TextArea]
    public string itemDescription;
    public float itemWeight;
    [Range(0, 100)] public float itemCondition;
    public float randomizeFactor;
    public float conditionPerDay;
    public Sprite itemIcon;
}
