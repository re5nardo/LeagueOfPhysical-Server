using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using NetworkModel.Mirror;

namespace LOP
{
    public partial class Game : GameFramework.Game
    {
        public const float BROADCAST_SCOPE = 80f;
        public const float BROADCAST_SCOPE_RADIUS = BROADCAST_SCOPE * 0.5f;

        public new static Game Current => GameFramework.Game.Current as Game;

        private Dictionary<string, int> playerUserIDEntityID = new Dictionary<string, int>();                                  //  key : Player UserID, vlue : EntityID
        private Dictionary<int, string> entityIDPlayerUserID = new Dictionary<int, string>();                                  //  key : EntityID, vlue : Player UserID
        private Dictionary<string, WeakReference> playerUserIDPhotonPlayer = new Dictionary<string, WeakReference>();          //  key : Player UserID, vlue : PhotonPlayer

        public ReadOnlyDictionary<string, int> PlayerUserIDEntityID { get; private set; }
        public ReadOnlyDictionary<int, string> EntityIDPlayerUserID { get; private set; }
        public ReadOnlyDictionary<string, WeakReference> PlayerUserIDPhotonPlayer { get; private set; }

        public GameEventManager GameEventManager => gameEventManager;
        public GameManager GameManager => gameManager;

        private GameEventManager gameEventManager = null;
        private GameManager gameManager = null;

        public override IEnumerator Initialize()
        {
            Physics.autoSimulation = false;

            PlayerUserIDEntityID = new ReadOnlyDictionary<string, int>(playerUserIDEntityID);
            EntityIDPlayerUserID = new ReadOnlyDictionary<int, string>(entityIDPlayerUserID);
            PlayerUserIDPhotonPlayer = new ReadOnlyDictionary<string, WeakReference>(playerUserIDPhotonPlayer);

            tickUpdater = gameObject.AddComponent<LOPTickUpdater>();
            gameEventManager = gameObject.AddComponent<GameEventManager>();
            gameManager = gameObject.AddComponent<GameManager>();

            SceneMessageBroker.AddSubscriber<CS_RequestEmotionExpression>(CS_RequestEmotionExpressionHandler.Handle);
            SceneMessageBroker.AddSubscriber<CS_Synchronization>(CS_SynchronizationHandler.Handle);
            SceneMessageBroker.AddSubscriber<RoomMessage.PlayerEnter>(OnPlayerEnter);
            SceneMessageBroker.AddSubscriber<RoomMessage.PlayerLeave>(OnPlayerLeave);

            tickUpdater.Initialize(1 / 30f, false, 0, OnTick, OnTickEnd, OnUpdateElapsedTime);

            EntityInfoSender.Instantiate();
            EntityManager.Instantiate();
            ResourcePool.Instantiate();

            Initialized = true;

            yield break;
        }

        protected override void Clear()
        {
            base.Clear();

            Physics.autoSimulation = true;

            playerUserIDEntityID = null;
            entityIDPlayerUserID = null;
            playerUserIDPhotonPlayer = null;

            SceneMessageBroker.RemoveSubscriber<CS_RequestEmotionExpression>(CS_RequestEmotionExpressionHandler.Handle);
            SceneMessageBroker.RemoveSubscriber<CS_Synchronization>(CS_SynchronizationHandler.Handle);
            SceneMessageBroker.RemoveSubscriber<RoomMessage.PlayerEnter>(OnPlayerEnter);
            SceneMessageBroker.RemoveSubscriber<RoomMessage.PlayerLeave>(OnPlayerLeave);
        }

        protected override void OnBeforeRun()
        {
            GameManager.StartGame();
        }

        private void OnTick(int tick)
        {
            SceneMessageBroker.Publish(new TickMessage.EarlyTick(tick));
            SceneMessageBroker.Publish(new TickMessage.Tick(tick));
            SceneMessageBroker.Publish(new TickMessage.LateTick(tick));
        }

        private void OnTickEnd(int tick)
        {
            SceneMessageBroker.Publish(new TickMessage.EarlyTickEnd(tick));
            SceneMessageBroker.Publish(new TickMessage.TickEnd(tick));
            SceneMessageBroker.Publish(new TickMessage.LateTickEnd(tick));

            if (gameManager.IsGameEnd)
            {
            }
        }

        private void OnUpdateElapsedTime(float time)
        {
            SceneMessageBroker.Publish(new TickMessage.BeforePhysicsSimulation(CurrentTick));

            Physics.Simulate(time);

            SceneMessageBroker.Publish(new TickMessage.AfterPhysicsSimulation(CurrentTick));
        }
    }
}
