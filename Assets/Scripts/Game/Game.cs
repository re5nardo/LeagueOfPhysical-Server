using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityEngine.SceneManagement;
using System;
using System.Collections.ObjectModel;

namespace LOP
{
    public partial class Game : GameFramework.Game
    {
        public const float BROADCAST_SCOPE = 80f;
        public const float BROADCAST_SCOPE_RADIUS = BROADCAST_SCOPE * 0.5f;

        public new static Game Current { get { return GameFramework.Game.Current as Game; } }

        private Dictionary<string, int> playerUserIDEntityID = new Dictionary<string, int>();                                  //  key : Player UserID, vlue : EntityID
        private Dictionary<int, string> entityIDPlayerUserID = new Dictionary<int, string>();                                  //  key : EntityID, vlue : Player UserID
        private Dictionary<string, WeakReference> playerUserIDPhotonPlayer = new Dictionary<string, WeakReference>();          //  key : Player UserID, vlue : PhotonPlayer

        public ReadOnlyDictionary<string, int> PlayerUserIDEntityID { get; private set; }
        public ReadOnlyDictionary<int, string> EntityIDPlayerUserID { get; private set; }
        public ReadOnlyDictionary<string, WeakReference> PlayerUserIDPhotonPlayer { get; private set; }

        private GameEventManager gameEventManager;
        public GameEventManager GameEventManager { get { return gameEventManager; } }

        private GameProtocolDispatcher protocolDispatcher = null;

        public override IEnumerator Initialize()
        {
            GameFramework.Game.Current = this;

            yield return SceneManager.LoadSceneAsync("RiftOfSummoner", LoadSceneMode.Additive);

            Physics.autoSimulation = false;

            PlayerUserIDEntityID = new ReadOnlyDictionary<string, int>(playerUserIDEntityID);
            EntityIDPlayerUserID = new ReadOnlyDictionary<int, string>(entityIDPlayerUserID);
            PlayerUserIDPhotonPlayer = new ReadOnlyDictionary<string, WeakReference>(playerUserIDPhotonPlayer);

            ResourcePool.Instantiate();

            protocolDispatcher = gameObject.AddComponent<GameProtocolDispatcher>();

            tickUpdater = gameObject.AddComponent<TickUpdater>();
            gameEventManager = gameObject.AddComponent<GameEventManager>();

            RoomNetwork.Instance.onMessage += OnNetworkMessage;

            RoomPubSubService.AddSubscriber(RoomMessageKey.PlayerEnter, OnPlayerEnter);
            RoomPubSubService.AddSubscriber(RoomMessageKey.PlayerLeave, OnPlayerLeave);

            tickUpdater.Initialize(1 / 30f, false, 0, OnTick, OnTickEnd);

            initialized = true;
        }

        protected override void Clear()
        {
            base.Clear();

            playerUserIDEntityID = null;
            entityIDPlayerUserID = null;
            playerUserIDPhotonPlayer = null;

            if (protocolDispatcher != null)
            {
                Destroy(protocolDispatcher);
                protocolDispatcher = null;
            }

            if (tickUpdater != null)
            {
                Destroy(tickUpdater);
                tickUpdater = null;
            }

            if (gameEventManager != null)
            {
                Destroy(gameEventManager);
                gameEventManager = null;
            }

            RoomPubSubService.RemoveSubscriber(RoomMessageKey.PlayerEnter, OnPlayerEnter);
            RoomPubSubService.RemoveSubscriber(RoomMessageKey.PlayerLeave, OnPlayerLeave);

            if (RoomNetwork.IsInstantiated())
            {
                RoomNetwork.Instance.onMessage -= OnNetworkMessage;
            }
        }

        protected override void OnBeforeRun()
        {
            SpawnManager.Instance.StartSpawn();
            EntityInfoSender.Instantiate();
            EntityManager.Instantiate();
        }

        private void OnTick(int tick)
        {
            TickPubSubService.Publish("EarlyTick", tick);
            TickPubSubService.Publish("Tick", tick);
            TickPubSubService.Publish("LateTick", tick);

            TickPubSubService.Publish("BeforePhysicsSimulation", tick);
           
            Physics.Simulate(TickInterval);

            TickPubSubService.Publish("AfterPhysicsSimulation", tick);
        }

        private void OnTickEnd(int tick)
        {
            TickPubSubService.Publish("TickEnd", tick);
            TickPubSubService.Publish("LateTickEnd", tick);
        }
    }
}
