using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IDMap
{
    public class ConnectionIdUserId
    {
        private static Dictionary<int, string> connectionIdUserId = new Dictionary<int, string>();
        private static Dictionary<string, int> userIdConnectionId = new Dictionary<string, int>();

        public static void Set(int connectionId, string userId)
        {
            connectionIdUserId[connectionId] = userId;
            userIdConnectionId[userId] = connectionId;
        }

        public static void Remove(int connectionId)
        {
            var userId = connectionIdUserId[connectionId];

            userIdConnectionId.Remove(userId);
            connectionIdUserId.Remove(connectionId);
        }

        public static bool TryGetConnectionId(string userId, out int connectionId)
        {
            if (!userIdConnectionId.TryGetValue(userId, out connectionId))
            {
                Debug.LogWarning($"There is no connectionId matched with the userId. userId: {userId}");
                connectionId = int.MinValue;
                return false;
            }

            return true;
        }

        public static bool TryGetUserId(int connectionId, out string userId)
        {
            if (!connectionIdUserId.TryGetValue(connectionId, out userId))
            {
                Debug.LogWarning($"There is no userId matched with the connectionId. connectionId: {connectionId}");
                userId = "";
                return false;
            }

            return true;
        }
    }

    public class UserIdEntityId
    {
        private static Dictionary<string, int> userIdEntityId = new Dictionary<string, int>();
        private static Dictionary<int, string> entityIdUserId = new Dictionary<int, string>();

        public static void Set(string userId, int entityId)
        {
            userIdEntityId[userId] = entityId;
            entityIdUserId[entityId] = userId;
        }

        public static void Remove(string userId)
        {
            var entityId = userIdEntityId[userId];

            entityIdUserId.Remove(entityId);
            userIdEntityId.Remove(userId);
        }

        public static bool TryGetUserId(int entityId, out string userId)
        {
            if (!entityIdUserId.TryGetValue(entityId, out userId))
            {
                Debug.LogWarning($"There is no userId matched with the entityId. entityId: {entityId}");
                userId = "";
                return false;
            }

            return true;
        }

        public static bool TryGetEntityId(string userId, out int entityId)
        {
            if (!userIdEntityId.TryGetValue(userId, out entityId))
            {
                Debug.LogWarning($"There is no entityId matched with the userId. userId: {userId}");
                entityId = int.MinValue;
                return false;
            }

            return true;
        }

        public static string[] GetAllUserIds()
        {
            return userIdEntityId.Keys.ToArray();
        }
    }

    #region Helper
    public static bool TryGetConnectionIdByEntityId(int entityId, out int connectionId)
    {
        if (!UserIdEntityId.TryGetUserId(entityId, out var userId))
        {
            Debug.LogWarning($"There is no userId matched with the entityId. entityId: {entityId}");
            connectionId = int.MinValue;
            return false;
        }

        return ConnectionIdUserId.TryGetConnectionId(userId, out connectionId);
    }

    public static bool TryGetEntityIdByConnectionId(int connectionId, out int entityId)
    {
        if (!ConnectionIdUserId.TryGetUserId(connectionId, out var userId))
        {
            Debug.LogWarning($"There is no userId matched with the connectionId. connectionId: {connectionId}");
            entityId = int.MinValue;
            return false;
        }

        return UserIdEntityId.TryGetEntityId(userId, out entityId);
    }

    public static string[] UserIds => UserIdEntityId.GetAllUserIds();
    #endregion
}
