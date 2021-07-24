using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;
using NetworkModel.Mirror;

public class CS_RequestEmotionExpressionHandler
{
    public static void Handle(IMessage msg)
    {
        CS_RequestEmotionExpression requestEmotionExpression = msg as CS_RequestEmotionExpression;

        IEntity senderEntity = Entities.Get(requestEmotionExpression.entityId);

        EmotionExpressionData emotionExpressionData = senderEntity.GetEntityComponent<EmotionExpressionData>();
        if (!emotionExpressionData.m_listEmotionExpressionID.Exists(x => x == requestEmotionExpression.emotionExpressionId))
        {
            Debug.LogWarning("Invalid nEmotionExpressionID! nEmotionExpressionID : " + requestEmotionExpression.emotionExpressionId);
            return;
        }

        SC_EmotionExpression emotionExpression = new SC_EmotionExpression();
        emotionExpression.entityId = senderEntity.EntityID;
        emotionExpression.emotionExpressionId = requestEmotionExpression.emotionExpressionId;

        RoomNetwork.Instance.SendToNear(emotionExpression, senderEntity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS);
    }
}
