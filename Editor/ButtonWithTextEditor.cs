using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ButtonWithText))]
[CanEditMultipleObjects]
public class ButtonWithTextEditor : ButtonEditor
{
    SerializedProperty buttonTextProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        buttonTextProperty = serializedObject.FindProperty("buttonText");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(buttonTextProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
