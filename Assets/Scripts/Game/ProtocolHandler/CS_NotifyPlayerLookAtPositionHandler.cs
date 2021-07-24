using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameFramework;
using NetworkModel.PUN;

public class CS_NotifyPlayerLookAtPositionHandler
{
    public static void Handle(IMessage msg)
    {
        CS_NotifyPlayerLookAtPosition notifyPlayerLookAtPosition = msg as CS_NotifyPlayerLookAtPosition;

        PhotonPlayer photonPlayer = PhotonNetwork.playerList.ToList().Find(x => x.ID == notifyPlayerLookAtPosition.senderID);

        EntityInfoSender.Instance.SetPlayerLookAtPosition(photonPlayer.UserId, notifyPlayerLookAtPosition.m_vec3Position);
    }
}
