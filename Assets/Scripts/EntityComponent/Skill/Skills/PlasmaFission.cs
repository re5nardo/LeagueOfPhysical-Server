using UnityEngine;
using Entity;
using System;
using System.Collections.Generic;
using GameFramework;

namespace Skill
{
    public class PlasmaFission : SkillBase, ISubscriber
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

		private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

		private void Awake()
        {
            m_nSkillMasterID = Define.MasterData.SkillID.PLASMA_FISSION;
            m_fCoolTime = 0f;
            m_State = State.Ready;

            GamePubSubService.Instance.AddSubscriber(GameMessageKey.EntityDestroy, this);

			m_dicMessageHandler.Add(GameMessageKey.EntityDestroy, OnEntityDestroy);
		}

        private void OnDestroy()
        {
            if (GamePubSubService.IsInstantiated())
            {
                GamePubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityDestroy, this);
            }

			m_dicMessageHandler.Clear();
		}

        private bool IsCoolTime()
        {
            return m_fCoolTime > 0;
        }

        protected override void OnSkillUpdate()
        {
            if (IsCoolTime())
            {
                m_fCoolTime = Mathf.Max(0, m_fCoolTime - DeltaTime);

                if (m_fCoolTime == 0)
                {
                    m_State = State.Ready;
                }
            }
            else
            {
                m_fElapsedTime += DeltaTime;

                if (m_State == State.WaitReuse)
                {
                    m_FireSkillElapsedTime += DeltaTime;
                }

                UpdateBody();

                m_fLastUpdateTime = m_fElapsedTime;
            }
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

                    Vector3 vec3Destination = projectile.Position + projectile.Forward * projectile.MovementSpeed * m_fTargetProjectileLifespan;
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
            Vector3 vec3Destination = projectile_left.Position + vec3NewDir.normalized * projectile_left.MovementSpeed * m_fTargetProjectileLifespan;
            projectile_left.Move(vec3Destination);

            //  Right
            Entity.Projectile projectile_right = CreateProjectile(true);

            vec3NewDir = Quaternion.Euler(0, 90f, 0) * projectile_right.Forward;
            vec3Destination = projectile_right.Position + vec3NewDir.normalized * projectile_right.MovementSpeed * m_fTargetProjectileLifespan;
            projectile_right.Move(vec3Destination);
        }

        private Entity.Projectile CreateProjectile(bool split = false)
        {
            MasterData.Projectile masterData = MasterDataManager.Instance.GetMasterData<MasterData.Projectile>(Define.MasterData.ProjectileID.PLASMA_1);

            IEntity firstProjectile = EntityManager.Instance.GetEntity(m_nFirstProjectileEntityID);

            Vector3 vec3StartPosition = split ? firstProjectile.Position : Entity.Position;
            vec3StartPosition.y = m_fTargetProjectileHeight;
            Vector3 vec3StartRotation = split ? firstProjectile.Rotation : Entity.Rotation;
			float fMovementSpeed = (Entity as Character).FirstStatus.DEX;
			Vector3 vec3Velocity = Entity.Forward * fMovementSpeed;

            return Projectile.Builder()
                .SetMasterDataID(Define.MasterData.ProjectileID.PLASMA_1)
                .SetPosition(vec3StartPosition)
                .SetRotation(vec3StartRotation)
                .SetVelocity(vec3Velocity)
				.SetAngularVelocity(Vector3.zero)
				.SetModelPath(masterData.ModelResID)
                .SetProjectorID(Entity.EntityID)
                .SetLifespan(m_fTargetProjectileLifespan)
				.SetMovementSpeed(fMovementSpeed)
                .SetEntityRole(EntityRole.NPC)
                .Build();
        }

		#region ISubscriber
        public void OnMessage(Enum key, params object[] param)
        {
			m_dicMessageHandler[key](param);
		}
		#endregion

		#region Message Handler
		private void OnEntityDestroy(params object[] param)
        {
			int nEntityID = (int)param[0];

			if (m_nFirstProjectileEntityID == nEntityID)
            {
                Splitting();

                m_fElapsedTime = 0;
                m_fLastUpdateTime = -1f;
                m_FireSkillElapsedTime = 0f;

                //  Cooltime
                MasterData.Skill masterData = MasterDataManager.Instance.GetMasterData<MasterData.Skill>(m_nSkillMasterID);
                m_fCoolTime = masterData.CoolTime;

                m_State = State.CoolTime;
            }
        }
		#endregion
	}
}
