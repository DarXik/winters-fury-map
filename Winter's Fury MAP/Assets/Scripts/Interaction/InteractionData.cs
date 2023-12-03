using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Interaction
{
    public enum InteractableType
    {
        Bed,
        Searchable
    }

    [Serializable]
    public class Item
    {
        public ItemData itemData;
        [Range(0f, 100f)] public float chance = 100f;
    }

    [Serializable]
    [CreateAssetMenu(fileName = "Interactable", menuName = "Scriptables/Interactable", order = 2)]
    public class InteractionData : ScriptableObject
    {
        public string interactableName;
        public InteractableType interactableType;
        
        [Header("Bed Properties")] [HideInInspector]
        public float warmthBonus;

        [Header("Searchable Properties")] 
        public List<Item> items;
    }

    [CustomEditor(typeof(InteractionData))]
    public class InteractionDataEditor : Editor
    {
        private SerializedProperty warmthBonus;
        private SerializedProperty items;

        private void OnEnable()
        {
            warmthBonus = serializedObject.FindProperty("warmthBonus");
            items = serializedObject.FindProperty("items");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InteractionData interactionData = (InteractionData)target;

            DrawDefaultInspector();

            if (interactionData.interactableType == InteractableType.Bed)
            {
                EditorGUILayout.PropertyField(warmthBonus);
            }
            else if (interactionData.interactableType == InteractableType.Searchable)
            {
                //EditorGUILayout.PropertyField(items);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}