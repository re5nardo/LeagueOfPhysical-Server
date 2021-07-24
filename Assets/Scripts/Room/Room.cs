using System.Collections;
using GameFramework;
using UnityEngine;
using Mirror;

namespace LOP
{
    public class Room : MonoSingleton<Room>
    {
        [SerializeField] private Game game = null;

        private RoomProtocolDispatcher roomProtocolDispatcher = null;

        #region MonoBehaviour
        private IEnumerator Start()
        {
            yield return Initialize();

            PhotonNetwork.isMessageQueueRunning = true;

            //UNetTransport.ServerListenPort = 7777;
            NetworkManager.singleton.StartServer();

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

            yield return game.Initialize();
        }

        private void Clear()
        {
        }

        private void SendHeartbeat()
        {
            LOPWebAPI.Heartbeat(PhotonNetwork.room.Name);
        }
    }
}
