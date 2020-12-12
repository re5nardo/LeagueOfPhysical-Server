using System.Collections;
using GameFramework;
using UnityEngine;

namespace LOP
{
    public class Room : MonoSingleton<Room>
    {
        [SerializeField] private Game game = null;
        [SerializeField] private RoomProtocolDispatcher roomProtocolDispatcher = null;
        [SerializeField] private RoomPunBehaviour roomPunBehaviour = null;

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
