using System;
using GameFramework;
using Entity;
using UnityEngine;

namespace State
{
	public abstract class StateBase : MonoComponentBase
    {
        public event Action<StateBase> onStateEnd = null;

        protected virtual void OnStateStart() { }
        protected abstract bool OnStateUpdate();         //  False : Finish
        protected virtual void OnStateEnd() { }          //  중간에 Stop 된 경우에도 호출됨

        protected int m_nStateMasterID = -1;
        protected int startTick = -1;
        protected int lastTick = -1;

        private bool isPlaying = false;

        protected float DeltaTime
        {
            get
            {
                return lastTick == -1 ? CurrentUpdateTime : CurrentUpdateTime - LastUpdateTime;
            }
        }

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

        new protected MonoEntityBase Entity = null;

        private MasterData.State masterData = null;
        public MasterData.State MasterData
        {
            get
            {
                if (masterData == null)
                {
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.State>(m_nStateMasterID);
                }

                return masterData;
            }
        }

        #region IComponent
        public override void OnAttached(IEntity entity)
        {
            base.OnAttached(entity);

            Entity = entity as MonoEntityBase;
        }

        public override void OnDetached()
        {
            base.OnDetached();

            Entity = null;
        }
        #endregion

        public virtual void SetData(int nStateMasterID, params object[] param)
        {
            m_nStateMasterID = nStateMasterID;
        }

        public void StartState()
        {
            if (isPlaying == true)
            {
                Debug.LogWarning("State is playing, StartState() is ignored!");
                return;
            }

            isPlaying = true;

            //SC_EntityBehaviorStartEvent entityBehaviorStartEvent = new SC_EntityBehaviorStartEvent(m_Entity.GetEntityID(), m_nBehaviorMasterID, m_Entity.GetPosition(), m_Entity.GetRotation());
            //RoomNetwork.Instance.SendToNear(entityBehaviorStartEvent, m_Entity.GetPosition(), GameRoom.BROADCAST_SCOPE_RADIUS);

            startTick = Game.Current.CurrentTick;
            lastTick = -1;

            OnStateStart();

            OnTick(Game.Current.CurrentTick);
        }

        public void OnTick(int tick)
        {
            if (lastTick == tick)
            {
                //Debug.LogWarning("Tick() is ignored! lastTick == tick");
                return;
            }

            if (!OnStateUpdate())
            {
                EndState();
            }

            lastTick = tick;
        }

        private void EndState()
        {
            isPlaying = false;

            if (!LOP.Application.IsApplicationQuitting)
            {
                //SC_EntityBehaviorEndEvent entityBehaviorEndEvent = new SC_EntityBehaviorEndEvent(m_Entity.GetEntityID(), m_nBehaviorMasterID);
                //RoomNetwork.Instance.SendToNear(entityBehaviorEndEvent, m_Entity.GetPosition(), GameRoom.BROADCAST_SCOPE_RADIUS);
            }

            OnStateEnd();

            onStateEnd?.Invoke(this);

            onStateEnd = null;
        }

        public void StopState()
        {
            if (isPlaying == false)
            {
                Debug.LogWarning("State is not playing, StopState() is ignored!");
                return;
            }

            EndState();
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public int GetStateMasterID()
        {
            return m_nStateMasterID;
        }

        private void OnDisable()
        {
            if (IsPlaying())
            {
                StopState();
            }
        }
    }
}
