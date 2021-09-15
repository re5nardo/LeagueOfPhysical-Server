using System.Linq;
using GameFramework;
using Entity;
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

        protected int behaviorMasterID = -1;
        protected int startTick = -1;
        protected int lastTick = -1;

        private bool isPlaying = false;

        protected float DeltaTime => Game.Current.CurrentTick == 0 ? 0 : Game.Current.TickInterval;

        protected float CurrentUpdateTime
        {
            get
            {
                return Game.Current.CurrentTick == 0 ? 0 : (Game.Current.CurrentTick - startTick + 1) * Game.Current.TickInterval;
            }
        }

        protected float LastUpdateTime
        {
            get
            {
                return lastTick == -1 ? -1 : (lastTick - startTick + 1) * Game.Current.TickInterval;
            }
        }

        new protected LOPMonoEntityBase Entity = null;

        private MasterData.Behavior masterData = null;
        public MasterData.Behavior MasterData
        {
            get
            {
                if (masterData == null)
                {
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Behavior>(behaviorMasterID);
                }

                return masterData;
            }
        }

        #region IComponent
        public override void OnAttached(IEntity entity)
        {
            base.OnAttached(entity);

            Entity = entity as LOPMonoEntityBase;
        }

        public override void OnDetached()
        {
            base.OnDetached();

            Entity = null;
        }
        #endregion

        public virtual void SetData(int behaviorMasterID, params object[] param)
        {
            this.behaviorMasterID = behaviorMasterID;
        }

        public void StartBehavior()
        {
            if (isPlaying == true)
            {
                Debug.LogWarning("Behavior is playing, StartBehavior() is ignored!");
                return;
            }

            isPlaying = true;

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
            behaviors.RemoveAll(x => !x.IsPlaying());

            foreach (var behavior in behaviors)
            {
                if (!MasterData.CompatibleBehaviorIDs.Contains(behavior.GetBehaviorMasterID()))
                {
                    behavior.StopBehavior();
                }
            }
        }

        public void OnTick(int tick)
        {
            if (lastTick == tick)
            {
                //Debug.LogWarning("Tick() is ignored! lastTick == tick");
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
            isPlaying = false;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorEnd(Entity.EntityID, MasterData.ID), Entity.Position);

            OnBehaviorEnd();

            onBehaviorEnd?.Invoke(this);

            onBehaviorEnd = null;
        }

        public void StopBehavior()
        {
            if (isPlaying == false)
            {
                Debug.LogWarning("Behavior is not playing, StopBehavior() is ignored!");
                return;
            }

            EndBehavior();
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public int GetBehaviorMasterID()
        {
            return behaviorMasterID;
        }

        private void OnDisable()
        {
            if (IsPlaying())
            {
                StopBehavior();
            }
        }
    }
}
