using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ButtonWithTextMenu
{
    [MenuItem("GameObject/UI/Button With Text (TMPro)", false, 2031)]
    private static void CreateButtonWithText(MenuCommand menuCommand)
    {
        // Find/create Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
        }

        // Find/create EventSystem
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }

        // Create button root
        var buttonGO = new GameObject("Button With Text", typeof(RectTransform), typeof(Image), typeof(ButtonWithText));
        Undo.RegisterCreatedObjectUndo(buttonGO, "Create Button With Text");
        GameObjectUtility.SetParentAndAlign(buttonGO, canvas.gameObject);
        var rect = buttonGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 30);

        // Create TMP label
        var textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        Undo.RegisterCreatedObjectUndo(textGO, "Create Button Text");
        GameObjectUtility.SetParentAndAlign(textGO, buttonGO);

        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = "Button";
        tmp.alignment = TextAlignmentOptions.Center;

        // Assign private serialized field: buttonText
        var button = buttonGO.GetComponent<ButtonWithText>();
        var so = new SerializedObject(button);
        so.FindProperty("buttonText").objectReferenceValue = tmp;
        so.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = buttonGO;
    }
}