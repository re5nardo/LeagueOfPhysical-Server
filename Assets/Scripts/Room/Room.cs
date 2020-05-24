using System.Collections;
using Photon;
using GameFramework;

namespace LOP
{
    public class Room : PunBehaviour
    {
        private Game game = null;
        private RoomProtocolDispatcher protocolDispatcher = null;

        public static Room Instance { get; private set; }

        public static bool IsInstantiated()
        {
            return Instance != null;
        }

        #region MonoBehaviour
        private IEnumerator Start()
        {
            Instance = this;

            yield return StartCoroutine(Initialize());

            PhotonNetwork.isMessageQueueRunning = true;

            game.Run();
        }

        private void OnDestroy()
        {
            Clear();

            Instance = null;
        }
        #endregion

        #region PunBehaviour
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            RoomPubSubService.Instance.Publish(RoomMessageKey.PlayerEnter, newPlayer);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            RoomPubSubService.Instance.Publish(RoomMessageKey.PlayerLeave, otherPlayer);
        }
        #endregion

        private IEnumerator Initialize()
        {
            MasterDataManager.Instance.Initialize();

            game = gameObject.AddComponent<Game>();
            yield return StartCoroutine(game.Initialize());

            protocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();
   
            RoomNetwork.Instance.onMessage += OnNetworkMessage;
        }

        private void Clear()
        {
            if (game != null)
            {
                Destroy(game);
                game = null;
            }

            if (protocolDispatcher != null)
            {
                Destroy(protocolDispatcher);
                protocolDispatcher = null;
            }

            if (RoomNetwork.IsInstantiated())
            {
                RoomNetwork.Instance.onMessage -= OnNetworkMessage;
            }
        }

        private void OnNetworkMessage(IMessage msg, object[] objects)
        {
            protocolDispatcher.DispatchProtocol(msg as IPhotonEventMessage);
        }
    }
}
