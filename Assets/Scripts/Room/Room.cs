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

        [SerializeField] private Game game = null;

        private RoomProtocolDispatcher roomProtocolDispatcher = null;

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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Clear();
        }
        #endregion

        private IEnumerator Initialize()
        {
            roomProtocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();

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
        }

        private void SendHeartbeat()
        {
            LOPWebAPI.Heartbeat(RoomId);
        }
    }
}
