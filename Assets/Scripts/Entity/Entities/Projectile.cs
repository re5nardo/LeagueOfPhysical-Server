using UnityEngine;

namespace Entity
{
	public class Projectile : MonoEntityBase
    {
		#region Builder
		private static ProjectileBuilder projectileBuilder = new ProjectileBuilder();
		public static ProjectileBuilder Builder()
		{
			projectileBuilder.Clear();
			return projectileBuilder;
		}
		#endregion

		private ProjectileBasicData projectileBasicData = null;

		private BehaviorController behaviorController = null;
        private StateController stateController = null;
        private ProjectilePhysicsController projectilePhysicsController = null;

        private MasterData.Projectile masterData = null;
		public MasterData.Projectile MasterData
		{
			get
			{
				if (masterData == null)
				{
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Projectile>(projectileBasicData.MasterDataID);
				}

				return masterData;
			}
		}

		#region MonoEntityBase
		protected override void InitComponents()
		{
			base.InitComponents();

            projectileBasicData = AttachComponent(gameObject.AddComponent<ProjectileBasicData>());

            entityBasicView = AttachComponent(gameObject.AddComponent<ProjectileView>());

            behaviorController = AttachComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachComponent(gameObject.AddComponent<StateController>());
            projectilePhysicsController = AttachComponent(gameObject.AddComponent<ProjectilePhysicsController>());
        }

		public override void Initialize(params object[] param)
		{
			EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.Projectile;
            EntityRole = (EntityRole)param[4];

            projectileBasicData.Initialize(param);
		}
		#endregion

		#region Interface For Convenience
		public override float MovementSpeed { get { return projectileBasicData.MovementSpeed; } }
	
		public override void Move(Vector3 vec3Destination)
		{
            behaviorController.Move(vec3Destination);
		}

		public override EntitySnapInfo GetEntitySnapInfo()
		{
			ProjectileSnapInfo entitySnapInfo = new ProjectileSnapInfo();

			entitySnapInfo.m_nEntityID = EntityID;
			entitySnapInfo.m_EntityType = EntityType;
            entitySnapInfo.m_EntityRole = EntityRole;
            entitySnapInfo.m_nMasterDataID = projectileBasicData.MasterDataID;
			entitySnapInfo.m_Position = Position;
			entitySnapInfo.m_Rotation = Rotation;
			entitySnapInfo.m_Velocity = Velocity;
			entitySnapInfo.m_AngularVelocity = AngularVelocity;
			entitySnapInfo.m_strModel = projectileBasicData.ModelName;
			entitySnapInfo.m_fMovementSpeed = projectileBasicData.MovementSpeed;

			return entitySnapInfo;
		}
		#endregion
	}
}
