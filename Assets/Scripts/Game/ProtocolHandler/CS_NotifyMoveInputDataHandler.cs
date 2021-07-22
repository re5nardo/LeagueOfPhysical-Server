using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public class CS_NotifyMoveInputDataHandler
{
    public static void Handle(IMessage msg)
    {
        var notifyMoveInputData = msg as CS_NotifyMoveInputData;

        IEntity entity = Entities.Get(notifyMoveInputData.playerMoveInput.entityId);
        entity?.GetEntityComponent<PlayerInputController>()?.AddMoveInputData(notifyMoveInputData);
    }
}
