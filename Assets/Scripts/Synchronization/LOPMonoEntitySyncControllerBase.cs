using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public abstract class LOPMonoEntitySyncControllerBase<T> : LOPMonoEntityComponentBase, ISyncController<T> where T : ISyncData
{
    public string ControllerId { get; private set; }
    public string OwnerId { get; private set; }
    public bool HasAuthority => OwnerId == LOP.Application.UserId || OwnerId == "local";
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
        ControllerId = $"{Entity.EntityID}_{GetType().Name}";
        OwnerId = Entity.OwnerId;

        SyncControllerManager.Instance.Register(this);

        SceneMessageBroker.AddSubscriber<CS_SyncController>(OnSyncController).Where(syncController => syncController.syncControllerData.controllerId == ControllerId);
        SceneMessageBroker.AddSubscriber<CS_Synchronization>(OnSynchronization).Where(synchronization => synchronization.syncDataEntry.meta.controllerId == ControllerId);
    }

    public virtual void OnFinalize()
    {
        SyncControllerManager.Instance.Unregister(this);

        SceneMessageBroker.RemoveSubscriber<CS_SyncController>(OnSyncController);
        SceneMessageBroker.RemoveSubscriber<CS_Synchronization>(OnSynchronization);
    }

    private void OnSyncController(CS_SyncController syncController)
    {
        //  accept change of ownerId from client
        Debug.Log($"Accept change of ownerId from client. controllerId: {syncController.syncControllerData.controllerId}, ownerId: {syncController.syncControllerData.ownerId}");

        OwnerId = syncController.syncControllerData.ownerId;

        //  broadcast change of ownerId
        var sc_syncController = ObjectPool.Instance.GetObject<SC_SyncController>();
        sc_syncController.syncControllerData = syncController.syncControllerData;

        switch (SyncScope)
        {
            case SyncScope.Local: RoomNetwork.Instance.SendToNear(sc_syncController, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true); break;
            case SyncScope.Global: RoomNetwork.Instance.SendToAll(sc_syncController, instant: true); break;
        }
    }

    private void OnSynchronization(CS_Synchronization synchronization)
    {
        if (OwnerId != synchronization.syncDataEntry.meta.userId)
        {
            Debug.LogWarning($"User (not owner) request synchronization. It is ignored. userId: {synchronization.syncDataEntry.meta.userId}, ownerId: {OwnerId}");
            return;
        }

        if (synchronization.syncDataEntry.meta.userId == LOP.Application.UserId)
        {
            return;
        }

        OnSync(synchronization.syncDataEntry);
        OnSync((T)synchronization.syncDataEntry.data);

        //  broadcast to clients
        var sc_synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
        sc_synchronization.syncDataEntry = synchronization.syncDataEntry;

        switch (SyncScope)
        {
            case SyncScope.Local: RoomNetwork.Instance.SendToNear(sc_synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true); break;
            case SyncScope.Global: RoomNetwork.Instance.SendToAll(sc_synchronization, instant: true); break;
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
        var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
        synchronization.syncDataEntry = new SyncDataEntry
        {
            meta = new SyncDataMeta(Game.Current.CurrentTick, LOP.Application.UserId, ControllerId, value.ObjectToHash()),
            data = value,
        };

        switch (SyncScope)
        {
            case SyncScope.Local: RoomNetwork.Instance.SendToNear(synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true); break;
            case SyncScope.Global: RoomNetwork.Instance.SendToAll(synchronization, instant: true); break;
        }
    }
    
    public virtual void OnSync(T value) { }
    public virtual void OnSync(SyncDataEntry value) { }
}