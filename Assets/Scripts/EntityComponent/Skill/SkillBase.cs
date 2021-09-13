using Entity;
using GameFramework;
using UnityEngine;
using NetworkModel.Mirror;

namespace Skill
{
    public abstract class SkillBase : MonoEntityComponentBase
    {
        protected abstract void OnSkillUpdate();

        protected int skillMasterID = -1;
        protected float coolTime = 0f;
        protected int startTick = -1;
        protected int lastTick = -1;

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

        public float CoolTime => coolTime;
        public bool IsCoolTime()
        {
            return CoolTime > 0;
        }

        new public LOPEntityBase Entity { get; private set; }

        private MasterData.Skill masterData = null;
        public MasterData.Skill MasterData
        {
            get
            {
                if (masterData == null)
                {
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Skill>(skillMasterID);
                }

                return masterData;
            }
        }

        #region IComponent
        public override void OnAttached(IEntity entity)
		{
			base.OnAttached(entity);

			Entity = entity as LOPEntityBase;
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

        public virtual void SetData(int skillMasterID, params object[] param)
        {
            this.skillMasterID = skillMasterID;
        }

        public void StartSkill()
        {
            startTick = Game.Current.CurrentTick;
            lastTick = -1;

            OnTick(Game.Current.CurrentTick);
        }

        public int GetSkillMasterID()
        {
            return skillMasterID;
        }

        public void OnTick(int tick)
        {
            if (lastTick == tick)
            {
                //Debug.LogWarning("Tick() is ignored! lastTick == tick");
                return;
            }

            coolTime = Mathf.Max(0, CoolTime - DeltaTime);

            OnSkillUpdate();

            lastTick = tick;
        }
    }
}
