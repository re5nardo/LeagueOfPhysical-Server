using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

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

            characterBasicData = AttachEntityComponent(gameObject.AddComponent<CharacterBasicData>());
            characterStatusData = AttachEntityComponent(gameObject.AddComponent<CharacterStatusData>());
            characterAbilityData = AttachEntityComponent(gameObject.AddComponent<CharacterAbilityData>());

            entityBasicView = AttachEntityComponent(gameObject.AddComponent<CharacterView>());

            behaviorController = AttachEntityComponent(gameObject.AddComponent<BehaviorController>());
            stateController = AttachEntityComponent(gameObject.AddComponent<StateController>());
            skillController = AttachEntityComponent(gameObject.AddComponent<SkillController>());
            characterStatusController = AttachEntityComponent(gameObject.AddComponent<CharacterStatusController>());
            characterAbilityController = AttachEntityComponent(gameObject.AddComponent<CharacterAbilityController>());
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
			var entitySnapInfo = new CharacterSnapInfo();

			entitySnapInfo.entityId = EntityID;
			entitySnapInfo.entityType = EntityType;
            entitySnapInfo.entityRole = EntityRole;
            entitySnapInfo.masterDataId = characterBasicData.MasterDataID;
			entitySnapInfo.position = Position;
			entitySnapInfo.rotation = Rotation;
			entitySnapInfo.velocity = Velocity;
			entitySnapInfo.angularVelocity = AngularVelocity;
			entitySnapInfo.model = characterBasicData.ModelId;
			entitySnapInfo.firstStatus = characterStatusData.FirstStatus;
			entitySnapInfo.secondStatus = characterStatusData.SecondStatus;

			return entitySnapInfo;
		}
		#endregion
	}
}
