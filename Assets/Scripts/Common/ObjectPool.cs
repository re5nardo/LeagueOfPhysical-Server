using System.Collections.Generic;
using GameFramework;
using System;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<Type, InternalObjectPool> objectPools = new Dictionary<Type, InternalObjectPool>();

    public T GetObject<T>() where T : class, IPoolObject
    {
        if (!objectPools.ContainsKey(typeof(T)))
        {
            objectPools[typeof(T)] = new InternalObjectPool();
        }

        return objectPools[typeof(T)].GetObject<T>();
    }

    public IPoolObject GetObject(Type type)
    {
        if (!objectPools.ContainsKey(type))
        {
            objectPools[type] = new InternalObjectPool();
        }

        return objectPools[type].GetObject(type);
    }

    public void ReturnObject(IPoolObject obj)
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
        private Queue<IPoolObject> pool = new Queue<IPoolObject>();
        private LinkedList<IPoolObject> beingUsed = new LinkedList<IPoolObject>();

        public T GetObject<T>() where T : IPoolObject
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

        public IPoolObject GetObject(Type type)
        {
            if (!typeof(IPoolObject).IsAssignableFrom(type))
            {
                Debug.LogError($"The type is invalid! Type: {type}");
                return null;
            }

            IPoolObject target = null;
            if (pool.Count > 0)
            {
                target = pool.Dequeue() as IPoolObject;
            }
            else
            {
                target = Activator.CreateInstance(type) as IPoolObject;
            }

            beingUsed.AddLast(target);
            return target;
        }

        public void ReturnObject(IPoolObject obj)
        {
            obj.Clear();

            beingUsed.Remove(obj);
            pool.Enqueue(obj);
        }
    }
}
