using UnityEngine;

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

        private EntityTransformSynchronization entityTransformSynchronization = null;

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

            entityTransformSynchronization = AttachComponent(gameObject.AddComponent<EntityTransformSynchronization>());

            gameItemBasicData = AttachComponent(gameObject.AddComponent<GameItemBasicData>());

            entityBasicView = AttachComponent(gameObject.AddComponent<GameItemView>());

            behaviorController = AttachComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachComponent(gameObject.AddComponent<StateController>());
            gameItemPhysicsController = AttachComponent(gameObject.AddComponent<GameItemPhysicsController>());
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
			GameItemSnapInfo entitySnapInfo = new GameItemSnapInfo();

			entitySnapInfo.m_nEntityID = EntityID;
			entitySnapInfo.m_EntityType = EntityType;
            entitySnapInfo.m_EntityRole = EntityRole;
            entitySnapInfo.m_nMasterDataID = gameItemBasicData.MasterDataID;
			entitySnapInfo.m_Position = Position;
			entitySnapInfo.m_Rotation = Rotation;
			entitySnapInfo.m_Velocity = Velocity;
			entitySnapInfo.m_AngularVelocity = AngularVelocity;
			entitySnapInfo.m_strModel = gameItemBasicData.ModelId;
			entitySnapInfo.m_nHP = gameItemBasicData.CurrentHP;
			entitySnapInfo.m_nMaximumHP = gameItemBasicData.MaximumHP;

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
