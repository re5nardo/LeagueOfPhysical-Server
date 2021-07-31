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
        public MatchSetting MatchSetting { get; private set; }

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
            var arguments = Environment.GetCommandLineArgs();
            RoomId = arguments[1];
            MatchId = arguments[2];
            Port = ushort.Parse(arguments[3]);

            var matchSetting = new MatchSetting();
            matchSetting.matchType = Util.TryEnumParse(arguments[4], MatchType.Friendly);
            matchSetting.subGameId = arguments[5];
            matchSetting.mapId = arguments[6];

            MatchSetting = matchSetting;

            string[] expectedUsers = new string[arguments.Length - 7];
            for (int i = 7; i < arguments.Length; ++i)
            {
                expectedUsers[i - 7] = arguments[i];
            }
            ExpectedPlayerList = expectedUsers;
#else
            RoomId = "EditorTestRoom";
            MatchId = "EditorTestMatch";
            Port = 7777;

            var matchSetting = new MatchSetting();
            matchSetting.matchType = MatchType.Friendly;
            matchSetting.subGameId = "JumpWang";
            matchSetting.mapId = "Space";

            MatchSetting = matchSetting;
#endif

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
