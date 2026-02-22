using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DuplicateIdsWindow : EditorWindow
{
    private class Row
    {
        public int Id;
        public List<AdvancedScriptableObject> Objects;
    }

    private Vector2 _scroll;
    private List<Row> _rows = new List<Row>();
    private readonly Dictionary<AdvancedScriptableObject, int> _pendingIds = new();
    private static ScriptableObjectIndexerAsset _indexer;

    public static void ShowWindow(IEnumerable<AdvancedScriptableObject> dupes, ScriptableObjectIndexerAsset indexer)
    {
        _indexer = indexer;
        var window = GetWindow<DuplicateIdsWindow>("Duplicate IDs");
        window.minSize = new Vector2(900, 350);
        window.BuildRows(dupes);
        window.Show();
    }

    private void BuildRows(IEnumerable<AdvancedScriptableObject> dupes)
    {
        _rows = dupes
            .GroupBy(x => x.Id)
            .OrderBy(g => g.Key)
            .Select(g => new Row
            {
                Id = g.Key,
                Objects = g.ToList()
            })
            .ToList();

        _pendingIds.Clear();
        foreach (var obj in _rows.SelectMany(x => x.Objects))
        {
            if (obj == null) continue;
            _pendingIds[obj] = obj.Id;
        }
    }
    private bool IsUsedByNonDuplicateAsset(int candidateId, AdvancedScriptableObject editedObject, out AdvancedScriptableObject conflictingObject)
    {
        var allAssets = Resources
            .FindObjectsOfTypeAll<AdvancedScriptableObject>()
            .Where(x => x != null)
            .ToList();

        var assetsWithCandidateId = allAssets.Where(x => x.Id == candidateId).ToList();
        var uniqueIdInProject = assetsWithCandidateId.Count == 1;

        if (!uniqueIdInProject)
        {
            conflictingObject = null;
            return false;
        }

        conflictingObject = assetsWithCandidateId[0];
        if (conflictingObject == editedObject)
            return false;

        return true;
    }
    private void OnGUI()
    {
        EditorGUILayout.Space(6);

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("ID", EditorStyles.boldLabel, GUILayout.Width(180));
            GUILayout.Label("AdvancedScriptableObject(s)", EditorStyles.boldLabel);
            GUILayout.Label("Change ID (Per Object)", EditorStyles.boldLabel, GUILayout.Width(220));
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        bool refreshRows = false;

        foreach (var row in _rows)
        {
            using (new EditorGUILayout.HorizontalScope("box"))
            {
                // Column 1: current duplicate ID
                GUILayout.Label(row.Id.ToString(), GUILayout.Width(180));

                // Column 2: all objects sharing that ID
                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (var obj in row.Objects)
                    {
                        EditorGUILayout.ObjectField(obj, typeof(AdvancedScriptableObject), false);
                    }
                }

                // Column 3: change ID per object
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(220)))
                {
                    foreach (var obj in row.Objects)
                    {
                        if (obj == null)
                        {
                            EditorGUILayout.LabelField("Missing", GUILayout.Width(200));
                            continue;
                        }

                        if (!_pendingIds.ContainsKey(obj))
                            _pendingIds[obj] = obj.Id;

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            _pendingIds[obj] = EditorGUILayout.IntField(_pendingIds[obj]);

                            using (new EditorGUI.DisabledScope(_pendingIds[obj] == obj.Id))
                            {
                                if (GUILayout.Button("Apply", GUILayout.Width(60)))
                                {
                                    if (IsUsedByNonDuplicateAsset(_pendingIds[obj], obj, out var conflictingObject))
                                    {
                                        EditorUtility.DisplayDialog(
                                            "ID In Use",
                                            $"Cannot assign ID {_pendingIds[obj]} to {obj.name} because it is already used by non-duplicate asset: {conflictingObject.name}.",
                                            "OK");
                                        continue;
                                    }

                                    Undo.RecordObject(obj, "Change Duplicate ID");
                                    obj.SetNewId(_pendingIds[obj]);
                                    EditorUtility.SetDirty(obj);

                                    if (_indexer != null)
                                    {
                                        _indexer.RegenerateAllDictIds();
                                        EditorUtility.SetDirty(_indexer);
                                    }

                                    refreshRows = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        EditorGUILayout.EndScrollView();

        if (refreshRows)
        {
            BuildRows(_indexer.GetDupes());
            GUIUtility.ExitGUI();
        }
    }
}