using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class ScriptableObjectIndexerAsset : ScriptableObject
{
    [SerializeField] private bool refresh;
    [SerializeField] private SerializableDictionary<int, AdvancedScriptableObject> MainIdsObjects;
    [SerializeField] private SerializableDictionary<Type, List<AdvancedScriptableObject>> SortedByTypeObjects;
    public const string Path = "Assets/Resources/DoNotMoveThisFile.asset"; //todo DO NOT MOVE THIS FILE
    public const string FileName = "DoNotMoveThisFile";
    [SerializeField] string currentPathCheck;
    [Header("DO NOT CLICK BOTH")]
    [SerializeField] private bool remove;
    [SerializeField] private bool actuallyRemove;
    public T GetObject<T>(int id) where T : ScriptableObject
    {
        if (MainIdsObjects.TryGetValue(id, out var obj) && obj is T)
            return obj as T;
        return null;
    }
    public int GetIdOfObject<T>(T obj) where T : ScriptableObject
    {
        foreach (var kvp in MainIdsObjects)
        {
            if (kvp.Value.Equals(obj))
                return kvp.Key;
        }
        return default; //todo ??? nie lepiej dac -1 sie upewnic czeba
    }
    public int GetIdOfObjectByName(string name)
    {
        foreach (var kvp in MainIdsObjects)
        {
            if (kvp.Value.name.Equals(name))
                return kvp.Key;
        }
        return -1;
    }
    public List<T> GetObjectsOfType<T>() where T : ScriptableObject
    {
        return MainIdsObjects.Values.OfType<T>().ToList();
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (actuallyRemove && !remove)
            actuallyRemove = false;
        if (remove && actuallyRemove)
        {
            MainIdsObjects.Clear();
            remove = false;
            actuallyRemove = false;
            return;
        }

        if (!PathIsCorrect())
            return;
        refresh = false;
        CheckForListChanges();
    }
    private bool PathIsCorrect()
    {
        currentPathCheck = UnityEditor.AssetDatabase.GetAssetPath(this);
        if (currentPathCheck != Path)
        {
            name = FileName;
            Debug.LogError($"FILE PATH IS WRONG IN CONST!!!! current: {currentPathCheck} != supposed: {Path} \n FIXING IT NOW....");
            string directory = System.IO.Path.GetDirectoryName(Path);
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            UnityEditor.AssetDatabase.MoveAsset(currentPathCheck, Path);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            currentPathCheck = UnityEditor.AssetDatabase.GetAssetPath(this);
            return currentPathCheck == Path;
        }
        else
            return true;
    }
    bool MainIsEqual(IEnumerable<AdvancedScriptableObject> allOrganizedObjects) => MainIdsObjects.Values.OrderBy(x => x.name).SequenceEqual(allOrganizedObjects);
    private void CheckForListChanges()
    {
        if (refresh)
        {
            refresh = false;
            return;
        }
        var allOrganizedObjects = Resources.FindObjectsOfTypeAll<AdvancedScriptableObject>().OrderBy(x => x.name);

        if (MainIsEqual(allOrganizedObjects))
        {
            //Debug.Log("Scriptable object list did not change, no update needed.");
            return;
        }

        foreach (var obj in allOrganizedObjects)
        {
            Type type = obj.GetType();
            if (!SortedByTypeObjects.ContainsKey(type))
            {
                if (type == null)
                {
                    Debug.LogError("Type is null for object: " + obj.name);
                    Debug.LogError("This is broken check your scriptable objects");
                    continue;
                }
                SortedByTypeObjects.Add(type, new List<AdvancedScriptableObject>());
            }
            if (!SortedByTypeObjects[type].Contains(obj))
                SortedByTypeObjects[type].Add(obj);
        }
        SortedByTypeObjects = new(TopologicalSortByInheritance(SortedByTypeObjects.Keys).ToDictionary(t => t, t => SortedByTypeObjects[t]));

        RegenerateMainDict();
    }
    void RegenerateMainDict()
    {
        MainIdsObjects.Clear();
        int typeIndex = 0;
        int objectIndex = 0;
        foreach (var kvp in SortedByTypeObjects)
        {
            foreach (var obj in kvp.Value)
            {
                MainIdsObjects.Add(typeIndex * 1000 + objectIndex, obj);
                objectIndex++;
            }
            objectIndex = 0;
            typeIndex++;
        }
    }

    IEnumerable<Type> TopologicalSortByInheritance(IEnumerable<Type> types)
    {
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