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

        private EntityTransformSynchronization entityTransformSynchronization = null;

        private CharacterBasicData characterBasicData = null;
		private CharacterStatusData characterStatusData = null;
		private CharacterAbilityData characterAbilityData = null;

        private BehaviorController behaviorController = null;
        private StateController stateController = null;
        private SkillController skillController = null;
		private CharacterStatusController characterStatusController = null;
		private CharacterAbilityController characterAbilityController = null;

		private MasterData.Character masterData = null;
		public MasterData.Character MasterData
		{
			get
			{
				if (masterData == null)
				{
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Character>(characterBasicData.MasterDataID);
				}

				return masterData;
			}
		}

		#region MonoEntityBase
		protected override void InitComponents()
		{
			base.InitComponents();

            entityTransformSynchronization = AttachComponent(gameObject.AddComponent<EntityTransformSynchronization>());

            characterBasicData = AttachComponent(gameObject.AddComponent<CharacterBasicData>());
            characterStatusData = AttachComponent(gameObject.AddComponent<CharacterStatusData>());
            characterAbilityData = AttachComponent(gameObject.AddComponent<CharacterAbilityData>());

            entityBasicView = AttachComponent(gameObject.AddComponent<CharacterView>());

            behaviorController = AttachComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachComponent(gameObject.AddComponent<StateController>());
            skillController = AttachComponent(gameObject.AddComponent<SkillController>());
            characterStatusController = AttachComponent(gameObject.AddComponent<CharacterStatusController>());
            characterAbilityController = AttachComponent(gameObject.AddComponent<CharacterAbilityController>());
		}

		public override void Initialize(params object[] param)
		{
            base.Initialize(param);

            EntityID = EntityManager.Instance.GenerateEntityID();
			EntityType = EntityType.Character;
            EntityRole = (EntityRole)param[5];

            characterBasicData.Initialize(param[0], param[1]);
            characterStatusData.Initialize(param[2], param[3], param[4]);
		}
		#endregion

		#region Interface For Convenience
		public int CurrentHP
		{
			get { return characterStatusData.CurrentHP; }
			set { characterStatusData.CurrentHP = value; }
		}

		public bool IsAlive { get { return characterStatusData.CurrentHP > 0; } }

		public bool IsSelectableFirstStatus { get { return characterStatusData.SelectableFirstStatusCount > 0; } }

        public override float MovementSpeed => characterStatusData.MovementSpeed;
        public override float FactoredMovementSpeed => characterStatusData.MovementSpeed * SubGameBase.Current.SubGameEnvironment.MoveSpeedFactor;

		public FirstStatus FirstStatus { get { return characterStatusData.FirstStatus; } }

		public SecondStatus SecondStatus { get { return characterStatusData.SecondStatus; } }

		public override EntitySnapInfo GetEntitySnapInfo()
		{
			CharacterSnapInfo entitySnapInfo = new CharacterSnapInfo();

			entitySnapInfo.m_nEntityID = EntityID;
			entitySnapInfo.m_EntityType = EntityType;
            entitySnapInfo.m_EntityRole = EntityRole;
            entitySnapInfo.m_nMasterDataID = characterBasicData.MasterDataID;
			entitySnapInfo.m_Position = Position;
			entitySnapInfo.m_Rotation = Rotation;
			entitySnapInfo.m_Velocity = Velocity;
			entitySnapInfo.m_AngularVelocity = AngularVelocity;
			entitySnapInfo.m_strModel = characterBasicData.ModelId;
			entitySnapInfo.m_FirstStatus = characterStatusData.FirstStatus;
			entitySnapInfo.m_SecondStatus = characterStatusData.SecondStatus;
			entitySnapInfo.m_nSelectableFirstStatusCount = characterStatusData.SelectableFirstStatusCount;

			return entitySnapInfo;
		}
		#endregion
	}
}
