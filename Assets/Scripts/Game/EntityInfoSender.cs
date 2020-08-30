using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;
using Entity;

public class EntityInfoSender : MonoSingleton<EntityInfoSender>
{
    private Dictionary<string, Vector3> m_dicPlayerUserIDLookAtPosition = new Dictionary<string, Vector3>();                    //  key : Player UserID, vlue : LookAtPosition

#region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();

        GamePubSubService.AddSubscriber(GameMessageKey.EntityDestroy, OnEntityDestroy);

        TickPubSubService.AddSubscriber("TickEnd", OnTickEnd);
    }

    private void OnDestroy()
    {
        GamePubSubService.RemoveSubscriber(GameMessageKey.EntityDestroy, OnEntityDestroy);

        TickPubSubService.RemoveSubscriber("TickEnd", OnTickEnd);
    }
    #endregion

    private void OnTickEnd(int tick)
    {
        SendInfo();
    }

    public void SendInfo()
    {
        SendTransformInfo();
        SendPlayerSkillInfo();
        SendSyncTick();
    }

    private void SendTransformInfo()
    {
        SendNearEntityTransformInfo();
    }

    private void SendNearEntityTransformInfo()
    {
        foreach (KeyValuePair<string, WeakReference> kv in LOP.Game.Current.PlayerUserIDPhotonPlayer)
        {
            if (!kv.Value.IsAlive)
            {
                continue;
            }

            string strPlayerUserID = kv.Key;
            PhotonPlayer photonPlayer = kv.Value.Target as PhotonPlayer;
            IEntity playerEntity = EntityManager.Instance.GetEntity(LOP.Game.Current.PlayerUserIDEntityID[strPlayerUserID]);
            if (playerEntity == null)
            {
                //	Already removed
                continue;
            }
            NearEntityAgent nearEntityAgent = playerEntity.GetComponent<NearEntityAgent>();

            //  Get target entities
            List<EntityTransformInfo> listEntityTransformInfo = new List<EntityTransformInfo>();
            Vector3 vec3Center = m_dicPlayerUserIDLookAtPosition.ContainsKey(strPlayerUserID) ? m_dicPlayerUserIDLookAtPosition[strPlayerUserID] : Vector3.zero;
            var entities = EntityManager.Instance.GetEntities(vec3Center, LOP.Game.BROADCAST_SCOPE_RADIUS, EntityRole.All);
            foreach (IEntity candidate in entities)
            {
                if (nearEntityAgent.m_EntityTransformSnaps.ContainsKey(candidate.EntityID))
                {
                    EntityTransformSnap entityTransformSnap = nearEntityAgent.m_EntityTransformSnaps[candidate.EntityID];
                    if (entityTransformSnap.HasChanged)
                    {
                        if (entityTransformSnap.HasVelocityChanged || (Game.Current.CurrentTick - entityTransformSnap.LastSendTick) >= entityTransformSnap.WaitingInterval)
                        {
                            EntityTransformInfo entityTransformInfo = ObjectPool.Instance.GetObject<EntityTransformInfo>();
                            entityTransformInfo.SetEntityTransformInfo(candidate, Game.Current.GameTime);

                            listEntityTransformInfo.Add(entityTransformInfo);

                            entityTransformSnap.LastSendTick = Game.Current.CurrentTick;
                            entityTransformSnap.HasChanged = false;
                        }
                    }
                }
            }

            //  Send packet
            if (listEntityTransformInfo.Count > 0)
            {
                SC_NearEntityTransformInfos nearEntityTransformInfos = ObjectPool.Instance.GetObject<SC_NearEntityTransformInfos>();

                nearEntityTransformInfos.tick = Game.Current.CurrentTick;
                nearEntityTransformInfos.entityTransformInfos = listEntityTransformInfo;

				RoomNetwork.Instance.Send(nearEntityTransformInfos, photonPlayer.ID, false, true);

                ObjectPool.Instance.ReturnObject(nearEntityTransformInfos);
            }
        }
    }

    private void SendPlayerSkillInfo()
    {
        foreach (KeyValuePair<string, WeakReference> kv in LOP.Game.Current.PlayerUserIDPhotonPlayer)
        {
            if (!kv.Value.IsAlive)
            {
                continue;
            }

            string strPlayerUserID = kv.Key;
            PhotonPlayer photonPlayer = kv.Value.Target as PhotonPlayer;
            IEntity entity = EntityManager.Instance.GetEntity(LOP.Game.Current.PlayerUserIDEntityID[strPlayerUserID]);
			if(entity == null)
			{
				//	Already removed
				continue;
			}

			SC_EntitySkillInfo entitySkillInfo = new SC_EntitySkillInfo();
			entitySkillInfo.m_nEntityID = entity.EntityID;
			SkillController controller = entity.GetComponent<SkillController>();
			entitySkillInfo.m_dicSkillInfo = controller.GetEntitySkillInfo();

			RoomNetwork.Instance.Send(entitySkillInfo, photonPlayer.ID);
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
    private void OnEntityDestroy(object[] param)
    {
        int nEntityID = (int)param[0];

        MonoEntityBase entity = EntityManager.Instance.GetEntity(nEntityID) as MonoEntityBase;

        //	Player's entity
        if (entity.EntityRole == EntityRole.Player)
        {
            string playerUserID = LOP.Game.Current.EntityIDPlayerUserID[nEntityID];

            m_dicPlayerUserIDLookAtPosition.Remove(playerUserID);
        }
    }
	#endregion
}
