using UnityEngine;
using NetworkModel.Mirror;

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
			var entitySnapInfo = new ProjectileSnapInfo();

			entitySnapInfo.entityId = EntityID;
			entitySnapInfo.entityType = EntityType;
            entitySnapInfo.entityRole = EntityRole;
            entitySnapInfo.masterDataId = projectileBasicData.MasterDataID;
			entitySnapInfo.position = Position;
			entitySnapInfo.rotation = Rotation;
			entitySnapInfo.velocity = Velocity;
			entitySnapInfo.angularVelocity = AngularVelocity;
			entitySnapInfo.model = projectileBasicData.ModelId;
			entitySnapInfo.movementSpeed = projectileBasicData.MovementSpeed;

			return entitySnapInfo;
		}
		#endregion
	}
}
