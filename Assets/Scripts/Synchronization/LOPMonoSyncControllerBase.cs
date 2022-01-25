using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public abstract class LOPMonoSyncControllerBase<T> : MonoBehaviour, ISyncController<T> where T : ISyncData
{
    public string ControllerId { get; private set; }
    public string OwnerId { get; private set; }
    public bool HasAuthority => OwnerId == LOP.Application.UserId || OwnerId == "local";
    public bool IsDirty { get; private set; }
    public virtual SyncScope SyncScope { get; protected set; } = SyncScope.Local;

    private void Awake()
    {
        OnInitialize();
    }

    private void OnDestroy()
    {
        OnFinalize();
    }

    public virtual void OnInitialize()
    {
        ControllerId = $"{GetType().Name}";
        OwnerId = LOP.Application.UserId;

        SyncControllerManager.Instance.Register(this);

        SceneMessageBroker.AddSubscriber<CS_SyncController>(OnSyncController).Where(syncController => syncController.syncControllerData.controllerId == ControllerId);
        SceneMessageBroker.AddSubscriber<CS_Synchronization>(OnSynchronization).Where(synchronization => synchronization.syncDataEntry.meta.controllerId == ControllerId);
    }

    public virtual void OnFinalize()
    {
        if (SyncControllerManager.HasInstance())
        {
            SyncControllerManager.Instance.Unregister(this);
        }

        if (SceneMessageBroker.HasInstance())
        {
            SceneMessageBroker.RemoveSubscriber<CS_SyncController>(OnSyncController);
            SceneMessageBroker.RemoveSubscriber<CS_Synchronization>(OnSynchronization);
        }
    }

    private void OnSyncController(CS_SyncController syncController)
    {
        //  accept change of ownerId from client
        Debug.Log($"Accept change of ownerId from client. controllerId: {syncController.syncControllerData.controllerId}, ownerId: {syncController.syncControllerData.ownerId}");

        OwnerId = syncController.syncControllerData.ownerId;

        //  broadcast change of ownerId
        using var disposer = PoolObjectDisposer<SC_SyncController>.Get();
        var message = disposer.PoolObject;
        message.syncControllerData = syncController.syncControllerData;

        switch (SyncScope)
        {
            case SyncScope.Global: RoomNetwork.Instance.SendToAll(message, instant: true); break;
        }
    }

    private void OnSynchronization(CS_Synchronization synchronization)
    {
        if (OwnerId != synchronization.syncDataEntry.meta.senderId)
        {
            Debug.LogWarning($"User (not owner) request synchronization. It is ignored. senderId: {synchronization.syncDataEntry.meta.senderId}, ownerId: {OwnerId}");
            return;
        }

        if (synchronization.syncDataEntry.meta.senderId == LOP.Application.UserId)
        {
            return;
        }

        OnSync(synchronization.syncDataEntry);
        OnSync((T)synchronization.syncDataEntry.data);

        //  broadcast to clients
        using var disposer = PoolObjectDisposer<SC_Synchronization>.Get();
        var message = disposer.PoolObject;
        message.syncDataEntry = synchronization.syncDataEntry;

        switch (SyncScope)
        {
            case SyncScope.Global: RoomNetwork.Instance.SendToAll(message, instant: true); break;
        }
    }

    public abstract T GetSyncData();

    public SyncDataEntry GetSyncDataEntry()
    {
        var syncData = GetSyncData();

        return new SyncDataEntry
        {
            meta = new SyncDataMeta(Game.Current.CurrentTick, LOP.Application.UserId, ControllerId, syncData.ObjectToHash()),
            data = syncData,
        };
    }

    public SyncControllerData GetSyncControllerData()
    {
        return new SyncControllerData
        {
            controllerId = ControllerId,
            ownerId = OwnerId,
        };
    }

    public void SetDirty()
    {
        IsDirty = true;
    }

    public void Sync(T value)
    {
        if (!HasAuthority)
        {
            Debug.LogWarning("You must have authority to sync.");
            return;
        }

        //  send syncData to clients
        using var disposer = PoolObjectDisposer<SC_Synchronization>.Get();
        var message = disposer.PoolObject;
        message.syncDataEntry = new SyncDataEntry
        {
            meta = new SyncDataMeta(Game.Current.CurrentTick, LOP.Application.UserId, ControllerId, value.ObjectToHash()),
            data = value,
        };

        switch (SyncScope)
        {
            case SyncScope.Global: RoomNetwork.Instance.SendToAll(message, instant: true); break;
        }
    }

    public virtual void OnSync(T value) { }
    public virtual void OnSync(SyncDataEntry value) { }
}
