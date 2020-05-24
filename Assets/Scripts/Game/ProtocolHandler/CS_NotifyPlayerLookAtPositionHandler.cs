using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameFramework;

public class CS_NotifyPlayerLookAtPositionHandler : IHandler<IPhotonEventMessage>
{
    public void Handle(IPhotonEventMessage msg)
    {
        CS_NotifyPlayerLookAtPosition notifyPlayerLookAtPosition = msg as CS_NotifyPlayerLookAtPosition;

        PhotonPlayer photonPlayer = PhotonNetwork.playerList.ToList().Find(x => x.ID == msg.senderID);

        EntityInfoSender.Instance.SetPlayerLookAtPosition(photonPlayer.UserId, notifyPlayerLookAtPosition.m_vec3Position);
    }
}
