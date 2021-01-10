using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using System.Linq;

public class RoomPunBehaviour : PunBehaviour
{
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.room.ExpectedUsers != null && !PhotonNetwork.room.ExpectedUsers.Contains(newPlayer.UserId))
        {
            PhotonNetwork.CloseConnection(newPlayer);

            Debug.LogError("Invalid user enter! UserId : " + newPlayer.UserId);
            return;
        }

        RoomPubSubService.Publish(RoomMessageKey.PlayerEnter, newPlayer);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        RoomPubSubService.Publish(RoomMessageKey.PlayerLeave, otherPlayer);
    }
}
