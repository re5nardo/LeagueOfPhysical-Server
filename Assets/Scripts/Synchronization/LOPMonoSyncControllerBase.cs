using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public abstract class LOPMonoSyncControllerBase<T> : LOPMonoEntityComponentBase, ISyncController<T> where T : ISyncData
{
    public int EntityId => Entity.EntityID;
    public string OwnerId { get; private set; }
    public bool HasAuthority => OwnerId == LOP.Application.UserId || OwnerId == "local";
    public bool IsDirty { get; private set; }

    protected override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        OwnerId = Entity.OwnerId;

        SceneMessageBroker.AddSubscriber<CS_SyncController>(OnSyncController).Where(syncController => syncController.syncControllerData.type == GetType().Name && syncController.syncControllerData.entityId == Entity.EntityID);
        SceneMessageBroker.AddSubscriber<CS_Synchronization>(OnSynchronization).Where(synchronization => synchronization.syncDataEntry.meta.type == GetType().Name && synchronization.syncDataEntry.meta.entityId == Entity.EntityID);
    }

    protected override void OnDetached()
    {
        base.OnDetached();

        SceneMessageBroker.RemoveSubscriber<CS_SyncController>(OnSyncController);
        SceneMessageBroker.RemoveSubscriber<CS_Synchronization>(OnSynchronization);
    }

    private void OnSyncController(CS_SyncController syncController)
    {
        //  accept change of ownerId from client
        Debug.Log($"Accept change of ownerId from client. entityId: {syncController.syncControllerData.entityId}, ownerId: {syncController.syncControllerData.ownerId}");

        OwnerId = syncController.syncControllerData.ownerId;

        //  broadcast change of ownerId
        var sc_syncController = ObjectPool.Instance.GetObject<SC_SyncController>();
        sc_syncController.syncControllerData = new SyncControllerData(GetType().Name, Entity.EntityID, OwnerId);

        RoomNetwork.Instance.SendToNear(sc_syncController, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true);
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

        RoomNetwork.Instance.SendToNear(sc_synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true);
    }

    public abstract T GetSyncData();

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
        synchronization.syncDataEntry = new SyncDataEntry();
        synchronization.syncDataEntry.meta = new SyncDataMeta(Game.Current.CurrentTick, GetType().Name, LOP.Application.UserId, Entity.EntityID, value.ObjectToHash());
        synchronization.syncDataEntry.data = value;
       
        RoomNetwork.Instance.SendToNear(synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true);
    }
    
    public virtual void OnSync(T value) { }
    public virtual void OnSync(SyncDataEntry value) { }
}
