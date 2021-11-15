using System.Collections.Generic;
using UnityEngine;
using NetworkModel.Mirror;
using GameFramework;
using System.Linq;

namespace Entity
{
    public abstract class LOPMonoEntityBase : MonoEntityBase
    {
        public EntityType EntityType { get; protected set; } = EntityType.None;
        public EntityRole EntityRole { get; protected set; } = EntityRole.None;

        public bool IsValid => EntityManager.Instance.IsRegistered(EntityID);
        public bool IsLocalEntity => EntityID < 0;
        public bool Initialized { get; private set; }

        private string ownerId = "server";
        public string OwnerId
        {
            get => ownerId;
            set
            {
                ownerId = value;

                SC_OwnerChanged message = ObjectPool.Instance.GetObject<SC_OwnerChanged>();
                message.entityId = EntityID;
                message.ownerId = ownerId;

                RoomNetwork.Instance.SendToAll(message);

                ObjectPool.Instance.ReturnObject(message);
            }
        }

        public bool HasAuthority => OwnerId == "server" || OwnerId == "local";

        public EntityBasicView EntityBasicView { get; protected set; }

        public BehaviorController BehaviorController { get; private set; }
        public StateController StateController { get; private set; }

        public Blackboard Blackboard { get; private set; }

        public Transform Transform { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public CollisionReporter CollisionReporter { get; private set; }

        public SimplePubSubService MessageBroker { get; } = new SimplePubSubService();

        protected virtual void Awake()
        {
            InitEntity();
            InitEntityComponents();
        }

        protected virtual void OnDestroy()
        {
            MessageBroker.Clear();
        }

        protected virtual void InitEntity()
        {
            Transform = gameObject.GetComponent<Transform>();
            Rigidbody = gameObject.AddComponent<Rigidbody>();
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Rigidbody.drag = 0.05f;
            CollisionReporter = gameObject.AddComponent<CollisionReporter>();
        }

        protected virtual void InitEntityComponents()
        {
            BehaviorController = AttachEntityComponent<BehaviorController>();
            StateController = AttachEntityComponent<StateController>();

            Blackboard = AttachEntityComponent<Blackboard>();
        }

        public void Initialize(EntityCreationData entityCreationData)
        {
            OnInitialize(entityCreationData);

            Initialized = true;
        }

        protected virtual void OnInitialize(EntityCreationData entityCreationData)
        {
            EntityID = entityCreationData.entityId;
            Position = entityCreationData.position;
            Rotation = entityCreationData.rotation;
            Velocity = entityCreationData.velocity;
            AngularVelocity = entityCreationData.angularVelocity;
            EntityType = entityCreationData.entityType;
            EntityRole = entityCreationData.entityRole;
            OwnerId = entityCreationData.ownerId;
        }

        public virtual void OnTick(int tick)
        {
            //  States
            GetEntityComponents<State.StateBase>()?.ForEach(state =>
            {
                state.OnTick(tick);
            });

            //  Behaviors
            GetEntityComponents<Behavior.BehaviorBase>()?.ForEach(behavior =>
            {
                behavior.OnTick(tick);
            });

            //  Skills
            GetEntityComponents<Skill.SkillBase>()?.ForEach(skill =>
            {
                skill.OnTick(tick);
            });
        }

        public bool IsGrounded
        {
            get
            {
                var halfExtents = new Vector3(ModelCollider.bounds.extents.x, 0.05f, ModelCollider.bounds.extents.z);

                var colliders = Physics.OverlapBox(Position, halfExtents, Quaternion.Euler(Rotation)).ToList();
                colliders.Remove(ModelCollider);

                return colliders.Count > 0;
            }
        }

        public virtual EntitySnap GetEntitySnap()
        {
            var entitySnap = new EntitySnap();

            entitySnap.entityId = EntityID;
            entitySnap.entityType = EntityType;
            entitySnap.entityRole = EntityRole;
            entitySnap.ownerId = OwnerId;
            entitySnap.position = Position;
            entitySnap.rotation = Rotation;
            entitySnap.velocity = Velocity;
            entitySnap.angularVelocity = AngularVelocity;

            return entitySnap;
        }

        #region IEntity
        public override Vector3 Position
        {
            get => Transform.position;
            set
            {
                Transform.position = value;

                SceneMessageBroker.Publish(new GameMessage.EntityMove(EntityID));
            }
        }

        public override Vector3 Rotation
        {
            get => Transform.rotation.eulerAngles;
            set
            {
                Transform.rotation = Quaternion.Euler(value);
            }
        }

        private Vector3 velocity;
        public override Vector3 Velocity
        {
            get => Rigidbody.isKinematic ? velocity : Rigidbody.velocity;
            set => Rigidbody.velocity = velocity = value;
        }

        private Vector3 angularVelocity;
        public override Vector3 AngularVelocity
        {
            get => Rigidbody.isKinematic ? angularVelocity : Rigidbody.angularVelocity;
            set => Rigidbody.angularVelocity = angularVelocity = value;
        }
        #endregion

        #region Interface For Convenience
        public abstract float MovementSpeed { get; }
        public abstract float FactoredMovementSpeed { get; }

        public Collider ModelCollider => EntityBasicView.ModelCollider;
        public Animator ModelAnimator => EntityBasicView.ModelAnimator;

        public Vector3 Forward => (Quaternion.Euler(Rotation) * Vector3.forward).normalized;
        public Vector3 Up => (Quaternion.Euler(Rotation) * Vector3.up).normalized;
        public Vector3 Down => (Quaternion.Euler(Rotation) * Vector3.down).normalized;

        public bool HasStatusEffect(StatusEffect statusEffect)
        {
            var behaviors = GetEntityComponents<Behavior.BehaviorBase>();
            if (behaviors != null)
            {
                foreach (var behavior in behaviors)
                {
                    if (behavior.MasterData.statusEffects != null && behavior.MasterData.statusEffects.Any(x => x == statusEffect))
                    {
                        return true;
                    }
                }
            }

            var states = GetEntityComponents<State.StateBase>();
            if (states != null)
            {
                foreach (var state in states)
                {
                    if (state.MasterData.statusEffects != null && state.MasterData.statusEffects.Any(x => x == statusEffect))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public State.StateBase GetState(int masterDataId)
        {
            return GetEntityComponents<State.StateBase>()?.Find(state => state.MasterDataId == masterDataId);
        }
        #endregion
    }
}
