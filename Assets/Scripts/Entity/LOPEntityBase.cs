using System.Collections.Generic;
using UnityEngine;
using NetworkModel.Mirror;

namespace Entity
{
    public abstract class LOPEntityBase : MonoEntityBase
    {
        public EntityType EntityType { get; protected set; } = EntityType.None;
        public EntityRole EntityRole { get; protected set; } = EntityRole.None;

        public bool IsValid => EntityManager.Instance.IsRegistered(EntityID);
        public bool IsLocalEntity => EntityID < 0;

        public string OwnerId { get; set; } = "server";
        public bool HasAuthority => OwnerId == "server" || OwnerId == "local";

        protected EntityBasicView entityBasicView;

        public Transform Transform { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        protected virtual void Awake()
        {
            InitEntity();
            InitEntityComponents();
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

        public virtual void Initialize(EntityCreationData entityCreationData)
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

        #region Interface For Convenience
        public abstract float MovementSpeed { get; }
        public abstract float FactoredMovementSpeed { get; }

        public Collider ModelCollider => entityBasicView.ModelCollider;
        public Animator ModelAnimator => entityBasicView.ModelAnimator;

        public Vector3 Forward { get { return (Quaternion.Euler(Rotation) * Vector3.forward).normalized; } }
        public Vector3 Up { get { return (Quaternion.Euler(Rotation) * Vector3.up).normalized; } }
        public Vector3 Down { get { return (Quaternion.Euler(Rotation) * Vector3.down).normalized; } }
        #endregion

        #region IEntity
        public override Vector3 Position
        {
            get => Transform.position;
            set
            {
                GamePubSubService.Publish(GameMessageKey.EntityMove, new object[] { EntityID });

                Transform.position = value;
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
    }
}
