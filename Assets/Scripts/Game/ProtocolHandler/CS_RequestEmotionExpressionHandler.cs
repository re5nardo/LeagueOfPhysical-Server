using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public class CS_RequestEmotionExpressionHandler
{
    public static void Handle(CS_RequestEmotionExpression requestEmotionExpression)
    {
        IEntity senderEntity = Entities.Get(requestEmotionExpression.entityId);

        EmotionExpressionData emotionExpressionData = senderEntity.GetEntityComponent<EmotionExpressionData>();
        if (!emotionExpressionData.m_listEmotionExpressionID.Exists(x => x == requestEmotionExpression.emotionExpressionId))
        {
            Debug.LogWarning("Invalid nEmotionExpressionID! nEmotionExpressionID : " + requestEmotionExpression.emotionExpressionId);
            return;
        }

        using var disposer = PoolObjectDisposer<SC_EmotionExpression>.Get();
        var emotionExpression = disposer.PoolObject;
        emotionExpression.entityId = senderEntity.EntityID;
        emotionExpression.emotionExpressionId = requestEmotionExpression.emotionExpressionId;

        RoomNetwork.Instance.SendToNear(emotionExpression, senderEntity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS);
    }
}
