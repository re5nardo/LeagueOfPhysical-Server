using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class RoomProtocolDispatcher : MonoBehaviour
{
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
    }

    private void OnNetworkMessage(IMessage message)
    {
        SceneMessageBroker.Publish(message.GetType(), message);
    }
}
