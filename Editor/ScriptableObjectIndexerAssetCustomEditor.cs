using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ScriptableObjectIndexerAsset))]
public class ScriptableObjectIndexerAssetCustomEditor : Editor {
    public static void CheckForNulls(ScriptableObjectIndexerAsset indexer)
    {
        var nulls = indexer.GetNulls();
        int nullsCount = nulls.Count();
        if (nullsCount == 0)
            EditorUtility.DisplayDialog("No Null Entries", "", "OK");
        else
        {
            string message = "Null entries found for IDs: " + string.Join(", ", nulls);
            var result = EditorUtility.DisplayDialog($"{nullsCount} null Entries Detected", message, "Remove nulls", "Cancel");
            if (result)
                indexer.RemoveNulls();
        }
    }
    public static void CheckForUnIdObjects(ScriptableObjectIndexerAsset indexer)
    {
        var unIdiedObjects = indexer.GetAllUnIdiedObjects().ToList();
        int unIdiedCount = unIdiedObjects.Count();
        if (unIdiedCount == 0)
            EditorUtility.DisplayDialog("No UnId-ied Objects", "", "OK");
        else
        {
            string message = "UnId-ied objects found: " + string.Join(", ", unIdiedObjects.Select(x => x.name));
            var result = EditorUtility.DisplayDialog($"{unIdiedCount} UnId-ied Objects Detected", message, "Regenerate IDs", "Cancel");
            if (result)
            {
                indexer.UpdateMainDict_SOFT();
                EditorUtility.SetDirty(indexer);
            }
        }
    }
    public static void CheckForUnlistedObjects(ScriptableObjectIndexerAsset indexer)
    {
        var unlistedObjects = indexer.GetAllUnlistedObjects().ToList();
        int unlistedCount = unlistedObjects.Count();
        if (unlistedCount == 0)
            EditorUtility.DisplayDialog("No Unlisted Objects", "", "OK");
        else
        {
            string message = "Unlisted objects found: " + string.Join(", ", unlistedObjects.Select(x => x.name));
            var result = EditorUtility.DisplayDialog($"{unlistedCount} Unlisted Objects Detected", message, "Relist All Unlisted Objects", "Cancel");
            if (result)
            {
                indexer.UpdateMainDict_SOFT();
                EditorUtility.SetDirty(indexer);
            }
        }
    }
    public static void CheckForSameIdObjects(ScriptableObjectIndexerAsset indexer)
    {
        var dupes = indexer.GetDupes();
        if (dupes.Count() == 0)
            EditorUtility.DisplayDialog("No Duplicates Found", "", "OK");
        else
            DuplicateIdsWindow.ShowWindow(dupes, indexer);
    }
    public static void UpdateTypesBaseIDs_SOFT(ScriptableObjectIndexerAsset indexer)
    {
        var result = EditorUtility.DisplayDialog($"Are you sure?", "This will update base id's for each type. This is used for new objects that don't have an ID yet. It won't change existing id's.", "Update", "Cancel");
        if (!result) return;
        indexer.UpdateTypesBaseIds_SOFT();
        EditorUtility.SetDirty(indexer);
    }
    public static void RegenerateMainDict_SOFT(ScriptableObjectIndexerAsset indexer)
    {
        var result = EditorUtility.DisplayDialog($"Are you sure?", "This will check and regenerate id's base on source (advanced scriptable object data)", "Regenerate", "Cancel");
        if (!result) return;
        indexer.RegenerateAllDictIds();
        EditorUtility.SetDirty(indexer);
    }
    public static void RegenerateMainDict_HARD(ScriptableObjectIndexerAsset indexer)
    {
        var result = EditorUtility.DisplayDialog($"ARE YOU SURE??????", "This will regenerate all IDs in the main dictionary. This action is irreversible and may break id's.", "Regenerate", "Cancel");
        if (!result) return;
        indexer.RegenerateMainDict_HARD();
        EditorUtility.SetDirty(indexer);
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Check For Nulls"))
            CheckForNulls((ScriptableObjectIndexerAsset)target);
        if (GUILayout.Button("Check For UnId-ied Objects"))
            CheckForUnIdObjects((ScriptableObjectIndexerAsset)target);
        if (GUILayout.Button("Check For Unlisted Objects"))
            CheckForUnlistedObjects((ScriptableObjectIndexerAsset)target);
        if (GUILayout.Button("Check for same ID Objects (Dupes)"))
            CheckForSameIdObjects((ScriptableObjectIndexerAsset)target);
        GUILayout.Space(15);
        GUILayout.Label("================= Warning ZONE =================", new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.orange } });
        if (GUILayout.Button("Update Types Base IDs SOFT"))
            UpdateTypesBaseIDs_SOFT((ScriptableObjectIndexerAsset)target);
        if (GUILayout.Button("Regenerate Main Dict SOFT"))
            RegenerateMainDict_SOFT((ScriptableObjectIndexerAsset)target);
        GUILayout.Space(15);
        GUILayout.Label("================= DANGER ZONE =================", new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.red } });
        if (GUILayout.Button("Regenerate Main Dict HARD"))
            RegenerateMainDict_HARD((ScriptableObjectIndexerAsset)target);
    }
}