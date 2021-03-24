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

            InvokeRepeating("SendAlive", 0, 7);
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

            roomProtocolDispatcher[typeof(CS_Ping)] = CS_PingHandler.Handle;

            yield return game.Initialize();
        }

        private void Clear()
        {
        }

        private void SendAlive()
        {
            LOPWebAPI.Alive();
        }
    }
}
