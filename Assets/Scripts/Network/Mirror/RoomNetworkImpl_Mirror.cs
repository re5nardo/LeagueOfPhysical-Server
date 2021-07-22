using UnityEngine;
using System;
using GameFramework;
using Mirror;

public class RoomNetworkImpl_Mirror : MonoBehaviour, INetworkImpl
{
    public Action<IMessage> OnMessage { get; set; }

    private void Awake()
    {
        RegisterMessage();
    }

    private void OnDestroy()
    {
        OnMessage = null;

        UnregisterMessage();
    }

    public void Send(IMessage msg, int targetId, bool reliable = true, bool instant = false)
    {
        IMirrorMessage mirrorMessage = msg as IMirrorMessage;

        CustomMirrorMessage customMirrorMessage = new CustomMirrorMessage();
        customMirrorMessage.id = mirrorMessage.GetMessageId();
        customMirrorMessage.payload = mirrorMessage;

        NetworkServer.SendToAll(customMirrorMessage);
    }

    public void Send(IMessage msg, ulong targetId, bool reliable = true, bool instant = false)
    {
        throw new NotImplementedException();
    }

    public void SendToAll(IMessage msg, bool reliable = true, bool instant = false)
    {
        foreach (var client in NetworkServer.connections.Values)
        {
            Send(msg, client.connectionId, reliable, instant);
        }
    }

    public void SendToNear(IMessage msg, Vector3 center, float radius, bool reliable = true, bool instant = false)
    {
        foreach (IEntity entity in Entities.Get(center, radius, EntityRole.Player))
        {
            if (LOP.Game.Current.EntityIdConnId.TryGetValue(entity.EntityID, out var connId))
            {
                if (NetworkServer.connections.ContainsKey(connId))
                {
                    Send(msg, connId, reliable, instant);
                }
            }
        }
    }

    private void InternalOnMessage(IMessage iMessage)
    {
        OnMessage?.Invoke(iMessage);
    }

    private void RegisterMessage()
    {
        NetworkServer.RegisterHandler<CustomMirrorMessage>((conn, message) =>
        {
            message.sender = conn.connectionId;
            InternalOnMessage(message.payload);
        }, false);
    }

    private void UnregisterMessage()
    {
        NetworkClient.UnregisterHandler<CustomMirrorMessage>();
    }
}
