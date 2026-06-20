using System.Collections.Generic;
using UnityEngine.Events;
[System.Serializable]
public class ObservedVariable<T>
{
    public UnityEvent<T> OnValueChanged
    {
        get { 
            onValueChanged ??= new UnityEvent<T>();
            return onValueChanged; 
        }
    }
    [System.NonSerialized]private UnityEvent<T> onValueChanged = new();
    [UnityEngine.SerializeField]private T _value;
    public void _QuietSetValue(T newValue) => _value = newValue;
    public ObservedVariable(T initialValue = default)
    {
        _value = initialValue;
    }
    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                OnValueChanged?.Invoke(value);
                _value = value;
            }
        }
    }
    public static implicit operator T(ObservedVariable<T> observedVariable) => observedVariable.Value;
}