using System.Collections.Generic;
using UnityEngine.Events;

public class ObservedList<T> : List<T>
{
    public UnityEvent OnListChanged = new();
    public UnityEvent<int> OnListElementChanged = new();
    public ObservedList() : base() { }
    public ObservedList(int capacity) : base(capacity) { }
    public new void Add(T item)
    {
        base.Add(item);
        OnListChanged?.Invoke();
    }
    public new void Remove(T item)
    {
        base.Remove(item);
        OnListChanged?.Invoke();
    }
    public new void Clear()
    {
        base.Clear();
        OnListChanged?.Invoke();
    }
    public new T this[int index]
    {
        get => base[index];
        set
        {
            base[index] = value;
            OnListElementChanged?.Invoke(index);
        }
    }
} 