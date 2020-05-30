using Entity;
using GameFramework;
using UnityEngine;

namespace Skill
{
    public abstract class SkillBase : MonoComponentBase, ITickable
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

        private MasterData.Skill m_MasterData__ = null;
        public MasterData.Skill m_MasterData
        {
            get
            {
                if (m_MasterData__ == null)
                {
                    m_MasterData__ = MasterDataManager.Instance.GetMasterData<MasterData.Skill>(m_nSkillMasterID);
                }

                return m_MasterData__;
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

            Tick(Game.Current.CurrentTick);
        }

        public int GetSkillMasterID()
        {
            return m_nSkillMasterID;
        }

        public void Tick(int tick)
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