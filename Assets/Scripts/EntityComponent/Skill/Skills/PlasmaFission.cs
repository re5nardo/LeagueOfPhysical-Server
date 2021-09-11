using UnityEngine;
using Entity;
using System;
using System.Collections.Generic;
using GameFramework;
using NetworkModel.Mirror;

namespace Skill
{
    public class PlasmaFission : SkillBase  //  다시 구현해야 함!!
    {
        public enum State
        {
            None,
            CoolTime,
            Ready,
            WaitReuse,
        }

        private float m_fReusableTime = 3f;
        private float m_FireSkillElapsedTime = 0f;

        private float m_fTargetProjectileHeight = 0.5f;
        private float m_fTargetProjectileLifespan = 3f;

        private SkillInputData m_SkillInputData = null;

        private State m_State = State.None;

        private int m_nFirstProjectileEntityID = -1;

		private void Awake()
        {
            skillMasterID = Define.MasterData.SkillID.PLASMA_FISSION;
            coolTime = 0f;
            m_State = State.Ready;

            GamePubSubService.AddSubscriber(GameMessageKey.EntityDestroy, OnEntityDestroy);
		}

        private void OnDestroy()
        {
            GamePubSubService.RemoveSubscriber(GameMessageKey.EntityDestroy, OnEntityDestroy);
        }

        protected override void OnSkillUpdate()
        {
            if (IsCoolTime())
            {
                return;
            }

            if (m_State == State.WaitReuse)
            {
                m_FireSkillElapsedTime += DeltaTime;
            }

            UpdateBody();
        }

        private void UpdateBody()
        {
            if (m_State == State.None || m_State == State.CoolTime)
            {
                return;
            }
            else if (m_State == State.Ready)
            {
                if (m_SkillInputData != null)
                {
                    Entity.Projectile projectile = CreateProjectile();

					m_nFirstProjectileEntityID = projectile.EntityID;

                    Vector3 vec3Destination = projectile.Position + projectile.Forward * projectile.FactoredMovementSpeed * m_fTargetProjectileLifespan;
                    projectile.Move(vec3Destination);

                    m_State = State.WaitReuse;

                    m_SkillInputData = null;
                }
            }
            else if (m_State == State.WaitReuse)
            {
                if (m_SkillInputData != null)
                {
                    LOP.Game.Current.DestroyEntity(m_nFirstProjectileEntityID);

                    m_SkillInputData = null;
                }
                else if (m_FireSkillElapsedTime >= m_fReusableTime)
                {
                    LOP.Game.Current.DestroyEntity(m_nFirstProjectileEntityID);
                }
            }
        }

#region SkillInputData
        public override void OnReceiveSkillInputData(SkillInputData skillInputData)
        {
            base.OnReceiveSkillInputData(skillInputData);

            if (IsCoolTime())
            {
                return;
            }

            if (m_SkillInputData == null)
            {
                m_SkillInputData = skillInputData;
            }
        }
#endregion

        private void Splitting()
        {
            //  Left
            Entity.Projectile projectile_left = CreateProjectile(true);

            Vector3 vec3NewDir = Quaternion.Euler(0, -90f, 0) * projectile_left.Forward;
            Vector3 vec3Destination = projectile_left.Position + vec3NewDir.normalized * projectile_left.FactoredMovementSpeed * m_fTargetProjectileLifespan;
            projectile_left.Move(vec3Destination);

            //  Right
            Entity.Projectile projectile_right = CreateProjectile(true);

            vec3NewDir = Quaternion.Euler(0, 90f, 0) * projectile_right.Forward;
            vec3Destination = projectile_right.Position + vec3NewDir.normalized * projectile_right.FactoredMovementSpeed * m_fTargetProjectileLifespan;
            projectile_right.Move(vec3Destination);
        }

        private Entity.Projectile CreateProjectile(bool split = false)
        {
            MasterData.Projectile masterData = MasterDataManager.Instance.GetMasterData<MasterData.Projectile>(Define.MasterData.ProjectileID.PLASMA_1);

            IEntity firstProjectile = Entities.Get(m_nFirstProjectileEntityID);

            Vector3 vec3StartPosition = split ? firstProjectile.Position : Entity.Position;
            vec3StartPosition.y = m_fTargetProjectileHeight;
            Vector3 vec3StartRotation = split ? firstProjectile.Rotation : Entity.Rotation;
			float fMovementSpeed = (Entity as Character).FirstStatus.DEX;
			Vector3 vec3Velocity = Entity.Forward * fMovementSpeed;

            var projectile = Projectile.Builder()
                .SetEntityId(EntityManager.Instance.GenerateEntityID())
                .SetMasterDataId(Define.MasterData.ProjectileID.PLASMA_1)
                .SetPosition(vec3StartPosition)
                .SetRotation(vec3StartRotation)
                .SetVelocity(vec3Velocity)
				.SetAngularVelocity(Vector3.zero)
				.SetModelId(masterData.ModelResID)
                .SetProjectorId(Entity.EntityID)
                .SetLifespan(m_fTargetProjectileLifespan)
				.SetMovementSpeed(fMovementSpeed)
                .SetEntityType(EntityType.Projectile)
                .SetEntityRole(EntityRole.NPC)
                .SetHasAuthority(true)
                .Build();

            StateController stateController = projectile.GetComponent<StateController>();
            stateController.StartState(Define.MasterData.StateID.EntitySelfDestroy, m_fTargetProjectileLifespan);

            return projectile;
        }

		#region Message Handler
		private void OnEntityDestroy(object[] param)
        {
			int nEntityID = (int)param[0];

			if (m_nFirstProjectileEntityID == nEntityID)
            {
                Splitting();

                m_FireSkillElapsedTime = 0f;

                //  Cooltime
                coolTime = MasterData.CoolTime;

                m_State = State.CoolTime;
            }
        }
		#endregion
	}
}
