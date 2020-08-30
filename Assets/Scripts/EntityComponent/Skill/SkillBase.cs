using Entity;
using GameFramework;
using UnityEngine;

namespace Skill
{
    public abstract class SkillBase : MonoComponentBase
    {
        protected abstract void OnSkillUpdate();

        protected int m_nSkillMasterID = -1;
        protected float m_fLastUpdateTime = -1f;
        protected float m_fElapsedTime = 0f;
        protected float m_fCoolTime = 0f;
        protected int startTick = -1;
        protected int lastTick = -1;

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

        public float CoolTime { get { return m_fCoolTime; } }

        new public MonoEntityBase Entity { get; private set; }

        private MasterData.Skill masterData = null;
        public MasterData.Skill MasterData
        {
            get
            {
                if (masterData == null)
                {
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Skill>(m_nSkillMasterID);
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

		#region SkillInputData
		public virtual void OnReceiveSkillInputData(SkillInputData skillInputData)
        {
        }
        #endregion

        public virtual void SetData(int nSkillMasterID, params object[] param)
        {
            m_nSkillMasterID = nSkillMasterID;
        }

        public void StartSkill()
        {
            startTick = Game.Current.CurrentTick;
            lastTick = -1;

            OnTick(Game.Current.CurrentTick);
        }

        public int GetSkillMasterID()
        {
            return m_nSkillMasterID;
        }

        public void OnTick(int tick)
        {
            if (lastTick == tick)
            {
                //Debug.LogWarning("Tick() is ignored! lastTick == tick");
                return;
            }

            OnSkillUpdate();

            lastTick = tick;
        }
    }
}
