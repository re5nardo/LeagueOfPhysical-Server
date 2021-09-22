using System.Collections.Generic;
using UnityEngine;
using NetworkModel.Mirror;
using GameFramework;

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

        protected EntityBasicView entityBasicView;

        public Transform Transform { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

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
        }

        protected virtual void InitEntityComponents()
        {
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
                float ySize = 0.01f;
                float maxDistance = 0.05f;
                var center = Position + Up * ySize;
                var halfExtents = new Vector3(ModelCollider.bounds.extents.x, ySize / 2, ModelCollider.bounds.extents.z);

                return Physics.BoxCast(center, halfExtents, Down, Quaternion.Euler(Rotation), maxDistance);
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

        public override Vector3 Velocity
        {
            get => Rigidbody.velocity;
            set
            {
                Rigidbody.velocity = value;
            }
        }

        public override Vector3 AngularVelocity
        {
            get => Rigidbody.angularVelocity;
            set
            {
                Rigidbody.angularVelocity = value;
            }
        }
        #endregion

        #region Interface For Convenience
        public abstract float MovementSpeed { get; }
        public abstract float FactoredMovementSpeed { get; }

        public Collider ModelCollider => entityBasicView.ModelCollider;
        public Animator ModelAnimator => entityBasicView.ModelAnimator;

        public Vector3 Forward => (Quaternion.Euler(Rotation) * Vector3.forward).normalized;
        public Vector3 Up => (Quaternion.Euler(Rotation) * Vector3.up).normalized;
        public Vector3 Down => (Quaternion.Euler(Rotation) * Vector3.down).normalized;
        #endregion
    }
}
