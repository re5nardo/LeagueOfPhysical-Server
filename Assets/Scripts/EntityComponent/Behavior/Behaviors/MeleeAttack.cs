using System.Collections.Generic;
using UnityEngine;
using Entity;
using EntityMessage;
using GameFramework;
using GameEvent;
using System.Linq;

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
        protected override void OnInitialize(BehaviorParam behaviorParam)
        {
            var attackBehaviorParam = behaviorParam as AttackBehaviorParam;
            m_fLifespan = MasterData.lifespan;
            if (attackBehaviorParam.skillInputData == null)
            {
                return;
            }
            if (attackBehaviorParam.skillInputData.inputData == Vector3.zero)
            {
                List<IEntity> targets = Entities.Get(Entity.Position, 20, EntityRole.All, new HashSet<int> { Entity.EntityId });
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
                if (candidates.Count > 0)
                {
                    candidates.Sort((x, y) => (x.Position - Entity.Position).sqrMagnitude.CompareTo((y.Position - Entity.Position).sqrMagnitude));
                    Entity.Rotation = Quaternion.LookRotation(candidates[0].Position - Entity.Position).eulerAngles;
                }
            }
            else
            {
                Entity.Rotation = Quaternion.LookRotation(attackBehaviorParam.skillInputData.inputData).eulerAngles;
            }
        }

        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

			Entity.MessageBroker.Publish(new AnimatorSetTrigger("Attack"));

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityId, MasterData.id), Entity.Position);
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
						if (target.EntityId == Entity.EntityId || !(target as Character).IsAlive)
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

                    LOP.Game.Current.AttackEntity(Entity.EntityId, candidates[0].EntityId);
                }
            }

            return CurrentUpdateTime < m_fLifespan;
        }
        #endregion
    }
}
