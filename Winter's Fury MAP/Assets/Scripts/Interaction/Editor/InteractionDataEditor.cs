using UnityEditor;
using UnityEngine;

namespace Interaction.Editor
{
        [CustomEditor(typeof(InteractionData))]
    public class InteractionDataEditor : UnityEditor.Editor
    {
        private SerializedProperty warmthBonus;

        private void OnEnable()
        {
            warmthBonus = serializedObject.FindProperty("warmthBonus");
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("items"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

// IngredientDrawer
    [CustomPropertyDrawer(typeof(Item))]
    public class IngredientDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            // Calculate rects
            var dataRect = new Rect(position.x, position.y, 200, position.height);
            var chanceRect = new Rect(position.x + 210, position.y, 100, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(dataRect, property.FindPropertyRelative("itemData"), GUIContent.none);
            EditorGUI.PropertyField(chanceRect, property.FindPropertyRelative("chance"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}