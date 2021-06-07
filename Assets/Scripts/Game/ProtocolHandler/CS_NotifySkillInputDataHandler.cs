using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;

public class CS_NotifySkillInputDataHandler
{
    public static void Handle(IMessage msg)
    {
        CS_NotifySkillInputData notifySkillInputData = msg as CS_NotifySkillInputData;

        PhotonPlayer photonPlayer = PhotonNetwork.playerList.ToList().Find(x => x.ID == notifySkillInputData.senderID);

        IEntity entity = Entities.Get(LOP.Game.Current.PlayerUserIDEntityID[photonPlayer.UserId]);
        foreach (Skill.SkillBase skill in entity.GetEntityComponents<Skill.SkillBase>())
        {
            if (skill.GetSkillMasterID() == notifySkillInputData.m_SkillInputData.skillID)
            {
                skill.OnReceiveSkillInputData(notifySkillInputData.m_SkillInputData);
                return;
            }
        }
    }
}
