using System.Collections.Generic;
using UnityEngine.Events;

public class ObservedVariable<T>
{
    public UnityEvent<T> OnValueChanged = new();
    private T _value;

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