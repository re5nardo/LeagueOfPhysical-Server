using System.Collections.Generic;
using GameFramework;
using System;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<Type, InternalObjectPool> objectPools = new Dictionary<Type, InternalObjectPool>();

    public T GetObject<T>() where T : class, IPoolable
    {
        if (!objectPools.ContainsKey(typeof(T)))
        {
            objectPools[typeof(T)] = new InternalObjectPool();
        }

        return objectPools[typeof(T)].GetObject<T>();
    }

    public IPoolable GetObject(Type type)
    {
        if (!objectPools.ContainsKey(type))
        {
            objectPools[type] = new InternalObjectPool();
        }

        return objectPools[type].GetObject(type);
    }

    public void ReturnObject(IPoolable obj)
    {
        if (objectPools.TryGetValue(obj.GetType(), out var internalObjectPool))
        {
            internalObjectPool.ReturnObject(obj);
        }
        else
        {
            Debug.LogWarning($"Returned object is invalid! Type: {obj.GetType()}");
        }
    }

    class InternalObjectPool
    {
        private Queue<IPoolable> pool = new Queue<IPoolable>();
        private LinkedList<IPoolable> beingUsed = new LinkedList<IPoolable>();

        public T GetObject<T>() where T : IPoolable
        {
            T target = default;
            if (pool.Count > 0)
            {
                target = (T)pool.Dequeue();
            }
            else
            {
                target = Activator.CreateInstance<T>();
            }

            beingUsed.AddLast(target);
            return target;
        }

        public IPoolable GetObject(Type type)
        {
            if (!typeof(IPoolable).IsAssignableFrom(type))
            {
                Debug.LogError($"The type is invalid! Type: {type}");
                return null;
            }

            IPoolable target = null;
            if (pool.Count > 0)
            {
                target = pool.Dequeue() as IPoolable;
            }
            else
            {
                target = Activator.CreateInstance(type) as IPoolable;
            }

            beingUsed.AddLast(target);
            return target;
        }

        public void ReturnObject(IPoolable obj)
        {
            obj.Clear();

            beingUsed.Remove(obj);
            pool.Enqueue(obj);
        }
    }
}
