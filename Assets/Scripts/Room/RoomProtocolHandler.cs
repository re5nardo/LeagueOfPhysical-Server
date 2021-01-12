using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;

public class RoomProtocolHandler : MonoBehaviour
{
    private Dictionary<Type, Action<IMessage>> handlers = new Dictionary<Type, Action<IMessage>>();

    public Action<IMessage> this[Type type]
    {
        get
        {
            if (!handlers.ContainsKey(type))
            {
                handlers.Add(type, default);
            }
            return handlers[type];
        }
        set => handlers[type] = value;
    }

    private void Awake()
    {
        RoomNetwork.Instance.onMessage += OnNetworkMessage;
    }

    private void OnDestroy()
    {
        if (RoomNetwork.HasInstance())
        {
            RoomNetwork.Instance.onMessage -= OnNetworkMessage;
        }

        handlers.Clear();
    }

    private void OnNetworkMessage(IMessage msg, object[] objects)
    {
        if (handlers.TryGetValue(msg.GetType(), out Action<IMessage> handler))
        {
            handler?.Invoke(msg);
        }
    }

    public void Clear()
    {
        handlers.Clear();
    }
}
