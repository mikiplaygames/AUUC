using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [Serializable]
    public struct Pair
    {
        public TKey Key;
        public TValue Value;
        public static implicit operator KeyValuePair<TKey, TValue>(Pair pair)
        {
            return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
        }
        public static implicit operator Pair(KeyValuePair<TKey, TValue> pair) => new()
        {
            Key = pair.Key,
            Value = pair.Value
        };
    }
    [SerializeField] private List<Pair> pairs = new();
    public SerializableDictionary() : base() { }
    public SerializableDictionary(int capacity) : base(capacity) { }
    public SerializableDictionary(IDictionary<TKey, TValue> pairs) : base(pairs) { }
    public SerializableDictionary(SerializableDictionary<TKey, TValue> pairs)
    {
        foreach (var pair in pairs)
            this[pair.Key] = pair.Value;
    }
    public int IndexOfKey(TKey key) => Keys.ToList().IndexOf(key);
    public int IndexOfValue(TValue value) => Values.ToList().IndexOf(value);
    public void OnBeforeSerialize()
    {
        pairs.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
            pairs.Add(pair);
    }
    public void OnAfterDeserialize()
    {
        Clear();

        foreach (var pair in pairs)
        {
            if (pair.Key == null)
            {
                Debug.LogWarning($"SerializableDictionary: Found null key during deserialization. type: {typeof(TKey)} value associated with this key: {pair.Value}. Skipping entry.");
                continue;
            }
            Add(pair.Key, pair.Value);
        }
    }
}
