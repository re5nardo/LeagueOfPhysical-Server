using System.Collections;
using GameFramework;
using UnityEngine;
using Mirror;

namespace LOP
{
    public class Room : MonoSingleton<Room>
    {
        public string RoomId { get; private set; }
        public string MatchId { get; private set; }
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
         
            LOPWebAPI.UpdateRoomStatus(new UpdateRoomStatusRequest
            {
                roomId = LOP.Room.Instance.RoomId,
                status = NetworkModel.RoomStatus.Ready,
            });

            LOPWebAPI.MatchStart(new MatchStartRequest
            {
                matchId = LOP.Room.Instance.MatchId,
            });

            game.Run();

            InvokeRepeating("SendHeartbeat", 0, 7);

            yield return new WaitUntil(() => game.IsGameEnd);

            LOPWebAPI.MatchEnd(new MatchEndRequest
            {
                matchId = LOP.Room.Instance.MatchId,
            });

            yield return new WaitForSeconds(2);

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

            var roomId = arguments[1];
            var matchId = arguments[2];
            var port = ushort.Parse(arguments[3]);

            var getMatch = LOPWebAPI.GetMatch(matchId);
            yield return getMatch;

            if (getMatch.isError)
            {
                throw new System.Exception(getMatch.error);
            }

            RoomId = roomId;
            MatchId = matchId;
            Port = port;
            ExpectedPlayerList = getMatch.response.match.playerList;

            SceneDataContainer.Get<MatchData>().matchId = getMatch.response.match.id;
            SceneDataContainer.Get<MatchData>().matchSetting = new MatchSetting
            {
                matchType = getMatch.response.match.matchType,
                subGameId = getMatch.response.match.subGameId,
                mapId = getMatch.response.match.mapId,
            };
#else

            RoomId = "EditorTestRoom";
            MatchId = "EditorTestMatch";
            Port = 7777;
            ExpectedPlayerList = new string[]
            {
                "375f9694a1e5c3af13ff9c75e11e1cb158f65521",
            };

            SceneDataContainer.Get<MatchData>().matchId = "EditorTestMatch";
            SceneDataContainer.Get<MatchData>().matchSetting = new MatchSetting
            {
                matchType = MatchType.Friendly,
                subGameId = "FlapWang",
                mapId = "FlapWangMap",
            };
#endif

            UpdateRoomStatusRequest request = new UpdateRoomStatusRequest
            {
                roomId = LOP.Room.Instance.RoomId,
                status = NetworkModel.RoomStatus.Spawned,
            };

            LOPWebAPI.UpdateRoomStatus(request);

            yield return game.Initialize();
        }

        private void Clear()
        {
            SceneMessageBroker.RemoveSubscriber<RoomMessage.PlayerEnter>(OnPlayerEnter);
            SceneMessageBroker.RemoveSubscriber<RoomMessage.PlayerLeave>(OnPlayerLeave);
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
