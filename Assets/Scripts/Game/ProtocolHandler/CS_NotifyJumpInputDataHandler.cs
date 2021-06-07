using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;

public class CS_NotifyJumpInputDataHandler
{
    public static void Handle(IMessage msg)
    {
        CS_NotifyJumpInputData notifyJumpInputData = msg as CS_NotifyJumpInputData;

        IEntity entity = Entities.Get(notifyJumpInputData.jumpInputData.entityID);
        entity?.GetEntityComponent<PlayerInputController>()?.AddJumpInputData(notifyJumpInputData);
    }
}
