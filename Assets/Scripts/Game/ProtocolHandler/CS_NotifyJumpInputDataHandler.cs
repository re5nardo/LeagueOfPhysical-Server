using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public class CS_NotifyJumpInputDataHandler
{
    public static void Handle(IMessage msg)
    {
        var notifyJumpInputData = msg as CS_NotifyJumpInputData;

        IEntity entity = Entities.Get(notifyJumpInputData.jumpInputData.entityId);
        entity?.GetEntityComponent<PlayerInputController>()?.AddJumpInputData(notifyJumpInputData);
    }
}
