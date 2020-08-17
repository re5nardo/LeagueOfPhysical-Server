using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class CS_NotifyMoveInputDataHandler
{
    public static void Handle(IPhotonEventMessage msg)
    {
        CS_NotifyMoveInputData notifyMoveInputData = msg as CS_NotifyMoveInputData;

        IEntity entity = EntityManager.Instance.GetEntity(notifyMoveInputData.m_PlayerMoveInput.entityID);
        entity.GetComponent<PlayerMoveInputController>()?.AddPlayerMoveInputController(notifyMoveInputData);
    }
}

