using UnityEngine;
using NetworkModel.Mirror;

namespace Entity
{
	public class Projectile : LOPMonoEntityBase
    {
		#region Builder
		private static ProjectileBuilder projectileBuilder = new ProjectileBuilder();
		public static ProjectileBuilder Builder()
		{
			projectileBuilder.Clear();
			return projectileBuilder;
		}
        #endregion

        public ProjectileBasicData ProjectileBasicData { get; private set; }

        private MasterData.Projectile masterData = null;
        public MasterData.Projectile MasterData => masterData ?? (masterData = MasterDataManager.Instance.GetMasterData<MasterData.Projectile>(ProjectileBasicData.MasterDataId));

        #region LOPEntityBase
        protected override void InitEntityComponents()
		{
			base.InitEntityComponents();

            ProjectileBasicData = AttachEntityComponent<ProjectileBasicData>();

            EntityBasicView = AttachEntityComponent<ProjectileView>();
        }

		protected override void OnInitialize(EntityCreationData entityCreationData)
		{
            base.OnInitialize(entityCreationData);

            ProjectileBasicData.Initialize(entityCreationData);
		}

        public override EntitySnap GetEntitySnap()
        {
            var entitySnap = new ProjectileSnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.masterDataId = ProjectileBasicData.MasterDataId;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;
            entitySnap.modelId = ProjectileBasicData.ModelId;
            entitySnap.movementSpeed = ProjectileBasicData.MovementSpeed;
            entitySnap.ownerId = OwnerId;

            return entitySnap;
        }
        #endregion

        #region Interface For Convenience
        public override float MovementSpeed => ProjectileBasicData.MovementSpeed;
        public override float FactoredMovementSpeed => MovementSpeed * LOP.Game.Current.MapData.mapEnvironment.MoveSpeedFactor;
		#endregion
	}
}
