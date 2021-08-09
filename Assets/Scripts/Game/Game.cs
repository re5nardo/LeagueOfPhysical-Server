﻿using System.Collections;
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

        private RoomProtocolDispatcher roomProtocolDispatcher = null;
        private GameEventManager gameEventManager = null;
        private GameManager gameManager = null;

        public override IEnumerator Initialize()
        {
            Physics.autoSimulation = false;

            PlayerUserIDEntityID = new ReadOnlyDictionary<string, int>(playerUserIDEntityID);
            EntityIDPlayerUserID = new ReadOnlyDictionary<int, string>(entityIDPlayerUserID);
            PlayerUserIDPhotonPlayer = new ReadOnlyDictionary<string, WeakReference>(playerUserIDPhotonPlayer);

            tickUpdater = gameObject.AddComponent<LOPTickUpdater>();
            roomProtocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();
            gameEventManager = gameObject.AddComponent<GameEventManager>();
            gameManager = gameObject.AddComponent<GameManager>();

            roomProtocolDispatcher[typeof(CS_NotifySkillInputData)]         = CS_NotifySkillInputDataHandler.Handle;
            roomProtocolDispatcher[typeof(CS_NotifyJumpInputData)]          = CS_NotifyJumpInputDataHandler.Handle;
            roomProtocolDispatcher[typeof(CS_RequestEmotionExpression)]     = CS_RequestEmotionExpressionHandler.Handle;
            roomProtocolDispatcher[typeof(CS_NotifyMoveInputData)]          = CS_NotifyMoveInputDataHandler.Handle;

            RoomPubSubService.AddSubscriber(RoomMessageKey.PlayerEnter, OnPlayerEnter);
            RoomPubSubService.AddSubscriber(RoomMessageKey.PlayerLeave, OnPlayerLeave);

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
    
            RoomPubSubService.RemoveSubscriber(RoomMessageKey.PlayerEnter, OnPlayerEnter);
            RoomPubSubService.RemoveSubscriber(RoomMessageKey.PlayerLeave, OnPlayerLeave);
        }

        protected override void OnBeforeRun()
        {
            GameManager.StartGame();
        }

        private void OnTick(int tick)
        {
            TickPubSubService.Publish("EarlyTick", tick);
            TickPubSubService.Publish("Tick", tick);
            TickPubSubService.Publish("LateTick", tick);
        }

        private void OnTickEnd(int tick)
        {
            TickPubSubService.Publish("EarlyTickEnd", tick);
            TickPubSubService.Publish("TickEnd", tick);
            TickPubSubService.Publish("LateTickEnd", tick);

            if (gameManager.IsGameEnd)
            {
            }
        }

        private void OnUpdateElapsedTime(float time)
        {
            TickPubSubService.Publish("BeforePhysicsSimulation", CurrentTick);

            Physics.Simulate(time);

            TickPubSubService.Publish("AfterPhysicsSimulation", CurrentTick);
        }
    }
}
