using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class SyncControllerManager : MonoSingleton<SyncControllerManager>
{
    public readonly List<ISyncControllerBase> syncControllers = new List<ISyncControllerBase>();

    public void Register<T>(ISyncController<T> syncController) where T : ISyncData
    {
        Register(syncController as ISyncControllerBase);
    }

    public void Register(ISyncControllerBase syncController)
    {
        syncControllers.Add(syncController);
    }

    public void Unregister<T>(ISyncController<T> syncController) where T : ISyncData
    {
        Unregister(syncController as ISyncControllerBase);
    }

    public void Unregister(ISyncControllerBase syncController)
    {
        syncControllers.Remove(syncController);
    }
}
