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

        private EntityTransformSynchronization entityTransformSynchronization = null;

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

            entityTransformSynchronization = AttachComponent(gameObject.AddComponent<EntityTransformSynchronization>());

            projectileBasicData = AttachComponent(gameObject.AddComponent<ProjectileBasicData>());

            entityBasicView = AttachComponent(gameObject.AddComponent<ProjectileView>());

            behaviorController = AttachComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachComponent(gameObject.AddComponent<StateController>());
            projectilePhysicsController = AttachComponent(gameObject.AddComponent<ProjectilePhysicsController>());
        }

		public override void Initialize(params object[] param)
		{
            base.Initialize(param);

			EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.Projectile;
            EntityRole = (EntityRole)param[4];

            projectileBasicData.Initialize(param);
		}
		#endregion

		#region Interface For Convenience
		public override float MovementSpeed => projectileBasicData.MovementSpeed;
        public override float FactoredMovementSpeed => projectileBasicData.MovementSpeed * SubGameBase.Current.SubGameEnvironment.MoveSpeedFactor;

        public void Move(Vector3 vec3Destination)
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
			entitySnapInfo.m_strModel = projectileBasicData.ModelId;
			entitySnapInfo.m_fMovementSpeed = projectileBasicData.MovementSpeed;

			return entitySnapInfo;
		}
		#endregion
	}
}
