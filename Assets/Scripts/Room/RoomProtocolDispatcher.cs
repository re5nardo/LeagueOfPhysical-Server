using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;

public class RoomProtocolDispatcher : MonoBehaviour
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
        RoomNetwork.Instance.OnMessage += OnNetworkMessage;
    }

    private void OnDestroy()
    {
        if (RoomNetwork.HasInstance())
        {
            RoomNetwork.Instance.OnMessage -= OnNetworkMessage;
        }

        handlers.Clear();
    }

    private void OnNetworkMessage(IMessage msg)
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
