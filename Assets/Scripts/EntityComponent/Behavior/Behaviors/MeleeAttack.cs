using System.Collections.Generic;
using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;
using GameEvent;

namespace Behavior
{
    public class MeleeAttack : BehaviorBase
    {
        #region ClassParams
        private float m_fLifespan;
        private float m_fAttackTime;
        private float m_fMaxHalfAngle = 45f;
        private float m_fRadius = 2f;
        #endregion

        #region BehaviorBase
        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

			Entity.SendCommandToViews(new AnimatorSetTrigger("Attack"));

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.ID), Entity.Position);
        }

        protected override bool OnBehaviorUpdate()
        {
            if (LastUpdateTime < m_fAttackTime && m_fAttackTime <= CurrentUpdateTime)
            {
				Collider[] targets = Physics.OverlapSphere(Entity.Position, m_fRadius);
                List<IEntity> candidates = new List<IEntity>();
                foreach (Collider collider in targets)
                {
                    EntityIDTag entityIDTag = collider.GetComponent<EntityIDTag>();
                    if(entityIDTag == null || Entities.Get(entityIDTag.GetEntityID()) == null)
                    {
                        continue;
                    }

					IEntity target = Entities.Get(entityIDTag.GetEntityID());
					if (target is Character)
					{
						if (target.EntityID == Entity.EntityID || !(target as Character).IsAlive)
						{
							continue;
						}
					}
					else if (target is GameItem)
					{
						if (!(target as GameItem).IsAlive)
						{
							continue;
						}
					}

					float theta = Vector3.Dot(Entity.Forward, (collider.transform.position - Entity.Position).normalized);
                    float angle = Mathf.Acos(theta) * Mathf.Rad2Deg;

                    if (angle <= m_fMaxHalfAngle)
                    {
                        candidates.Add(target);
                    }
                }

                //  Find best suitable one
                if (candidates.Count > 0)
                {
                    candidates.Sort((x, y) => (x.Position - Entity.Position).sqrMagnitude.CompareTo((y.Position - Entity.Position).sqrMagnitude));

                    LOP.Game.Current.AttackEntity(Entity.EntityID, candidates[0].EntityID);
                }
            }

            return CurrentUpdateTime < m_fLifespan;
        }

        public override void SetData(int nBehaviorMasterID, params object[] param)
        {
            base.SetData(nBehaviorMasterID);

            m_fLifespan = MasterData.Lifespan;
            m_fAttackTime = float.Parse(MasterData.ClassParams.Find(x => x.Contains("AttackTime")).Split(':')[1]);

            if (param.Length > 0 && param[0] is SkillInputData)
            {
                SkillInputData data = param[0] as SkillInputData;

                if (data.m_InputData.ToVector3() == Vector3.zero)
                {
                    //  Auto aiming
                    List<IEntity> targets = Entities.Get(Entity.Position, 20, EntityRole.All, new HashSet<int> { Entity.EntityID });
                    List<IEntity> candidates = new List<IEntity>();
                    foreach (IEntity target in targets)
                    {
                        if (target is Character)
                        {
                            if (!(target as Character).IsAlive)
                            {
                                continue;
                            }
                        }
                        else if (target is GameItem)
                        {
                            if (!(target as GameItem).IsAlive)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        candidates.Add(target);
                    }

                    //  Find best suitable one
                    if (candidates.Count > 0)
                    {
                        candidates.Sort((x, y) => (x.Position - Entity.Position).sqrMagnitude.CompareTo((y.Position - Entity.Position).sqrMagnitude));

                        Entity.Rotation = Quaternion.LookRotation(candidates[0].Position - Entity.Position).eulerAngles;
                    }
                }
                else
                {
                    Entity.Rotation = Quaternion.LookRotation(data.m_InputData.ToVector3()).eulerAngles;
                }
            }
        }
        #endregion
    }
}
