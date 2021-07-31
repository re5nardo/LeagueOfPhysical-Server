using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;

public class SceneDataContainer : MonoSingleton<SceneDataContainer>
{
    private MonoDataContainerImpl containerImpl = null;

    protected override void Awake()
    {
        base.Awake();

        containerImpl = gameObject.AddComponent<MonoDataContainerImpl>();
    }

    public static T Get<T>() where T : IDataComponent
    {
        return Instance.containerImpl.Get<T>();
    }

    public static IDataComponent Get(Type type)
    {
        return Instance.containerImpl.Get(type);
    }

    public static void OnUpdate(IDataSource source)
    {
        Instance.containerImpl.OnUpdate(source);
    }
}
