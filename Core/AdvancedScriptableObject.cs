using System;
using UnityEngine;
public class AdvancedScriptableObject : ScriptableObject , IIdentify {
    [SerializeField]private int _id = 0;
    public int Id => _id;
    public void SetNewId(int newId) => _id = newId;
}
[Serializable]
public class ASOSerializer<T> : IIdentify , ISerializationCallbackReceiver where T : AdvancedScriptableObject
{
    [NonSerialized]private T _value;
    public int Id => ASOid;
    public int ASOid;
    public ASOSerializer(T newValue)
    {
        _value = newValue;
        ASOid = newValue.Id;
    }
    public ASOSerializer(int itemId)
    {
        ASOid = itemId;
        _value = ScriptableObjectIndexer.GetObject<T>(ASOid);
    }
    public void OnBeforeSerialize()
    {
        ASOid = _value.Id;
    }
    public void OnAfterDeserialize()
    {
        if (_value == null || _value.Id != ASOid)            
            _value = ScriptableObjectIndexer.GetObject<T>(ASOid);
    }
    public static implicit operator T(ASOSerializer<T> serializer) => serializer._value;
    public static implicit operator ASOSerializer<T>(T value) => new(value);
}