using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;
using Entity;

public class EntityInfoSender : MonoSingleton<EntityInfoSender>, ISubscriber, ITickable
{
    private Dictionary<string, Vector3> m_dicPlayerUserIDLookAtPosition = new Dictionary<string, Vector3>();                    //  key : Player UserID, vlue : LookAtPosition

    private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

#region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();

        GamePubSubService.Instance.AddSubscriber(GameMessageKey.EntityDestroy, this);

        m_dicMessageHandler.Add(GameMessageKey.EntityDestroy, OnEntityDestroy);
    }

    private void OnDestroy()
    {
        if (GamePubSubService.IsInstantiated())
        {
            GamePubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityDestroy, this);
        }

		m_dicMessageHandler.Clear();
	}
    #endregion

    public void Tick(int tick)
    {
        SendTransformInfo();
        SendPlayerSkillInfos();
    }

    private void SendTransformInfo()
    {
        SendNearEntityTransformInfos();
    }

    private void SendNearEntityTransformInfos()
    {
        foreach (KeyValuePair<string, WeakReference> kv in LOP.Game.Current.PlayerUserIDPhotonPlayer)
        {
            if (!kv.Value.IsAlive)
            {
                continue;
            }

            string strPlayerUserID = kv.Key;
            PhotonPlayer photonPlayer = kv.Value.Target as PhotonPlayer;

            Vector3 vec3Center = m_dicPlayerUserIDLookAtPosition.ContainsKey(strPlayerUserID) ? m_dicPlayerUserIDLookAtPosition[strPlayerUserID] : Vector3.zero;

            SC_NearEntityTransformInfos nearEntityTransformInfos = ObjectPool.Instance.GetObject<SC_NearEntityTransformInfos>();
			List<EntityTransformInfo> listEntityTransformInfo = GetNearEntityTransformInfo(vec3Center, LOP.Game.BROADCAST_SCOPE_RADIUS, EntityRole.All);

            if (listEntityTransformInfo.Count > 0)
            {
                nearEntityTransformInfos.m_listEntityTransformInfo = listEntityTransformInfo;

				RoomNetwork.Instance.Send(nearEntityTransformInfos, photonPlayer.ID, false, true);
            }

            ObjectPool.Instance.ReturnObject(nearEntityTransformInfos);
        }
    }

    private List<EntityTransformInfo> GetNearEntityTransformInfo(Vector3 vec3Position, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        List<EntityTransformInfo> listEntityTransformInfo = new List<EntityTransformInfo>();

        foreach (IEntity entity in EntityManager.Instance.GetEntities(vec3Position, fRadius, entityRoleFlag, hashExceptID))
        {
            TransformAgent transformAgent = entity.GetComponent<TransformAgent>();

            if (transformAgent.HasChanged)
            {
                if (transformAgent.HasVelocityChanged || (Game.Current.CurrentTick - transformAgent.LastSendTick) >= transformAgent.WaitingInterval)
                {
                    EntityTransformInfo entityTransformInfo = ObjectPool.Instance.GetObject<EntityTransformInfo>();
                    entityTransformInfo.SetEntityTransformInfo(entity, Game.Current.GameTime);

                    listEntityTransformInfo.Add(entityTransformInfo);

                    transformAgent.LastSendTick = Game.Current.CurrentTick;
                    transformAgent.HasChanged = false;
                }
            }
        }

        return listEntityTransformInfo;
    }

    private void SendPlayerSkillInfos()
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

    public void SetPlayerLookAtPosition(string userId, Vector3 position)
    {
        m_dicPlayerUserIDLookAtPosition[userId] = position;
    }

    #region ISubscriber
    public void OnMessage(Enum key, params object[] param)
    {
		m_dicMessageHandler[key](param);
	}
	#endregion

	#region Message Handler
    private void OnEntityDestroy(params object[] param)
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
