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
		protected override void InitComponents()
		{
			base.InitComponents();

            gameItemBasicData = AttachEntityComponent(gameObject.AddComponent<GameItemBasicData>());

            entityBasicView = AttachEntityComponent(gameObject.AddComponent<GameItemView>());

            behaviorController = AttachEntityComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachEntityComponent(gameObject.AddComponent<StateController>());
            gameItemPhysicsController = AttachEntityComponent(gameObject.AddComponent<GameItemPhysicsController>());
        }

		public override void Initialize(params object[] param)
		{
            base.Initialize(param);

            EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.GameItem;
            EntityRole = (EntityRole)param[2];

            gameItemBasicData.Initialize(param);
		}
        #endregion

        #region Interface For Convenience
        public bool IsAlive { get { return gameItemBasicData.CurrentHP > 0; } }

        public override float MovementSpeed => gameItemBasicData.MovementSpeed;
        public override float FactoredMovementSpeed => gameItemBasicData.MovementSpeed * SubGameBase.Current.SubGameEnvironment.MoveSpeedFactor;

        public override EntitySnapInfo GetEntitySnapInfo()
		{
			var entitySnapInfo = new GameItemSnapInfo();

			entitySnapInfo.entityId = EntityID;
			entitySnapInfo.entityType = EntityType;
            entitySnapInfo.entityRole = EntityRole;
            entitySnapInfo.masterDataId = gameItemBasicData.MasterDataID;
			entitySnapInfo.position = Position;
			entitySnapInfo.rotation = Rotation;
			entitySnapInfo.velocity = Velocity;
			entitySnapInfo.angularVelocity = AngularVelocity;
			entitySnapInfo.model = gameItemBasicData.ModelId;
			entitySnapInfo.HP = gameItemBasicData.CurrentHP;
			entitySnapInfo.maximumHP = gameItemBasicData.MaximumHP;

			return entitySnapInfo;
		}

		public int CurrentHP
		{
			get { return gameItemBasicData.CurrentHP; }
			set { gameItemBasicData.CurrentHP = value; }
		}
		#endregion
	}
}
