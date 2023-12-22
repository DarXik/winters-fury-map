using System;
using UnityEditor;
using UnityEditor.Rendering;

namespace Inventory.Editor
{
    [CustomEditor(typeof(Affliction))]
    public class AfflictionEditor : UnityEditor.Editor
    {
        private SerializedProperty treated;
        private SerializedProperty untreatedMin;
        private SerializedProperty untreatedMax;

        private SerializedProperty treatment;
        private SerializedProperty treatmentAmount;

        private void OnEnable()
        {
            treated = serializedObject.FindProperty("treated");
            untreatedMin = serializedObject.FindProperty("untreatedMin");
            untreatedMax = serializedObject.FindProperty("untreatedMax");

            treatment = serializedObject.FindProperty("treatment");
            treatmentAmount = serializedObject.FindProperty("treatmentAmount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Affliction affliction = (Affliction)target;

            DrawDefaultInspector();

            if (affliction.hasSetDuration)
            {
                EditorGUILayout.PropertyField(treated);
                EditorGUILayout.PropertyField(untreatedMin);
                EditorGUILayout.PropertyField(untreatedMax);
            }

            if (affliction.hasTreatment)
            {
                EditorGUILayout.PropertyField(treatment);
                EditorGUILayout.PropertyField(treatmentAmount);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}