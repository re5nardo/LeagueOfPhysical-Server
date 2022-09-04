using System.Collections;
using GameFramework;
using UnityEngine;
using Mirror;

namespace LOP
{
    public class Room : MonoSingleton<Room>
    {
        public string RoomId { get; private set; }
        public ushort Port { get; private set; }
        public string[] ExpectedPlayerList { get; private set; }

        [SerializeField] private Game game;

        private RoomProtocolDispatcher roomProtocolDispatcher;

        public RoomIdMap roomIdMap { get; private set; } = new RoomIdMap();

        #region MonoBehaviour
        private IEnumerator Start()
        {
            yield return Initialize();

            (Transport.activeTransport as kcp2k.KcpTransport).Port = Port;
            NetworkManager.singleton.StartServer();

            while (!NetworkServer.active)
            {
                yield return null;
            }

            UpdateRoomStatusRequest request = new UpdateRoomStatusRequest
            {
                roomId = LOP.Room.Instance.RoomId,
                status = RoomStatus.Ready,
            };

            LOPWebAPI.UpdateRoomStatus(request);

            game.Run();

            InvokeRepeating("SendHeartbeat", 0, 7);

            yield return new WaitUntil(() => game.IsGameEnd);

            Destroy(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Clear();

            LOP.Application.Quit();
        }
        #endregion

        private IEnumerator Initialize()
        {
            roomProtocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();

            SceneMessageBroker.AddSubscriber<RoomMessage.PlayerEnter>(OnPlayerEnter);
            SceneMessageBroker.AddSubscriber<RoomMessage.PlayerLeave>(OnPlayerLeave);

#if !UNITY_EDITOR
            var arguments = System.Environment.GetCommandLineArgs();

            string[] expectedUsers = new string[arguments.Length - 7];
            for (int i = 7; i < arguments.Length; ++i)
            {
                expectedUsers[i - 7] = arguments[i];
            }

            RoomId = arguments[1];
            Port = ushort.Parse(arguments[3]);
            ExpectedPlayerList = expectedUsers;

            SceneDataContainer.Get<MatchData>().matchId = arguments[2];
            SceneDataContainer.Get<MatchData>().matchSetting = new MatchSetting
            {
                matchType = Util.TryEnumParse(arguments[4], MatchType.Friendly),
                subGameId = arguments[5],
                mapId = arguments[6],
            };
#else

            RoomId = "EditorTestRoom";
            Port = 7777;
            ExpectedPlayerList = null;

            SceneDataContainer.Get<MatchData>().matchId = "EditorTestMatch";
            SceneDataContainer.Get<MatchData>().matchSetting = new MatchSetting
            {
                matchType = MatchType.Friendly,
                subGameId = "JumpWang",
                mapId = "Space",
            };
#endif

            UpdateRoomStatusRequest request = new UpdateRoomStatusRequest
            {
                roomId = LOP.Room.Instance.RoomId,
                status = RoomStatus.Spawned,
            };

            LOPWebAPI.UpdateRoomStatus(request);

            yield return game.Initialize();
        }

        private void Clear()
        {
            SceneMessageBroker.RemoveSubscriber<RoomMessage.PlayerEnter>(OnPlayerEnter);
            SceneMessageBroker.RemoveSubscriber<RoomMessage.PlayerLeave>(OnPlayerLeave);

            if (NetworkServer.active)
            {
                NetworkManager.singleton.StopServer();
            }
        }

        private void SendHeartbeat()
        {
            LOPWebAPI.Heartbeat(RoomId);
        }

        private void OnPlayerEnter(RoomMessage.PlayerEnter message)
        {
            var conn = message.networkConnection;
            var customProperties = conn.authenticationData as CustomProperties;

            if (roomIdMap.TryGetConnectionId(customProperties.userId, out var connectionId))
            {
                Debug.LogWarning($"There is alreay connectionId. OnPlayerLeave was called?");
            }

            roomIdMap.Add(conn.connectionId, customProperties.userId);

            Debug.Log($"[OnPlayerEnter] userId: {customProperties.userId}, connectionId: {conn.connectionId}");
        }

        private void OnPlayerLeave(RoomMessage.PlayerLeave message)
        {
            var conn = message.networkConnection;
            var customProperties = conn.authenticationData as CustomProperties;

            if (roomIdMap.TryGetConnectionId(customProperties.userId, out var connectionId) == false)
            {
                Debug.LogWarning($"There is no connectionId. OnPlayerEnter was called?");
            }

            roomIdMap.Remove(conn.connectionId);

            Debug.Log($"[OnPlayerLeave] userId: {customProperties.userId}, connectionId: {conn.connectionId}");
        }
    }
}
