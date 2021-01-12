using System.Collections;
using GameFramework;
using UnityEngine;

namespace LOP
{
    public class Room : MonoSingleton<Room>
    {
        [SerializeField] private Game game = null;

        private RoomProtocolHandler roomProtocolHandler = null;
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
            roomProtocolHandler = gameObject.AddComponent<RoomProtocolHandler>();
            roomPunBehaviour = gameObject.AddComponent<RoomPunBehaviour>();

            roomProtocolHandler[typeof(CS_PingHandler)] = CS_PingHandler.Handle;

            yield return game.Initialize();
        }

        private void Clear()
        {
        }
    }
}
