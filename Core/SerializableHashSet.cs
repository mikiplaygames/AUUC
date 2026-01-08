using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializableHashSet<T> : HashSet<T> , ISerializationCallbackReceiver {
    [SerializeField] private List<T> items = new();

    public void OnBeforeSerialize() {
        items.Clear();
        foreach (var item in this)
            items.Add(item);
    }

    public void OnAfterDeserialize() {
        Clear();
        foreach (var item in items)
            Add(item);
    }
}