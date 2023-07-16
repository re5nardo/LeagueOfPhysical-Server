using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;
using System;

public abstract class LOPMonoEntitySyncControllerBase<T> : LOPMonoEntityComponentBase, ISyncController<T> where T : ISyncData
{
    public string ControllerId { get; private set; }
    public string OwnerId => Entity.OwnerId;
    public bool HasAuthority => Entity.HasAuthority;
    public bool IsDirty { get; private set; }
    public virtual SyncScope SyncScope { get; protected set; } = SyncScope.Local;

    protected override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        OnInitialize();
    }

    protected override void OnDetached()
    {
        base.OnDetached();

        OnFinalize();
    }

    public virtual void OnInitialize()
    {
        ControllerId = $"{Entity.EntityId}_{GetType().Name}";

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
        throw new NotSupportedException($"[OnSyncController] controllerId: {syncController.syncControllerData.controllerId}, ownerId: {syncController.syncControllerData.ownerId}");
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
            case SyncScope.Local: RoomNetwork.Instance.SendToNear(message, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true); break;
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
            case SyncScope.Local: RoomNetwork.Instance.SendToNear(message, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true); break;
            case SyncScope.Global: RoomNetwork.Instance.SendToAll(message, instant: true); break;
        }
    }
    
    public virtual void OnSync(T value) { }
    public virtual void OnSync(SyncDataEntry value) { }
}
