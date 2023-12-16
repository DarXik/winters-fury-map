using UnityEngine;

namespace Inventory
{
    public enum Treatments
    {
        Antibiotics
    }
    
    public enum AfflictionType
    {
        FoodPoisoning
    }
    
    [System.Serializable]
    [CreateAssetMenu(fileName = "Affliction", menuName = "Scriptables/Affliction", order = 2)]
    public class Affliction : ScriptableObject
    {
        public string afflictionName;
        [TextArea] public string afflictionDescription;
        public Sprite afflictionIcon;
        public AfflictionType afflictionType;
        
        [Header("Duration")] 
        public float treated;
        public float untreatedMin;
        public float untreatedMax;
        [HideInInspector] public float currentDuration;
        [HideInInspector] public float totalDuration;

        [Header("Treatment")] 
        public Treatments treatment;
        public int treatmentAmount;
    }
}