using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameIdMap
{
    private Dictionary<string, int> userIdEntityId = new Dictionary<string, int>();
    private Dictionary<int, string> entityIdUserId = new Dictionary<int, string>();

    public void Add(string userId, int entityId)
    {
        userIdEntityId[userId] = entityId;
        entityIdUserId[entityId] = userId;
    }

    public void Remove(string userId)
    {
        var entityId = userIdEntityId[userId];

        entityIdUserId.Remove(entityId);
        userIdEntityId.Remove(userId);
    }

    public void Clear()
    {
        entityIdUserId.Clear();
        userIdEntityId.Clear();
    }

    public bool TryGetUserId(int entityId, out string userId)
    {
        if (!entityIdUserId.TryGetValue(entityId, out userId))
        {
            Debug.LogWarning($"There is no userId matched with the entityId. entityId: {entityId}");
            userId = default;
            return false;
        }

        return true;
    }

    public bool TryGetEntityId(string userId, out int entityId)
    {
        if (!userIdEntityId.TryGetValue(userId, out entityId))
        {
            Debug.LogWarning($"There is no entityId matched with the userId. userId: {userId}");
            entityId = default;
            return false;
        }

        return true;
    }

    public string[] GetAllUserIds()
    {
        return userIdEntityId.Keys.ToArray();
    }

    #region Helper
    public static bool TryGetConnectionIdByEntityId(int entityId, out int connectionId)
    {
        if (!LOP.Game.Current.gameIdMap.TryGetUserId(entityId, out var userId))
        {
            Debug.LogWarning($"There is no userId matched with the entityId. entityId: {entityId}");
            connectionId = default;
            return false;
        }

        return LOP.Room.Instance.roomIdMap.TryGetConnectionId(userId, out connectionId);
    }

    public static bool TryGetEntityIdByConnectionId(int connectionId, out int entityId)
    {
        if (!LOP.Room.Instance.roomIdMap.TryGetUserId(connectionId, out var userId))
        {
            Debug.LogWarning($"There is no userId matched with the connectionId. connectionId: {connectionId}");
            entityId = default;
            return false;
        }

        return LOP.Game.Current.gameIdMap.TryGetEntityId(userId, out entityId);
    }

    public static string[] UserIds => LOP.Game.Current.gameIdMap.GetAllUserIds();
    #endregion
}
