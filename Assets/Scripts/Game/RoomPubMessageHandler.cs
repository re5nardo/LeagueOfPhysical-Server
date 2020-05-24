using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;

public class RoomPubMessageHandler : MonoBehaviour, ISubscriber
{
    private Dictionary<Enum, Action<object[]>> dicPubMessageHandler = new Dictionary<Enum, Action<object[]>>();

    public void Initialize(Dictionary<Enum, Action<object[]>> dicPubMessageHandler)
    {
        this.dicPubMessageHandler = new Dictionary<Enum, Action<object[]>>(dicPubMessageHandler);

        foreach (var key in dicPubMessageHandler.Keys)
        {
            RoomPubSubService.Instance.AddSubscriber(key, this);
        }
    }

    private void OnDestroy()
    {
        if (RoomPubSubService.IsInstantiated())
        {
            foreach (var key in dicPubMessageHandler.Keys)
            {
                RoomPubSubService.Instance.RemoveSubscriber(key, this);
            }
        }

        dicPubMessageHandler.Clear();
    }

    public void OnMessage(Enum key, params object[] param)
    {
        if (dicPubMessageHandler.ContainsKey(key))
        {
            dicPubMessageHandler[key](param);
        }
    }
}
