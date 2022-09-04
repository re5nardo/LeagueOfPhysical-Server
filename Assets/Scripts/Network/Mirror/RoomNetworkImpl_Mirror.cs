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
        if (!NetworkServer.connections.ContainsKey(targetId))
        {
            Debug.Log($"Target is not connected. targetId: {targetId}");
            return;
        }

        if (!NetworkServer.connections[targetId].isAuthenticated)
        {
            Debug.Log($"Target is not authenticated. targetId: {targetId}");
            return;
        }

        IMirrorMessage mirrorMessage = msg as IMirrorMessage;

        CustomMirrorMessage customMirrorMessage = new CustomMirrorMessage();
        customMirrorMessage.id = mirrorMessage.GetMessageId();
        customMirrorMessage.payload = mirrorMessage;

        NetworkServer.connections[targetId].Send(customMirrorMessage);
    }

    public void Send(IMessage msg, ulong targetId, bool reliable = true, bool instant = false)
    {
        throw new NotImplementedException();
    }

    public void SendToAll(IMessage msg, bool reliable = true, bool instant = false)
    {
        foreach (var connectionId in NetworkServer.connections.Keys)
        {
            Send(msg, connectionId, reliable, instant);
        }
    }

    public void SendToNear(IMessage msg, Vector3 center, float radius, bool reliable = true, bool instant = false)
    {
        foreach (IEntity entity in Entities.Get(center, radius, EntityRole.Player))
        {
            if (GameIdMap.TryGetConnectionIdByEntityId(entity.EntityID, out var connectionId))
            {
                Send(msg, connectionId, reliable, instant);
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
        });
    }

    private void UnregisterMessage()
    {
        NetworkServer.UnregisterHandler<CustomMirrorMessage>();
    }
}
