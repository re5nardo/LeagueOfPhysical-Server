using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace Skill
{
    public class FireBehavior : SkillBase
    {
        private int m_nTargetBehaviorID = -1;
        private float m_fFireTime;
        private SkillInputData m_SkillInputData;

        #region SkillBase
        public override void SetData(int nSkillMasterID, params object[] param)
        {
            base.SetData(nSkillMasterID, param);

            MasterData.Skill masterData = MasterDataManager.Instance.GetMasterData<MasterData.Skill>(nSkillMasterID);

            string[] splitted = masterData.ClassParams[0].Split(':');
            m_nTargetBehaviorID = int.Parse(splitted[0]);
            m_fFireTime = float.Parse(splitted[1]);
        }
        #endregion

        #region SkillInputData
        public override void OnReceiveSkillInputData(SkillInputData skillInputData)
        {
            base.OnReceiveSkillInputData(skillInputData);

            m_SkillInputData = skillInputData;

            if (m_fCoolTime > 0)
            {
                return;
            }

            m_fElapsedTime = DeltaTime;
            m_fLastUpdateTime = -1f;
        }
        #endregion

        protected override void OnSkillUpdate()
        {
            if (m_fLastUpdateTime < m_fFireTime && m_fFireTime <= m_fElapsedTime)
            {
                MasterData.Skill masterData = MasterDataManager.Instance.GetMasterData<MasterData.Skill>(m_nSkillMasterID);

                BasicController controller = Entity.GetComponent<BasicController>();
                controller?.StartBehavior(m_nTargetBehaviorID, m_SkillInputData);

                m_fCoolTime = masterData.CoolTime;
            }

            m_fLastUpdateTime = m_fElapsedTime;

            m_fElapsedTime += DeltaTime;
            m_fCoolTime = Mathf.Max(0, m_fCoolTime - DeltaTime);
        }
    }
}