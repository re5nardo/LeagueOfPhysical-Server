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
            MasterDataId = Define.MasterData.SkillID.PLASMA_FISSION;
            CoolTime = 0f;
            m_State = State.Ready;

            SceneMessageBroker.AddSubscriber<GameMessage.EntityDestroy>(OnEntityDestroy);
		}

        private void OnDestroy()
        {
            SceneMessageBroker.RemoveSubscriber<GameMessage.EntityDestroy>(OnEntityDestroy);
        }

        protected override void OnSkillUpdate()
        {
            if (IsCoolTime)
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
                    Projectile projectile = CreateProjectile();

					m_nFirstProjectileEntityID = projectile.EntityID;

                    Vector3 destination = projectile.Position + projectile.Forward * projectile.FactoredMovementSpeed * m_fTargetProjectileLifespan;

                    var behaviorController = projectile.GetEntityComponent<BehaviorController>();
                    behaviorController.Move(destination);

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

            if (IsCoolTime)
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
            Projectile projectile_left = CreateProjectile(true);

            Vector3 newDir = Quaternion.Euler(0, -90f, 0) * projectile_left.Forward;
            Vector3 destination = projectile_left.Position + newDir.normalized * projectile_left.FactoredMovementSpeed * m_fTargetProjectileLifespan;

            var behaviorController = projectile_left.GetEntityComponent<BehaviorController>();
            behaviorController.Move(destination);

            //  Right
            Projectile projectile_right = CreateProjectile(true);

            newDir = Quaternion.Euler(0, 90f, 0) * projectile_right.Forward;
            destination = projectile_right.Position + newDir.normalized * projectile_right.FactoredMovementSpeed * m_fTargetProjectileLifespan;

            behaviorController = projectile_right.GetEntityComponent<BehaviorController>();
            behaviorController.Move(destination);
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
                .SetOwnerId("server")
                .Build();

            StateController stateController = projectile.GetComponent<StateController>();
            stateController.StartState(new BasicStateParam(Define.MasterData.StateId.EntitySelfDestroy, m_fTargetProjectileLifespan));

            return projectile;
        }

		#region Message Handler
		private void OnEntityDestroy(GameMessage.EntityDestroy message)
        {
			if (m_nFirstProjectileEntityID == message.entityId)
            {
                Splitting();

                m_FireSkillElapsedTime = 0f;

                //  Cooltime
                CoolTime = MasterData.CoolTime;

                m_State = State.CoolTime;
            }
        }
		#endregion
	}
}
