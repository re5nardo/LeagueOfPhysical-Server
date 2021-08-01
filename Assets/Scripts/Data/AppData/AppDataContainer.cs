using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;

public class AppDataContainer : MonoSingleton<AppDataContainer>
{
    private Dictionary<Type, object> dataComponents = new Dictionary<Type, object>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public static T Get<T>()
    {
        return Instance.GetInternal<T>();
    }

    public static void Remove<T>()
    {
        Instance.RemoveInternal<T>();
    }

    private T GetInternal<T>()
    {
        return (T)GetInternal(typeof(T));
    }

    private object GetInternal(Type type)
    {
        if (dataComponents.TryGetValue(type, out var dataComponent))
        {
            return dataComponent;
        }

        object target;
        if (type.IsSubclassOf(typeof(Component)))
        {
            target = gameObject.AddComponent(type);
        }
        else
        {
            target = Activator.CreateInstance(type);
        }

        dataComponents.Add(type, target);

        return target;
    }

    private void RemoveInternal<T>()
    {
        RemoveInternal(typeof(T));
    }

    private void RemoveInternal(Type type)
    {
        dataComponents.Remove(type);
    }
}
