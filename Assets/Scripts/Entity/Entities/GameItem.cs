using UnityEngine;
using NetworkModel.Mirror;

namespace Entity
{
	public class GameItem : MonoEntityBase
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
        private GameItemPhysicsController gameItemPhysicsController = null;

        private MasterData.GameItem masterData = null;
		public MasterData.GameItem MasterData
		{
			get
			{
				if (masterData == null)
				{
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(gameItemBasicData.MasterDataID);
				}

				return masterData;
			}
		}

		#region MonoEntityBase
		protected override void InitEntityComponents()
		{
			base.InitEntityComponents();

            gameItemBasicData = AttachEntityComponent(gameObject.AddComponent<GameItemBasicData>());

            entityBasicView = AttachEntityComponent(gameObject.AddComponent<GameItemView>());

            behaviorController = AttachEntityComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachEntityComponent(gameObject.AddComponent<StateController>());
            gameItemPhysicsController = AttachEntityComponent(gameObject.AddComponent<GameItemPhysicsController>());
        }

		public override void Initialize(EntityCreationData entityCreationData)
		{
            base.Initialize(entityCreationData);

            gameItemBasicData.Initialize(entityCreationData);
		}
        #endregion

        #region Interface For Convenience
        public bool IsAlive { get { return gameItemBasicData.CurrentHP > 0; } }

        public override float MovementSpeed => gameItemBasicData.MovementSpeed;
        public override float FactoredMovementSpeed => gameItemBasicData.MovementSpeed * SubGameBase.Current.SubGameEnvironment.MoveSpeedFactor;

        public override EntitySnap GetEntitySnap()
		{
			var entitySnap = new GameItemSnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.masterDataId = gameItemBasicData.MasterDataID;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;
            entitySnap.modelId = gameItemBasicData.ModelId;
            entitySnap.HP = gameItemBasicData.CurrentHP;
            entitySnap.maximumHP = gameItemBasicData.MaximumHP;
            entitySnap.hasAuthority = HasAuthority;


            return entitySnap;
		}

		public int CurrentHP
		{
			get { return gameItemBasicData.CurrentHP; }
			set { gameItemBasicData.CurrentHP = value; }
		}
		#endregion
	}
}
