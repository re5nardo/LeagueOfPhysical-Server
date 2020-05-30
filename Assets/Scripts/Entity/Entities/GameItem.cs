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

		private GameItemBasicData gameItemBasicData = null;

		private GameItemView gameItemView = null;

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

            gameItemBasicData = AttachComponent(gameObject.AddComponent<GameItemBasicData>());

            gameItemView = AttachComponent(gameObject.AddComponent<GameItemView>());

            behaviorController = AttachComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachComponent(gameObject.AddComponent<StateController>());
            gameItemPhysicsController = AttachComponent(gameObject.AddComponent<GameItemPhysicsController>());
        }

		public override void Initialize(params object[] param)
		{
			EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.GameItem;
            EntityRole = (EntityRole)param[2];

            gameItemBasicData.Initialize(param);
		}
        #endregion

        #region Interface For Convenience
        public bool IsAlive { get { return gameItemBasicData.CurrentHP > 0; } }
	
		public override float MovementSpeed { get { return gameItemBasicData.MovementSpeed; } }
	
		public override void Move(Vector3 vec3Destination)
		{
            behaviorController.Move(vec3Destination);
		}

		public override EntitySnapInfo GetEntitySnapInfo()
		{
			GameItemSnapInfo entitySnapInfo = new GameItemSnapInfo();

			entitySnapInfo.m_nEntityID = EntityID;
			entitySnapInfo.m_EntityType = EntityType;
			entitySnapInfo.m_nMasterDataID = gameItemBasicData.MasterDataID;
			entitySnapInfo.m_Position = Position;
			entitySnapInfo.m_Rotation = Rotation;
			entitySnapInfo.m_Velocity = Velocity;
			entitySnapInfo.m_AngularVelocity = AngularVelocity;
			entitySnapInfo.m_strModel = gameItemBasicData.ModelName;
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
