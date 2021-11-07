using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;
using Entity;
using NetworkModel.Mirror;
using Mirror;

public class EntityInfoSender : MonoSingleton<EntityInfoSender>
{
    private Dictionary<string, Vector3> m_dicPlayerUserIDLookAtPosition = new Dictionary<string, Vector3>();                    //  key : Player UserID, vlue : LookAtPosition

    private const float SEND_SYNC_TICK_INTERVAL = 0.5f;
    private float sendSyncTickTime = 0;

#region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();

        SceneMessageBroker.AddSubscriber<GameMessage.EntityDestroy>(OnEntityDestroy);
        SceneMessageBroker.AddSubscriber<TickMessage.TickEnd>(OnTickEnd);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityDestroy>(OnEntityDestroy);
        SceneMessageBroker.RemoveSubscriber<TickMessage.TickEnd>(OnTickEnd);
    }
    #endregion

    private void OnTickEnd(TickMessage.TickEnd message)
    {
        SendInfo();
    }

    public void SendInfo()
    {
        //SendPlayerSkillInfo();

        sendSyncTickTime += Game.Current.TickInterval;
        if (sendSyncTickTime >= SEND_SYNC_TICK_INTERVAL)
        {
            SendSyncTick();

            sendSyncTickTime -= SEND_SYNC_TICK_INTERVAL;
        }
    }

    private void SendPlayerSkillInfo()
    {
        foreach (var connectionId in NetworkServer.connections.Keys)
        {
            if (IDMap.TryGetEntityIdByConnectionId(connectionId, out var entityId))
            {
                var character = Entities.Get<Character>(entityId);

                var entitySkillInfo = new SC_EntitySkillInfo();
                entitySkillInfo.entityId = character.EntityID;
                entitySkillInfo.dicSkillInfo = character.SkillController.GetEntitySkillInfo();

                RoomNetwork.Instance.Send(entitySkillInfo, connectionId);
            }
        }
    }

    private void SendSyncTick()
    {
        RoomNetwork.Instance.SendToAll(new SC_SyncTick(Game.Current.CurrentTick));
    }

    public void SetPlayerLookAtPosition(string userId, Vector3 position)
    {
        m_dicPlayerUserIDLookAtPosition[userId] = position;
    }

	#region Message Handler
    private void OnEntityDestroy(GameMessage.EntityDestroy message)
    {
        var entity = Entities.Get<LOPMonoEntityBase>(message.entityId);

        //	Player's entity
        if (entity.EntityRole == EntityRole.Player)
        {
            string playerUserID = LOP.Game.Current.EntityIDPlayerUserID[message.entityId];

            m_dicPlayerUserIDLookAtPosition.Remove(playerUserID);
        }
    }
	#endregion
}
