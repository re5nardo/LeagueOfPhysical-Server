using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIdMap
{
    private Dictionary<int, string> connectionIdUserId = new Dictionary<int, string>();
    private Dictionary<string, int> userIdConnectionId = new Dictionary<string, int>();

    public void Add(int connectionId, string userId)
    {
        connectionIdUserId[connectionId] = userId;
        userIdConnectionId[userId] = connectionId;
    }

    public void Remove(int connectionId)
    {
        var userId = connectionIdUserId[connectionId];

        userIdConnectionId.Remove(userId);
        connectionIdUserId.Remove(connectionId);
    }

    public bool TryGetConnectionId(string userId, out int connectionId)
    {
        if (!userIdConnectionId.TryGetValue(userId, out connectionId))
        {
            Debug.LogWarning($"There is no connectionId matched with the userId. userId: {userId}");
            connectionId = default;
            return false;
        }

        return true;
    }

    public bool TryGetUserId(int connectionId, out string userId)
    {
        if (!connectionIdUserId.TryGetValue(connectionId, out userId))
        {
            Debug.LogWarning($"There is no userId matched with the connectionId. connectionId: {connectionId}");
            userId = default;
            return false;
        }

        return true;
    }
}
