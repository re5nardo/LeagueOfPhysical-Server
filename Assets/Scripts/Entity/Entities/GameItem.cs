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

		//	Model
		private GameItemModel m_GameItemModel = null;

		//	View
		private GameItemView m_GameItemView = null;

		//	Controller
		private BasicController m_BasicController = null;

		private MasterData.GameItem m_MasterData__ = null;
		public MasterData.GameItem m_MasterData
		{
			get
			{
				if (m_MasterData__ == null)
				{
					m_MasterData__ = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(m_GameItemModel.MasterDataID);
				}

				return m_MasterData__;
			}
		}

		#region MonoEntityBase
		protected override void InitComponents()
		{
			base.InitComponents();

			//	Model
			m_GameItemModel = AttachComponent(gameObject.AddComponent<GameItemModel>());

			//	View
			m_GameItemView = AttachComponent(gameObject.AddComponent<GameItemView>());

			//	Controller
			m_BasicController = AttachComponent(gameObject.AddComponent<BasicController>());
		}

		public override void Initialize(params object[] param)
		{
			EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.GameItem;
            EntityRole = (EntityRole)param[2];

			m_GameItemModel.Initialize(param);
		}
        #endregion

        #region Interface For Convenience
        public bool IsAlive { get { return m_GameItemModel.CurrentHP > 0; } }
	
		public override float MovementSpeed { get { return m_GameItemModel.MovementSpeed; } }
	
		public override void Move(Vector3 vec3Destination)
		{
			m_BasicController.Move(vec3Destination);
		}

		public override EntitySnapInfo GetEntitySnapInfo()
		{
			GameItemSnapInfo entitySnapInfo = new GameItemSnapInfo();

			entitySnapInfo.m_nEntityID = EntityID;
			entitySnapInfo.m_EntityType = EntityType;
			entitySnapInfo.m_nMasterDataID = m_GameItemModel.MasterDataID;
			entitySnapInfo.m_Position = Position;
			entitySnapInfo.m_Rotation = Rotation;
			entitySnapInfo.m_Velocity = Velocity;
			entitySnapInfo.m_AngularVelocity = AngularVelocity;
			entitySnapInfo.m_strModel = m_GameItemModel.ModelName;
			entitySnapInfo.m_nHP = m_GameItemModel.CurrentHP;
			entitySnapInfo.m_nMaximumHP = m_GameItemModel.MaximumHP;

			return entitySnapInfo;
		}

		public int CurrentHP
		{
			get { return m_GameItemModel.CurrentHP; }
			set { m_GameItemModel.CurrentHP = value; }
		}
		#endregion
	}
}
