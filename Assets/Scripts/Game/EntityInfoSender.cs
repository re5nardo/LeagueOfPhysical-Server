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

    protected override void OnDestroy()
    {
        base.OnDestroy();

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
        SendPlayerSkillInfo();
        SendSyncTick();
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
            IEntity entity = Entities.Get(LOP.Game.Current.PlayerUserIDEntityID[strPlayerUserID]);
			if(entity == null)
			{
				//	Already removed
				continue;
			}

			SC_EntitySkillInfo entitySkillInfo = new SC_EntitySkillInfo();
			entitySkillInfo.m_nEntityID = entity.EntityID;
			SkillController controller = entity.GetEntityComponent<SkillController>();
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

        MonoEntityBase entity = Entities.Get<MonoEntityBase>(nEntityID);

        //	Player's entity
        if (entity.EntityRole == EntityRole.Player)
        {
            string playerUserID = LOP.Game.Current.EntityIDPlayerUserID[nEntityID];

            m_dicPlayerUserIDLookAtPosition.Remove(playerUserID);
        }
    }
	#endregion
}
