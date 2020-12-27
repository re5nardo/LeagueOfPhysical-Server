using System.Collections;
using GameFramework;
using UnityEngine;

namespace LOP
{
    public class Room : MonoSingleton<Room>
    {
        [SerializeField] private Game game = null;

        private RoomProtocolDispatcher roomProtocolDispatcher = null;
        private RoomPunBehaviour roomPunBehaviour = null;

        #region MonoBehaviour
        private IEnumerator Start()
        {
            yield return Initialize();

            PhotonNetwork.isMessageQueueRunning = true;

            game.Run();
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
            roomPunBehaviour = gameObject.AddComponent<RoomPunBehaviour>();

            yield return game.Initialize();

            RoomNetwork.Instance.onMessage += OnNetworkMessage;
        }

        private void Clear()
        {
            if (RoomNetwork.HasInstance())
            {
                RoomNetwork.Instance.onMessage -= OnNetworkMessage;
            }
        }

        private void OnNetworkMessage(IMessage msg, object[] objects)
        {
            roomProtocolDispatcher.DispatchProtocol(msg as IPhotonEventMessage);
        }
    }
}
