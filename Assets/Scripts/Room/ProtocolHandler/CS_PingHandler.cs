using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class CS_PingHandler : IHandler<IPhotonEventMessage>
{
    public void Handle(IPhotonEventMessage msg)
    {
        CS_Ping ping = msg as CS_Ping;

        RoomNetwork.Instance.Send(new SC_Ping(), ping.senderID, bInstant: true);
    }
}
