using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public class CS_NotifySkillInputDataHandler
{
    public static void Handle(IMessage msg)
    {
        CS_NotifySkillInputData notifySkillInputData = msg as CS_NotifySkillInputData;

        IEntity entity = Entities.Get(notifySkillInputData.skillInputData.entityId);

        foreach (Skill.SkillBase skill in entity.GetEntityComponents<Skill.SkillBase>())
        {
            if (skill.GetSkillMasterID() == notifySkillInputData.skillInputData.skillId)
            {
                skill.OnReceiveSkillInputData(notifySkillInputData.skillInputData);
                return;
            }
        }
    }
}
