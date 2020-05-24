using UnityEngine;

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

		//	Model
		private ProjectileModel m_ProjectileModel = null;

		//	View
		private ProjectileView m_ProjectileView = null;

		//	Controller
		private BasicController m_BasicController = null;

		private MasterData.Projectile m_MasterData__ = null;
		public MasterData.Projectile m_MasterData
		{
			get
			{
				if (m_MasterData__ == null)
				{
					m_MasterData__ = MasterDataManager.Instance.GetMasterData<MasterData.Projectile>(m_ProjectileModel.MasterDataID);
				}

				return m_MasterData__;
			}
		}

		#region MonoEntityBase
		protected override void InitComponents()
		{
			base.InitComponents();

			//	Model
			m_ProjectileModel = AttachComponent(gameObject.AddComponent<ProjectileModel>());

			//	View
			m_ProjectileView = AttachComponent(gameObject.AddComponent<ProjectileView>());

			//	Controller
			m_BasicController = AttachComponent(gameObject.AddComponent<BasicController>());
		}

		public override void Initialize(params object[] param)
		{
			EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.Projectile;
            EntityRole = (EntityRole)param[4];

			m_ProjectileModel.Initialize(param);
		}
		#endregion

		#region Interface For Convenience
		public override float MovementSpeed { get { return m_ProjectileModel.MovementSpeed; } }
	
		public override void Move(Vector3 vec3Destination)
		{
			m_BasicController.Move(vec3Destination);
		}

		public override EntitySnapInfo GetEntitySnapInfo()
		{
			ProjectileSnapInfo entitySnapInfo = new ProjectileSnapInfo();

			entitySnapInfo.m_nEntityID = EntityID;
			entitySnapInfo.m_EntityType = EntityType;
			entitySnapInfo.m_nMasterDataID = m_ProjectileModel.MasterDataID;
			entitySnapInfo.m_Position = Position;
			entitySnapInfo.m_Rotation = Rotation;
			entitySnapInfo.m_Velocity = Velocity;
			entitySnapInfo.m_AngularVelocity = AngularVelocity;
			entitySnapInfo.m_strModel = m_ProjectileModel.ModelName;
			entitySnapInfo.m_fMovementSpeed = m_ProjectileModel.MovementSpeed;

			return entitySnapInfo;
		}
		#endregion
	}
}
