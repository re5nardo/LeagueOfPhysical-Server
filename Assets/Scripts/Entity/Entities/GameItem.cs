using UnityEngine;
using NetworkModel.Mirror;

namespace Entity
{
	public class GameItem : LOPMonoEntityBase
    {
		#region Builder
		private static GameItemBuilder gameItemBuilder = new GameItemBuilder();
		public static GameItemBuilder Builder()
		{
			gameItemBuilder.Clear();
			return gameItemBuilder;
		}
        #endregion

        public GameItemBasicData GameItemBasicData { get; private set; }

        private MasterData.GameItem masterData = null;
        public MasterData.GameItem MasterData => masterData ?? (masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(GameItemBasicData.MasterDataId));

        #region LOPEntityBase
        protected override void InitEntityComponents()
		{
			base.InitEntityComponents();

            GameItemBasicData = AttachEntityComponent<GameItemBasicData>();

            EntityBasicView = AttachEntityComponent<GameItemView>();
        }

		protected override void OnInitialize(EntityCreationData entityCreationData)
		{
            base.OnInitialize(entityCreationData);

            GameItemBasicData.Initialize(entityCreationData);
		}

        public override EntitySnap GetEntitySnap()
        {
            var entitySnap = new GameItemSnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.masterDataId = GameItemBasicData.MasterDataId;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;
            entitySnap.modelId = GameItemBasicData.ModelId;
            entitySnap.HP = GameItemBasicData.HP;
            entitySnap.maximumHP = GameItemBasicData.MaximumHP;
            entitySnap.ownerId = OwnerId;

            return entitySnap;
        }
        #endregion

        #region Interface For Convenience
        public bool IsAlive => GameItemBasicData.HP > 0;

        public override float MovementSpeed => GameItemBasicData.MovementSpeed;
        public override float FactoredMovementSpeed => MovementSpeed * LOP.Game.Current.GameManager.MapData.mapEnvironment.MoveSpeedFactor;

		public int HP
		{
            get => GameItemBasicData.HP;
            set => GameItemBasicData.HP = value;
		}
		#endregion
	}
}
