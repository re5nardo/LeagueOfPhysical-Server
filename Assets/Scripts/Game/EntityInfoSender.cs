﻿using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;
using Entity;
using NetworkModel.Mirror;
using Mirror;

public class EntityInfoSender : MonoSingleton<EntityInfoSender>
{
    //private Dictionary<string, Vector3> m_dicPlayerUserIDLookAtPosition = new Dictionary<string, Vector3>();                    //  key : Player UserID, vlue : LookAtPosition

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
    }

    private void SendPlayerSkillInfo()
    {
        foreach (var connectionId in NetworkServer.connections.Keys)
        {
            if (GameIdMap.TryGetEntityIdByConnectionId(connectionId, out var entityId))
            {
                var character = Entities.Get<Character>(entityId);

                using var disposer = PoolObjectDisposer<SC_EntitySkillInfo>.Get();
                var entitySkillInfo = disposer.PoolObject;
                entitySkillInfo.entityId = character.EntityId;
                entitySkillInfo.dicSkillInfo = character.SkillController.GetEntitySkillInfo();

                RoomNetwork.Instance.Send(entitySkillInfo, connectionId);
            }
        }
    }

    public void SetPlayerLookAtPosition(string userId, Vector3 position)
    {
        //m_dicPlayerUserIDLookAtPosition[userId] = position;
    }

	#region Message Handler
    private void OnEntityDestroy(GameMessage.EntityDestroy message)
    {
        var entity = Entities.Get<LOPMonoEntityBase>(message.entityId);

        //	Player's entity
        if (entity.EntityRole == EntityRole.Player)
        {
            //if (LOP.Game.Current.EntityIDPlayerUserID.TryGetValue(message.entityId, out var playerUserID))
            //{
            //    m_dicPlayerUserIDLookAtPosition.Remove(playerUserID);
            //}
        }
    }
	#endregion
}
