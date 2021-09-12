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
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Projectile>(projectileBasicData.MasterDataId);
				}

				return masterData;
			}
		}

		#region MonoEntityBase
		protected override void InitEntityComponents()
		{
			base.InitEntityComponents();

            projectileBasicData = AttachEntityComponent(gameObject.AddComponent<ProjectileBasicData>());

            entityBasicView = AttachEntityComponent(gameObject.AddComponent<ProjectileView>());

            behaviorController = AttachEntityComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachEntityComponent(gameObject.AddComponent<StateController>());
            projectilePhysicsController = AttachEntityComponent(gameObject.AddComponent<ProjectilePhysicsController>());
        }

		public override void Initialize(EntityCreationData entityCreationData)
		{
            base.Initialize(entityCreationData);

            projectileBasicData.Initialize(entityCreationData);
		}

        public override EntitySnap GetEntitySnap()
        {
            var entitySnap = new ProjectileSnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.masterDataId = projectileBasicData.MasterDataId;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;
            entitySnap.modelId = projectileBasicData.ModelId;
            entitySnap.movementSpeed = projectileBasicData.MovementSpeed;
            entitySnap.ownerId = OwnerId;

            return entitySnap;
        }
        #endregion

        #region Interface For Convenience
        public override float MovementSpeed => projectileBasicData.MovementSpeed;
        public override float FactoredMovementSpeed => MovementSpeed * SubGameBase.Current.SubGameEnvironment.MoveSpeedFactor;

        public void Move(Vector3 vec3Destination)
		{
            behaviorController.Move(vec3Destination);
		}
		#endregion
	}
}
