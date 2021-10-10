using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

namespace Entity
{
	public class Character : LOPMonoEntityBase
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
        public MasterData.Character MasterData => masterData ?? (masterData = MasterDataManager.Instance.GetMasterData<MasterData.Character>(characterBasicData.MasterDataId));

        #region LOPEntityBase
        protected override void InitEntity()
        {
            base.InitEntity();

            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

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

        protected override void OnInitialize(EntityCreationData entityCreationData)
		{
            base.OnInitialize(entityCreationData);

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
            entitySnap.ownerId = OwnerId;

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
        public override float FactoredMovementSpeed => MovementSpeed * LOP.Game.Current.GameManager.MapData.mapEnvironment.MoveSpeedFactor;

		public FirstStatus FirstStatus => characterStatusData.firstStatus;
        public SecondStatus SecondStatus => characterStatusData.secondStatus;
		#endregion
	}
}
