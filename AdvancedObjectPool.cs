using System;
using System.Collections.Generic;
using UnityEngine.Pool;
namespace MikiHeadDev.Helpers
{
public class AdvancedObjectPool<T> : IDisposable, IObjectPool<T> where T : class
{
    internal readonly List<T> a_List;
    internal readonly List<T> i_List;

    private readonly Func<T> m_CreateFunc;
    private readonly Action<T> m_ActionOnGet;
    private readonly Action<T> m_ActionOnRelease;
    private readonly Action<T> m_ActionOnDestroy;

    private readonly int m_MaxSize;
    internal bool m_CollectionCheck;

    public bool PoolFullyUtilized => CountActive >= m_MaxSize;
    public bool PoolEmpty => CountAll == 0;
    public int CountAll => CountActive + CountInactive;
    public int CountActive => a_List.Count;
    public int CountInactive => i_List.Count;

    public AdvancedObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        if (createFunc == null)
            throw new ArgumentNullException("createFunc");

        if (maxSize <= 0)
            throw new ArgumentException("Max Size must be greater than 0", "maxSize");

        i_List = new List<T>(defaultCapacity);
        a_List = new List<T>(defaultCapacity);
        m_CreateFunc = createFunc;
        m_MaxSize = maxSize;
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
        m_ActionOnDestroy = actionOnDestroy;
        m_CollectionCheck = collectionCheck;
    }

    public T Get()
    {
        T val;
        if (i_List.Count == 0)
            val = m_CreateFunc();
        else
        {
            int index = i_List.Count - 1;
            val = i_List[index];
            i_List.RemoveAt(index);
        }

        m_ActionOnGet?.Invoke(val);
        a_List.Add(val);
        return val;
    }

    public PooledObject<T> Get(out T v) => new(v = Get(), this);

    public void Release(T element)
    {
        if (m_CollectionCheck && i_List.Count > 0)
        {
            for (int i = 0; i < i_List.Count; i++)
            {
                if (element == i_List[i])
                    throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
            }
        }

        m_ActionOnRelease?.Invoke(element);
        if (CountInactive < m_MaxSize)
        {
            a_List.Remove(element);
            i_List.Add(element);
            return;
        }

        m_ActionOnDestroy?.Invoke(element);
    }
    public void ReleaseAll()
    {
        while (a_List.Count > 0)
            Release(a_List[0]);
    }
    public void Clear()
    {
        if (m_ActionOnDestroy != null)
        {
            foreach (T item in i_List)
            {
                m_ActionOnDestroy(item);
            }
            foreach (T item in a_List)
            {
                m_ActionOnDestroy(item);
            }
        }

        i_List.Clear();
        a_List.Clear();
    }
    public void Dispose()
    {
        Clear();
    }
}
}