using UnityEngine;
using Entity;
using EntityCommand;
using System.Collections.Generic;
using GameFramework;
using GameEvent;
using NetworkModel.Mirror;

namespace Behavior
{
    public class RangeAttack : BehaviorBase
    {
        #region ClassParams
        private float m_fLifespan;
        private float m_fAttackTime;
        private int m_nProjectileID;
        private float m_fProjectileHeight;
        private float m_fProjectileLifespan;
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
                Projectile projectile = CreateProjectile();

				Vector3 vec3Destination = projectile.Position + projectile.Forward * projectile.FactoredMovementSpeed * m_fProjectileLifespan;
				projectile.Move(vec3Destination);
			}

            return CurrentUpdateTime < m_fLifespan;
        }

        public override void SetData(int nBehaviorMasterID, params object[] param)
        {
            base.SetData(nBehaviorMasterID);

            m_fLifespan = MasterData.Lifespan;
            m_fAttackTime = float.Parse(MasterData.ClassParams.Find(x => x.Contains("AttackTime")).Split(':')[1]);
            m_nProjectileID = int.Parse(MasterData.ClassParams.Find(x => x.Contains("ProjectileID")).Split(':')[1]);
            m_fProjectileHeight = float.Parse(MasterData.ClassParams.Find(x => x.Contains("ProjectileHeight")).Split(':')[1]);
            m_fProjectileLifespan = float.Parse(MasterData.ClassParams.Find(x => x.Contains("ProjectileLifespan")).Split(':')[1]);

            if (param.Length > 0 && param[0] is SkillInputData)
            {
                SkillInputData data = param[0] as SkillInputData;

                if (data.inputData == Vector3.zero)
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
                    Entity.Rotation = Quaternion.LookRotation(data.inputData).eulerAngles;
                }
            }
        }
        #endregion

        private Projectile CreateProjectile()
        {
            MasterData.Projectile masterData = MasterDataManager.Instance.GetMasterData<MasterData.Projectile>(m_nProjectileID);

            Vector3 vec3StartPosition = Entity.Position;
            vec3StartPosition.y = m_fProjectileHeight;
            Vector3 vec3StartRotation = Entity.Rotation;
			float fMovementSpeed = (Entity as Character).FirstStatus.DEX;
			Vector3 vec3Velocity = Entity.Forward * fMovementSpeed;

			return Projectile.Builder()
                .SetMasterDataID(m_nProjectileID)
                .SetPosition(vec3StartPosition)
                .SetRotation(vec3StartRotation)
                .SetVelocity(vec3Velocity)
				.SetAngularVelocity(Vector3.zero)
				.SetModelPath(masterData.ModelResID)
                .SetProjectorID(Entity.EntityID)
                .SetLifespan(m_fProjectileLifespan)
				.SetMovementSpeed(fMovementSpeed)
                .SetEntityRole(EntityRole.NPC)
				.Build();
        }
    }
}
