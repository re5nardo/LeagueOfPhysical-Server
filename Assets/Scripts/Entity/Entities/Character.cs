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
                    masterData = MasterDataManager.Instance.GetMasterData<MasterData.Character>(characterBasicData.MasterDataId);
				}

				return masterData;
			}
		}

		#region MonoEntityBase
		protected override void InitEntityComponents()
		{
			base.InitEntityComponents();

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

		public override void Initialize(EntityCreationData entityCreationData)
		{
            base.Initialize(entityCreationData);

            characterBasicData.Initialize(entityCreationData);
            characterStatusData.Initialize(entityCreationData);
		}

        public override EntitySnap GetEntitySnap()
        {
            var entitySnap = new CharacterSnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.masterDataId = characterBasicData.MasterDataId;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;
            entitySnap.modelId = characterBasicData.ModelId;
            entitySnap.firstStatus = characterStatusData.firstStatus;
            entitySnap.secondStatus = characterStatusData.secondStatus;
            entitySnap.hasAuthority = HasAuthority;

            return entitySnap;
        }
        #endregion

        #region Interface For Convenience
        public int HP
        {
			get => characterStatusData.HP;
            set => characterStatusData.HP = value;
        }

		public bool IsAlive => characterStatusData.HP > 0;

        public override float MovementSpeed => characterStatusData.MovementSpeed;
        public override float FactoredMovementSpeed => MovementSpeed * SubGameBase.Current.SubGameEnvironment.MoveSpeedFactor;

		public FirstStatus FirstStatus => characterStatusData.firstStatus;
        public SecondStatus SecondStatus => characterStatusData.secondStatus;
		#endregion
	}
}
