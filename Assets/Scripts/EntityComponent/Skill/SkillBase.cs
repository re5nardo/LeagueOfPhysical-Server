using GameFramework;
using UnityEngine;
using NetworkModel.Mirror;

namespace Skill
{
    public abstract class SkillBase : LOPMonoEntityComponentBase
    {
        protected abstract void OnSkillUpdate();

        public int MasterDataId { get; protected set; } = -1;

        public double CoolTime { get; protected set; }
        public bool IsCoolTime => CoolTime > 0;

        protected int startTick = -1;
        protected int lastTick = -1;

        protected double DeltaTime => Game.Current.CurrentTick == 0 ? 0 : Game.Current.TickInterval;
        protected double CurrentUpdateTime => Game.Current.CurrentTick == 0 ? 0 : (Game.Current.CurrentTick - startTick + 1) * Game.Current.TickInterval;
        protected double LastUpdateTime => lastTick == -1 ? -1 : (lastTick - startTick + 1) * Game.Current.TickInterval;

        private MasterData.Skill masterData = null;
        public MasterData.Skill MasterData => masterData ?? (masterData = MasterDataManager.Instance.GetMasterData<MasterData.Skill>(MasterDataId));

        #region SkillInputData
        public virtual void OnReceiveSkillInputData(SkillInputData skillInputData)
        {
        }
        #endregion

        public virtual void Initialize(SkillParam skillParam)
        {
            this.MasterDataId = skillParam.masterDataId;
        }

        public void StartSkill()
        {
            startTick = Game.Current.CurrentTick;
            lastTick = -1;

            OnTick(Game.Current.CurrentTick);
        }

        public void OnTick(int tick)
        {
            if (lastTick == tick)
            {
                return;
            }

            CoolTime -= DeltaTime;
            if (CoolTime < 0)
            {
                CoolTime = 0;
            }

            OnSkillUpdate();

            lastTick = tick;
        }
    }
}
