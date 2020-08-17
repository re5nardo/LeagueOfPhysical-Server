using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomProtocolDispatcher : MonoBehaviour
{
    private Dictionary<int, Action<IPhotonEventMessage>> protocolHandlers = new Dictionary<int, Action<IPhotonEventMessage>>();

    private void Awake()
    {
        protocolHandlers.Add(PhotonEvent.CS_Ping, CS_PingHandler.Handle);
    }

    private void OnDestroy()
    {
        protocolHandlers.Clear();
    }

    public void DispatchProtocol(IPhotonEventMessage msg)
    {
        Action<IPhotonEventMessage> handler = null;

        if (protocolHandlers.TryGetValue(msg.GetEventID(), out handler))
        {
            handler?.Invoke(msg);
        }
    }
}
