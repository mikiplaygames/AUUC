using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ScriptableObjectIndexerAsset))]
public class ScriptableObjectIndexerAssetCustomEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Check For Nulls"))
        {
            var indexer = (ScriptableObjectIndexerAsset)target;
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
        if (GUILayout.Button("Check For UnId-ied Objects"))
        {
            var indexer = (ScriptableObjectIndexerAsset)target;
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
        if (GUILayout.Button("Check For Unlisted Objects"))
        {
            var indexer = (ScriptableObjectIndexerAsset)target;
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
        if (GUILayout.Button("Check for same ID Objects (Dupes)"))
        {
            var indexer = (ScriptableObjectIndexerAsset)target;
            var dupes = indexer.GetDupes();
            if (dupes.Count() == 0)
                EditorUtility.DisplayDialog("No Duplicates Found", "", "OK");
            else
                DuplicateIdsWindow.ShowWindow(dupes, indexer);
        }
        GUILayout.Space(15);
        GUILayout.Label("================= Warning ZONE =================", new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.orange } });
        if (GUILayout.Button("Update Types Base IDs SOFT"))
        {
            var result = EditorUtility.DisplayDialog($"Are you sure?", "This will update base id's for each type. This is used for new objects that don't have an ID yet. It won't change existing id's.", "Update", "Cancel");
            if (!result) return;
            var indexer = (ScriptableObjectIndexerAsset)target;
            indexer.UpdateTypesBaseIds_SOFT();
            EditorUtility.SetDirty(indexer);
        }
        if (GUILayout.Button("Regenerate Main Dict SOFT"))
        {
            var result = EditorUtility.DisplayDialog($"Are you sure?", "This will check and regenerate id's base on source (advanced scriptable object data)", "Regenerate", "Cancel");
            if (!result) return;
            var indexer = (ScriptableObjectIndexerAsset)target;
            indexer.RegenerateAllDictIds();
            EditorUtility.SetDirty(indexer);
        }
        GUILayout.Space(15);
        GUILayout.Label("================= DANGER ZONE =================", new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.red } });
        if (GUILayout.Button("Regenerate Main Dict HARD"))
        {
            var result = EditorUtility.DisplayDialog($"ARE YOU SURE??????", "This will regenerate all IDs in the main dictionary. This action is irreversible and may break id's.", "Regenerate", "Cancel");
            if (!result) return;
            var indexer = (ScriptableObjectIndexerAsset)target;
            indexer.RegenerateMainDict_HARD();
            EditorUtility.SetDirty(indexer);
        }
    }
}