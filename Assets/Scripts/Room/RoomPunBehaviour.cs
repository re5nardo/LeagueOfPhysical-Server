using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class RoomPunBehaviour : PunBehaviour
{
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        RoomPubSubService.Publish(RoomMessageKey.PlayerEnter, newPlayer);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        RoomPubSubService.Publish(RoomMessageKey.PlayerLeave, otherPlayer);
    }
}
