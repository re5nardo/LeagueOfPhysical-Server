using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;

public class CS_RequestEmotionExpressionHandler : IHandler<IPhotonEventMessage>
{
    public void Handle(IPhotonEventMessage msg)
    {
        CS_RequestEmotionExpression requestEmotionExpression = msg as CS_RequestEmotionExpression;

        PhotonPlayer photonPlayer = PhotonNetwork.playerList.ToList().Find(x => x.ID == msg.senderID);

        IEntity senderEntity = EntityManager.Instance.GetEntity(LOP.Room.Instance.dicPlayerUserIDEntityID[photonPlayer.UserId]);

        EmotionExpressionData emotionExpressionData = senderEntity.GetComponent<EmotionExpressionData>();
        if (!emotionExpressionData.m_listEmotionExpressionID.Exists(x => x == requestEmotionExpression.m_nEmotionExpressionID))
        {
            Debug.LogWarning("Invalid nEmotionExpressionID! nEmotionExpressionID : " + requestEmotionExpression.m_nEmotionExpressionID);
            return;
        }

        SC_EmotionExpression emotionExpression = new SC_EmotionExpression();
        emotionExpression.m_nEntityID = senderEntity.EntityID;
        emotionExpression.m_nEmotionExpressionID = requestEmotionExpression.m_nEmotionExpressionID;

        RoomNetwork.Instance.SendToNear(emotionExpression, senderEntity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS);
    }
}
