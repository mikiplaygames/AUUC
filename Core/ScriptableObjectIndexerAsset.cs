using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEditor;

public class ScriptableObjectIndexerAsset : ScriptableObject
{
    [SerializeField] private SerializableDictionary<string, int> TypeStartingIds;
    [SerializeField] private SerializableDictionary<int, AdvancedScriptableObject> MainIdsObjects;
    public const string Path = "Assets/Resources/DoNotMoveThisFile.asset"; //todo DO NOT MOVE THIS FILE
    public const string FileName = "DoNotMoveThisFile";
    public T GetObject<T>(int id) where T : AdvancedScriptableObject
    {
        if (MainIdsObjects.TryGetValue(id, out var obj) && obj is T)
            return obj as T;
        return null;
    }
    public int GetIdOfObject<T>(T obj) where T : AdvancedScriptableObject
    {
        foreach (var kvp in MainIdsObjects)
        {
            if (kvp.Value.Equals(obj))
                return kvp.Key;
        }
        return default;
    }
    public int GetIdOfObjectByName(string name)
    {
        foreach (var kvp in MainIdsObjects)
        {
            if (kvp.Value.name.Equals(name))
                return kvp.Key;
        }
        return default;
    }
    public List<T> GetObjectsOfType<T>() where T : AdvancedScriptableObject
    {
        return MainIdsObjects.Values.OfType<T>().ToList();
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!IsPathCorrect())
            return;
    }
    private bool IsPathCorrect()
    {
        var currentPathCheck = AssetDatabase.GetAssetPath(this);
        if (currentPathCheck != Path)
        {
            name = FileName;
            Debug.LogError($"{this} file is not in right dir! current: {currentPathCheck} != supposed: {Path} \n FIXING IT NOW....");
            string directory = System.IO.Path.GetDirectoryName(Path);
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            AssetDatabase.MoveAsset(currentPathCheck, Path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            currentPathCheck = AssetDatabase.GetAssetPath(this);
            return currentPathCheck == Path;
        }
        else
            return true;
    }
    private IEnumerable<AdvancedScriptableObject> GetAllObjects() => Resources.FindObjectsOfTypeAll<AdvancedScriptableObject>();
    private IEnumerable<AdvancedScriptableObject> GetAllSortedObjects() => GetAllObjects().OrderBy(x => x.name);
    private IEnumerable<Type> GetAllSortedTypes() => TopologicalSortByInheritance(GetAllObjects().Select(x => x.GetType()).Where(t => t != null).Distinct());
    private Dictionary<Type, List<AdvancedScriptableObject>> GetObjectsSortedByType()
    {
        var allSortedObjs = GetAllSortedObjects();
        Dictionary<Type, List<AdvancedScriptableObject>> SortedByTypeObjects = new();
        foreach (var obj in allSortedObjs)
        {
            Type type = obj.GetType();
            if (type == null)
            {
                Debug.LogError("Type is null for object: " + obj.name);
                Debug.LogError("This is broken check your scriptable objects, skipping...");
                continue;
            }
            if (!SortedByTypeObjects.ContainsKey(type))
                SortedByTypeObjects.Add(type, new List<AdvancedScriptableObject>());
            if (!SortedByTypeObjects[type].Contains(obj))
                SortedByTypeObjects[type].Add(obj);
        }
        SortedByTypeObjects = new(TopologicalSortByInheritance(SortedByTypeObjects.Keys).ToDictionary(t => t, t => SortedByTypeObjects[t]));
        return SortedByTypeObjects;
    }
    public IEnumerable<AdvancedScriptableObject> GetAllUnIdiedObjects() => GetAllSortedObjects().Where(x => x.Id == 0);
    public IEnumerable<AdvancedScriptableObject> GetAllUnlistedObjects()
    {
        var allObjs = GetAllSortedObjects();
        var listedObjs = MainIdsObjects.Values.ToHashSet();
        return allObjs.Where(x => !listedObjs.Contains(x));
    }
    public IEnumerable<int> GetNulls() => MainIdsObjects.Where(x => x.Value == null).Select(x => x.Key);
    public void RemoveNulls()
    {
        var nullKeys = GetNulls().ToList();
        foreach (var key in nullKeys)
            MainIdsObjects.Remove(key);
    }
    public IEnumerable<AdvancedScriptableObject> GetDupes()
    {
        var allAssets = Resources
            .FindObjectsOfTypeAll<AdvancedScriptableObject>()
            .Where(x => x != null)
            .ToList();

        return allAssets
            .GroupBy(x => x.Id)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g);
    }
    public void RegenerateAllDictIds()
    {
        SerializableDictionary<int, AdvancedScriptableObject> tempDict = new();
        foreach (var item in MainIdsObjects)
        {
            if (item.Value == null) continue;
            tempDict.Add(item.Value.Id, item.Value);
        }
        MainIdsObjects = new(tempDict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value));
    }
    public void UpdateTypesBaseIds_SOFT()
    {
        var types = GetAllSortedTypes();
        types.Where(t => !TypeStartingIds.ContainsKey(t.FullName)).ToList().ForEach(t =>
        {
            int newBaseId = TypeStartingIds.Values.Count > 0 ? TypeStartingIds.Values.Max() + 1000 : 1;
            TypeStartingIds.Add(t.FullName, newBaseId);
        });
        TypeStartingIds.Where(kvp => !types.Any(t => t.FullName == kvp.Key)).Select(kvp => kvp.Key).ToList().ForEach(key =>
        {
            TypeStartingIds.Remove(key);
        });
    }
    public void RegenerateTypesBaseIds_HARD()
    {
        var types = GetAllSortedTypes();
        TypeStartingIds = new();
        int baseStartId = 0;
        foreach (var type in types)
        {
            TypeStartingIds[type.FullName] = baseStartId;
            baseStartId += 1000;
        }
    }
    public void UpdateMainDict_SOFT()
    {
        UpdateTypesBaseIds_SOFT();
        SerializableDictionary<int, AdvancedScriptableObject> tempNewDict = new(MainIdsObjects);
        var allNewFoundObjects = GetObjectsSortedByType();
        
        foreach (var kvp in allNewFoundObjects)
        {
            foreach (var obj in kvp.Value)
            {
                if (!tempNewDict.ContainsValue(obj))
                {
                    int newId = obj.Id != 0 ? obj.Id : kvp.Value.Count > 0 ? kvp.Value.Max(x => x.Id) + 1 : 1;
                    if (tempNewDict.TryGetValue(newId, out var val))
                    {
                        Debug.Log($"ID conflict for object {obj.name} with ID {newId} already used by {val.name}. Use \"Check For Unlisted Objects\" function in the indexer");
                        continue;
                    }
                    tempNewDict.Add(newId, obj);
                    if (obj.Id == newId) continue;
                    obj.SetNewId(newId);
                    EditorUtility.SetDirty(obj);
                }
            }
        }
        MainIdsObjects = new(tempNewDict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value));
    }
    public void RegenerateMainDict_HARD()
    {
        RegenerateTypesBaseIds_HARD();
        MainIdsObjects.Clear();
        int objectIndex = 1;
        var objs = GetObjectsSortedByType();
        foreach (var kvp in objs)
        {
            foreach (var obj in kvp.Value)
            {
                var id = TypeStartingIds[kvp.Key.FullName] + objectIndex;
                obj.SetNewId(id);
                EditorUtility.SetDirty(obj);
                MainIdsObjects.Add(id, obj);
                objectIndex++;
            }
            objectIndex = 1;
        }
    }
    IEnumerable<Type> TopologicalSortByInheritance(IEnumerable<Type> stringTypes)
    {
        var types = stringTypes.Where(t => t != null).ToList();
        var typeSet = new HashSet<Type>(types);
        var children = new Dictionary<Type, List<Type>>();
        foreach (var t in types)
        {
            var baseType = t.BaseType;
            if (baseType != null && typeSet.Contains(baseType))
            {
                if (!children.ContainsKey(baseType))
                    children[baseType] = new List<Type>();
                children[baseType].Add(t);
            }
        }

        var visited = new HashSet<Type>();
        var result = new List<Type>();

        void Visit(Type t)
        {
            if (!visited.Add(t)) return;
            if (children.TryGetValue(t, out var derived))
                foreach (var c in derived.OrderBy(x => x.Name))
                    Visit(c);
            result.Add(t);
        }

        // Start from all roots (types whose base is not in the set)
        foreach (var t in types.Where(t => t.BaseType == null || !typeSet.Contains(t.BaseType)).OrderBy(x => x.Name))
            Visit(t);

        // Reverse to get base classes first, then derived
        result.Reverse();
        return result;
    }
#endif
}