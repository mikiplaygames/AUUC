using System;
using UnityEngine;
public class AdvancedScriptableObject : ScriptableObject , IIdentify {
    private int _id = 0;
    public int Id{
        get {
            if (_id == 0)
                _id = GetId();
            return _id;
        }
    }
    public static int GetIdOf(AdvancedScriptableObject objectData) => ScriptableObjectIndexer.GetIdOfObject(objectData);
    public virtual int GetId() => GetIdOf(this);
    public static T GetById<T>(int itemId) where T : ScriptableObject => ScriptableObjectIndexer.GetObject<T>(itemId);
}
[Serializable]
public class ASOWrapper<T> : IIdentify where T : AdvancedScriptableObject
{
    [NonSerialized]private T _value;
    public int Id => ASOid;
    public int ASOid = 0;
    public T Value{
        get {
            if (_value == null)
                _value = AdvancedScriptableObject.GetById<T>(ASOid);
            return _value;
        }
        set{
            _value = value;
            if (value != null)
                ASOid = value.Id;
            else 
                ASOid = 0;
        }
    }
    public ASOWrapper(T value) => Value = value;
    public ASOWrapper(int itemId)
    {
        ASOid = itemId;
        _value = AdvancedScriptableObject.GetById<T>(ASOid);
    }
    public static implicit operator T(ASOWrapper<T> wrapper) => wrapper.Value;
    public static implicit operator ASOWrapper<T>(T value) => new(value);
}