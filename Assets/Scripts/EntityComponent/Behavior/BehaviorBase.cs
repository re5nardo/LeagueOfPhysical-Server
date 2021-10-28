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

        private BehaviorMasterData masterData = null;
        public BehaviorMasterData MasterData => masterData ?? (masterData = ScriptableObjectUtil.Get<BehaviorMasterData>(x => x.id == MasterDataId));

        public virtual void Initialize(BehaviorParam behaviorParam)
        {
            this.MasterDataId = behaviorParam.masterDataId;
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
                if (!MasterData.compatibleBehaviors.Any(x => x.id == behavior.MasterDataId))
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

            LOP.Game.Current?.GameEventManager.SendToNear(new EntityBehaviorEnd(Entity.EntityID, MasterData.id), Entity.Position);

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
