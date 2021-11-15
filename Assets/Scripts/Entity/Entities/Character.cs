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

        public CharacterBasicData CharacterBasicData { get; private set; }
        public CharacterStatusData CharacterStatusData { get; private set; }
        public CharacterAbilityData CharacterAbilityData { get; private set; }

        public SkillController SkillController { get; private set; }
        public CharacterStatusController CharacterStatusController { get; private set; }
        public CharacterAbilityController CharacterAbilityController { get; private set; }

        private MasterData.Character masterData = null;
        public MasterData.Character MasterData => masterData ?? (masterData = MasterDataManager.Instance.GetMasterData<MasterData.Character>(CharacterBasicData.MasterDataId));

        #region LOPEntityBase
        protected override void InitEntity()
        {
            base.InitEntity();

            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        protected override void InitEntityComponents()
		{
			base.InitEntityComponents();

            CharacterBasicData = AttachEntityComponent<CharacterBasicData>();
            CharacterStatusData = AttachEntityComponent<CharacterStatusData>();
            CharacterAbilityData = AttachEntityComponent<CharacterAbilityData>();

            EntityBasicView = AttachEntityComponent<CharacterView>();

            SkillController = AttachEntityComponent<SkillController>();
            CharacterStatusController = AttachEntityComponent<CharacterStatusController>();
            CharacterAbilityController = AttachEntityComponent<CharacterAbilityController>();
		}

        protected override void OnInitialize(EntityCreationData entityCreationData)
		{
            base.OnInitialize(entityCreationData);

            CharacterBasicData.Initialize(entityCreationData);
            CharacterStatusData.Initialize(entityCreationData);
		}

        public override EntitySnap GetEntitySnap()
        {
            var entitySnap = new CharacterSnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.masterDataId = CharacterBasicData.MasterDataId;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;
            entitySnap.modelId = CharacterBasicData.ModelId;
            entitySnap.firstStatus = CharacterStatusData.firstStatus;
            entitySnap.secondStatus = CharacterStatusData.secondStatus;
            entitySnap.ownerId = OwnerId;

            return entitySnap;
        }
        #endregion

        #region Interface For Convenience
        public int HP
        {
			get => CharacterStatusData.HP;
            set => CharacterStatusData.HP = value;
        }

        public int MaximumHP => CharacterStatusData.MaximumHP;

        public bool IsAlive => CharacterStatusData.HP > 0;

        public override float MovementSpeed => CharacterStatusData.MovementSpeed;
        public override float FactoredMovementSpeed => MovementSpeed * LOP.Game.Current.GameManager.MapData.mapEnvironment.MoveSpeedFactor;

		public FirstStatus FirstStatus => CharacterStatusData.firstStatus;
        public SecondStatus SecondStatus => CharacterStatusData.secondStatus;
		#endregion
	}
}
