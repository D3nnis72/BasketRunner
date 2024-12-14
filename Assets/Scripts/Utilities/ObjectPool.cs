using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Queue<T> _poolQueue = new Queue<T>();
    private readonly T _prefab;
    private readonly Transform _parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            AddToPool(CreateNewObject());
        }
    }

    private T CreateNewObject()
    {
        var obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        return obj;
    }

    private void AddToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        _poolQueue.Enqueue(obj);
    }

    public T GetFromPool()
    {
        if (_poolQueue.Count == 0)
        {
            AddToPool(CreateNewObject());
        }
        var obj = _poolQueue.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        AddToPool(obj);
    }
}
