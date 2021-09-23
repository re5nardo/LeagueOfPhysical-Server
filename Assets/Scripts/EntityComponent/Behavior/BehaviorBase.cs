using System.Linq;
using GameFramework;
using UnityEngine;
using System;
using GameEvent;

namespace Behavior
{
    public abstract class BehaviorBase : LOPMonoEntityComponentBase
    {
        public event Action<BehaviorBase> onBehaviorEnd = null;

        protected virtual void OnBehaviorStart() { }
        protected abstract bool OnBehaviorUpdate();			//  False : Finish
        protected virtual void OnBehaviorEnd() { }

        public int MasterDataId { get; protected set; } = -1;
        public bool IsPlaying { get; private set; } = false;

        protected int startTick = -1;
        protected int lastTick = -1;

        protected float DeltaTime => Game.Current.CurrentTick == 0 ? 0 : Game.Current.TickInterval;
        protected float CurrentUpdateTime => Game.Current.CurrentTick == 0 ? 0 : (Game.Current.CurrentTick - startTick + 1) * Game.Current.TickInterval;
        protected float LastUpdateTime => lastTick == -1 ? -1 : (lastTick - startTick + 1) * Game.Current.TickInterval;

        private MasterData.Behavior masterData = null;
        public MasterData.Behavior MasterData
        {
            get
            {
                if (masterData == null)
                {
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Behavior>(MasterDataId);
                }

                return masterData;
            }
        }

        public virtual void SetData(int masterDataId, params object[] param)
        {
            this.MasterDataId = masterDataId;
        }

        public void StartBehavior()
        {
            if (IsPlaying == true)
            {
                Debug.LogWarning("Behavior is playing, StartBehavior() is ignored!");
                return;
            }

            IsPlaying = true;

            StopPreviousBehaviors();
         
            startTick = Game.Current.CurrentTick;
            lastTick = -1;

            OnBehaviorStart();

            OnTick(Game.Current.CurrentTick);
        }

        private void StopPreviousBehaviors()
        {
            var behaviors = GetComponents<BehaviorBase>().ToList();
            behaviors.Remove(this);
            behaviors.RemoveAll(x => !x.IsPlaying);

            foreach (var behavior in behaviors)
            {
                if (!MasterData.CompatibleBehaviorIDs.Contains(behavior.MasterDataId))
                {
                    behavior.StopBehavior();
                }
            }
        }

        public void OnTick(int tick)
        {
            if (lastTick == tick)
            {
                return;
            }

            if (!OnBehaviorUpdate())
            {
                EndBehavior();
            }

            lastTick = tick;
        }

        private void EndBehavior()
        {
            IsPlaying = false;

            LOP.Game.Current?.GameEventManager.SendToNear(new EntityBehaviorEnd(Entity.EntityID, MasterData.ID), Entity.Position);

            OnBehaviorEnd();

            onBehaviorEnd?.Invoke(this);

            onBehaviorEnd = null;
        }

        public void StopBehavior()
        {
            if (IsPlaying == false)
            {
                Debug.LogWarning("Behavior is not playing, StopBehavior() is ignored!");
                return;
            }

            EndBehavior();
        }

        private void OnDisable()
        {
            if (IsPlaying)
            {
                StopBehavior();
            }
        }
    }
}
