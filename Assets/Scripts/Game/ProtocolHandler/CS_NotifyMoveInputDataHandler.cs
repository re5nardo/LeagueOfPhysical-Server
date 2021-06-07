using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class CS_NotifyMoveInputDataHandler
{
    public static void Handle(IMessage msg)
    {
        CS_NotifyMoveInputData notifyMoveInputData = msg as CS_NotifyMoveInputData;

        IEntity entity = Entities.Get(notifyMoveInputData.m_PlayerMoveInput.entityID);
        entity?.GetEntityComponent<PlayerInputController>()?.AddMoveInputData(notifyMoveInputData);
    }
}
