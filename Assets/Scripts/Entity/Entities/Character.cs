using UnityEngine;
using GameFramework;

namespace Entity
{
	public class Character : MonoEntityBase
    {
		#region Builder
		private static CharacterBuilder characterBuilder = new CharacterBuilder();
		public static CharacterBuilder Builder()
		{
			characterBuilder.Clear();
			return characterBuilder;
		}
		#endregion

		//	Model
		private CharacterModel m_CharacterModel = null;
		private CharacterStatusModel m_CharacterStatusModel = null;
		private CharacterAbilityModel m_CharacterAbilityModel = null;

		//	View
		private CharacterView m_CharacterView = null;

		//	Controller
		private BasicController m_BasicController = null;
		private SkillController m_SkillController = null;
		private CharacterStatusController m_CharacterStatusController = null;
		private CharacterAbilityController m_CharacterAbilityController = null;

		private MasterData.Character m_MasterData__ = null;
		public MasterData.Character m_MasterData
		{
			get
			{
				if (m_MasterData__ == null)
				{
					m_MasterData__ = MasterDataManager.Instance.GetMasterData<MasterData.Character>(m_CharacterModel.MasterDataID);
				}

				return m_MasterData__;
			}
		}

		#region MonoEntityBase
		protected override void InitComponents()
		{
			base.InitComponents();

			//	Model
			m_CharacterModel = AttachComponent(gameObject.AddComponent<CharacterModel>());
			m_CharacterStatusModel = AttachComponent(gameObject.AddComponent<CharacterStatusModel>());
			m_CharacterAbilityModel = AttachComponent(gameObject.AddComponent<CharacterAbilityModel>());

			//	View
			m_CharacterView = AttachComponent(gameObject.AddComponent<CharacterView>());

			//	Controller
			m_BasicController = AttachComponent(gameObject.AddComponent<BasicController>());
			m_SkillController = AttachComponent(gameObject.AddComponent<SkillController>());
			m_CharacterStatusController = AttachComponent(gameObject.AddComponent<CharacterStatusController>());
			m_CharacterAbilityController = AttachComponent(gameObject.AddComponent<CharacterAbilityController>());
		}

		public override void Initialize(params object[] param)
		{
			EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.Character;
            EntityRole = (EntityRole)param[5];

			m_CharacterModel.Initialize(param[0], param[1]);
			m_CharacterStatusModel.Initialize(param[2], param[3], param[4]);
		}
		#endregion

		#region Interface For Convenience
		public int CurrentHP
		{
			get { return m_CharacterStatusModel.CurrentHP; }
			set { m_CharacterStatusModel.CurrentHP = value; }
		}

		public bool IsAlive { get { return m_CharacterStatusModel.CurrentHP > 0; } }

		public bool IsSelectableFirstStatus { get { return m_CharacterStatusModel.SelectableFirstStatusCount > 0; } }

		public Transform ModelTransform { get { return m_CharacterView.ModelTransform; } }

		public override float MovementSpeed { get { return m_CharacterStatusModel.MovementSpeed; } }

		public FirstStatus FirstStatus { get { return m_CharacterStatusModel.FirstStatus; } }

		public SecondStatus SecondStatus { get { return m_CharacterStatusModel.SecondStatus; } }

		public override void Move(Vector3 vec3Destination)
		{
			m_BasicController.Move(vec3Destination);
		}

		public override EntitySnapInfo GetEntitySnapInfo()
		{
			CharacterSnapInfo entitySnapInfo = new CharacterSnapInfo();

			entitySnapInfo.m_nEntityID = EntityID;
			entitySnapInfo.m_EntityType = EntityType;
			entitySnapInfo.m_nMasterDataID = m_CharacterModel.MasterDataID;
			entitySnapInfo.m_Position = Position;
			entitySnapInfo.m_Rotation = Rotation;
			entitySnapInfo.m_Velocity = Velocity;
			entitySnapInfo.m_AngularVelocity = AngularVelocity;
			entitySnapInfo.m_strModel = m_CharacterModel.ModelName;
			entitySnapInfo.m_FirstStatus = m_CharacterStatusModel.FirstStatus;
			entitySnapInfo.m_SecondStatus = m_CharacterStatusModel.SecondStatus;
			entitySnapInfo.m_nSelectableFirstStatusCount = m_CharacterStatusModel.SelectableFirstStatusCount;

			return entitySnapInfo;
		}
		#endregion
	}
}
