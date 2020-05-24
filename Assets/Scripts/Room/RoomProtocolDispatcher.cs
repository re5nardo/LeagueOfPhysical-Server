using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;

public class RoomProtocolDispatcher : MonoBehaviour
{
    private Dictionary<int, IHandler<IPhotonEventMessage>> m_dicProtocolHandler = new Dictionary<int, IHandler<IPhotonEventMessage>>();

    private void Awake()
    {
        m_dicProtocolHandler.Add(PhotonEvent.CS_Ping, new CS_PingHandler());
    }

    private void OnDestroy()
    {
        m_dicProtocolHandler.Clear();
    }

    public void DispatchProtocol(IPhotonEventMessage msg)
    {
        if (m_dicProtocolHandler.ContainsKey(msg.GetEventID()))
        {
            m_dicProtocolHandler[msg.GetEventID()].Handle(msg);
        }
    }
}
