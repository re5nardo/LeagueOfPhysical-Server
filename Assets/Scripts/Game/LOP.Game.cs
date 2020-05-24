using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;
using System.Linq;

namespace LOP
{
    public partial class Game : GameFramework.Game
    {
        public const float BROADCAST_SCOPE = 80f;
        public const float BROADCAST_SCOPE_RADIUS = BROADCAST_SCOPE * 0.5f;

        public new static Game Current { get { return Current as Game; } }

        private GameEventManager gameEventManager;
        public GameEventManager GameEventManager { get { return gameEventManager; } }

        public override void Initialize()
        {
            tickUpdater = gameObject.AddComponent<TickUpdater>();
            gameEventManager = gameObject.AddComponent<GameEventManager>();

            tickUpdater.Initialize(1 / 30f, false, OnTick, OnTickEnd);
        }

        protected override void OnBeforeRun()
        {
            SpawnManager.Instance.StartSpawn();
        }

        private void OnTick(int tick)
        {
            var entities = EntityManager.Instance.GetAllEntities().Cast<MonoEntityBase>().ToList();

            //  sort
            //  ...

            entities.ForEach(entity =>
            {
                //  Iterating중에 Entity가 Destroy 안되었는지 체크
                if (entity.IsValid)
                {
                    entity.Tick(tick);
                }
            });

            entities.ForEach(entity =>
            {
                if (entity.IsValid)
                {
                    entity.OnBeforePhysicsSimulation(tick);
                }
            });

            Physics.Simulate(TickInterval);

            entities.ForEach(entity =>
            {
                if (entity.IsValid)
                {
                    entity.OnAfterPhysicsSimulation(tick);
                }
            });

            EntityInfoSender.Instance.Tick(tick);
            SpawnManager.Instance.Tick(tick);
        }

        private void OnTickEnd(int tick)
        {
            //BroadCastGameEvent();

            //gameEvents.Clear();
        }
    }
}
