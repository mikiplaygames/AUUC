using UnityEditor;

[CustomEditor(typeof(AdvancedScriptableObject), true)]
public class AdvancedScriptableObjectEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        // Draw all properties except defaultInSpotTime
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        
        while (property.NextVisible(enterChildren)) {
            enterChildren = false;

            // Skip the script reference and defaultInSpotTime field
            if (property.propertyPath == "_id")
            {
                EditorGUILayout.LabelField("ID", serializedObject.FindProperty("_id").intValue.ToString());
                continue;
            }

            EditorGUILayout.PropertyField(property, true);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}