using System;
using GameFramework;
using UnityEngine;

namespace State
{
	public abstract class StateBase : LOPMonoEntityComponentBase
    {
        public event Action<StateBase> onStateEnd = null;

        protected virtual void OnStateStart() { }
        protected abstract bool OnStateUpdate();         //  False : Finish
        protected virtual void OnStateEnd() { }

        public int MasterDataId { get; protected set; } = -1;
        public bool IsPlaying { get; private set; } = false;

        protected int startTick = -1;
        protected int lastTick = -1;

        protected float DeltaTime => lastTick == -1 ? CurrentUpdateTime : CurrentUpdateTime - LastUpdateTime;
        protected float CurrentUpdateTime => Game.Current.CurrentTick == 0 ? 0 : (Game.Current.CurrentTick - startTick + 1) * Game.Current.TickInterval;
        protected float LastUpdateTime => lastTick == -1 ? -1 : (lastTick - startTick + 1) * Game.Current.TickInterval;

        private MasterData.State masterData = null;
        public MasterData.State MasterData => masterData ?? (masterData = MasterDataManager.Instance.GetMasterData<MasterData.State>(MasterDataId));

        public virtual void Initialize(StateParam stateParam)
        {
            this.MasterDataId = stateParam.masterDataId;
        }

        public void StartState()
        {
            if (IsPlaying == true)
            {
                Debug.LogWarning("State is playing, StartState() is ignored!");
                return;
            }

            IsPlaying = true;

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
            IsPlaying = false;

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
            if (IsPlaying == false)
            {
                Debug.LogWarning("State is not playing, StopState() is ignored!");
                return;
            }

            EndState();
        }

        private void OnDisable()
        {
            if (IsPlaying)
            {
                StopState();
            }
        }
    }
}
