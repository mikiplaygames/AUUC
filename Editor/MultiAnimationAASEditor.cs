using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiAnimationAAS),true), CanEditMultipleObjects]
public class MultiAnimationAASEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        // Draw all properties except defaultInSpotTime
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        
        while (property.NextVisible(enterChildren)) {
            enterChildren = false;

            // Skip the script reference and defaultInSpotTime field
            if (property.propertyPath == "spriteList")
                continue;
                
            EditorGUILayout.PropertyField(property, true);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}