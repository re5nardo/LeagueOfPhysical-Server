using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;

public class CS_NotifySkillInputDataHandler : IHandler<IPhotonEventMessage>
{
    public void Handle(IPhotonEventMessage msg)
    {
        CS_NotifySkillInputData notifySkillInputData = msg as CS_NotifySkillInputData;

        PhotonPlayer photonPlayer = PhotonNetwork.playerList.ToList().Find(x => x.ID == msg.senderID);

        IEntity entity = EntityManager.Instance.GetEntity(LOP.Room.Instance.dicPlayerUserIDEntityID[photonPlayer.UserId]);
        foreach (Skill.SkillBase skill in entity.GetComponents<Skill.SkillBase>())
        {
            if (skill.GetSkillMasterID() == notifySkillInputData.m_SkillInputData.m_nSkillID)
            {
                skill.OnReceiveSkillInputData(notifySkillInputData.m_SkillInputData);
                return;
            }
        }
    }
}
