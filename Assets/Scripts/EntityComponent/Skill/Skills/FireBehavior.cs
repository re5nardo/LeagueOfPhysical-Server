using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

namespace Skill
{
    public class FireBehavior : SkillBase
    {
        #region MasterData
        private int targetBehaviorID = -1;
        private float fireTime;
        #endregion

        private SkillInputData skillInputData;
        private int fireStartTick = -1;

        public float FireTime => fireStartTick * Game.Current.TickInterval + fireTime;

        #region SkillBase
        public override void SetData(int nSkillMasterID, params object[] param)
        {
            base.SetData(nSkillMasterID, param);

            string[] splitted = MasterData.ClassParams[0].Split(':');
            targetBehaviorID = int.Parse(splitted[0]);
            fireTime = float.Parse(splitted[1]);
        }
        #endregion

        #region SkillInputData
        public override void OnReceiveSkillInputData(SkillInputData skillInputData)
        {
            base.OnReceiveSkillInputData(skillInputData);

            if (IsCoolTime)
            {
                return;
            }

            this.skillInputData = skillInputData;
            fireStartTick = Game.Current.CurrentTick;
        }
        #endregion

        protected override void OnSkillUpdate()
        {
            if (IsCoolTime)
            {
                return;
            }

            if (LastUpdateTime < FireTime && FireTime <= CurrentUpdateTime)
            {
                //BehaviorController behaviorController = Entity.GetComponent<BehaviorController>();
                //behaviorController?.StartBehavior(new BehaviorParam(targetBehaviorID));

                CoolTime = MasterData.CoolTime;
            }
        }
    }
}
