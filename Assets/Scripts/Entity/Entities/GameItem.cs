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

        private GameItemBasicData gameItemBasicData = null;

        private BehaviorController behaviorController = null;
        private StateController stateController = null;

        private MasterData.GameItem masterData = null;
        public MasterData.GameItem MasterData => masterData ?? (masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(gameItemBasicData.MasterDataId));

        #region LOPEntityBase
        protected override void InitEntityComponents()
		{
			base.InitEntityComponents();

            gameItemBasicData = AttachEntityComponent(gameObject.AddComponent<GameItemBasicData>());

            entityBasicView = AttachEntityComponent(gameObject.AddComponent<GameItemView>());

            behaviorController = AttachEntityComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachEntityComponent(gameObject.AddComponent<StateController>());
        }

		protected override void OnInitialize(EntityCreationData entityCreationData)
		{
            base.OnInitialize(entityCreationData);

            gameItemBasicData.Initialize(entityCreationData);
		}

        public override EntitySnap GetEntitySnap()
        {
            var entitySnap = new GameItemSnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.masterDataId = gameItemBasicData.MasterDataId;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;
            entitySnap.modelId = gameItemBasicData.ModelId;
            entitySnap.HP = gameItemBasicData.HP;
            entitySnap.maximumHP = gameItemBasicData.MaximumHP;
            entitySnap.ownerId = OwnerId;

            return entitySnap;
        }
        #endregion

        #region Interface For Convenience
        public bool IsAlive => gameItemBasicData.HP > 0;

        public override float MovementSpeed => gameItemBasicData.MovementSpeed;
        public override float FactoredMovementSpeed => MovementSpeed * LOP.Game.Current.GameManager.MapData.mapEnvironment.MoveSpeedFactor;

		public int HP
		{
            get => gameItemBasicData.HP;
            set => gameItemBasicData.HP = value;
		}
		#endregion
	}
}
