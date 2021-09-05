using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameFramework;
using NetworkModel.Mirror;

namespace Entity
{
	public abstract class MonoEntityBase : MonoBehaviour, IEntity
	{
        public EntityType EntityType { get; protected set; } = EntityType.None;
        public EntityRole EntityRole { get; protected set; } = EntityRole.None;

        public bool IsValid => EntityManager.Instance.IsRegistered(EntityID);
        public bool IsLocalEntity => EntityID < 0;

        public bool HasAuthority { get; set; }

        private List<IEntityComponent> entityComponents = new List<IEntityComponent>();

        protected EntityBasicView entityBasicView;

        protected virtual void Awake()
		{
			InitComponents();
		}

        protected virtual void OnDestroy()
        {
            DetachAllComponents();
        }

		protected virtual void InitComponents()
		{
        }

		public virtual void Initialize(params object[] param)
		{
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

        #region Interface For Convenience
        public abstract float MovementSpeed { get; }
        public abstract float FactoredMovementSpeed { get; }

        public Transform ModelTransform => entityBasicView.ModelTransform;
        public Rigidbody ModelRigidbody => entityBasicView.ModelRigidbody;
        public Collider ModelCollider => entityBasicView.ModelCollider;
        public Animator ModelAnimator => entityBasicView.ModelAnimator;

        public Vector3 Forward { get { return (Quaternion.Euler(Rotation) * Vector3.forward).normalized; } }
        public Vector3 Up { get { return (Quaternion.Euler(Rotation) * Vector3.up).normalized; } }
        public Vector3 Down { get { return (Quaternion.Euler(Rotation) * Vector3.down).normalized; } }

        public virtual EntitySnapInfo GetEntitySnapInfo()
        {
            var entitySnapInfo = new EntitySnapInfo();

            entitySnapInfo.entityId = EntityID;
            entitySnapInfo.entityType = EntityType;
            entitySnapInfo.entityRole = EntityRole;
            entitySnapInfo.position = Position;
            entitySnapInfo.rotation = Rotation;
            entitySnapInfo.velocity = Velocity;
            entitySnapInfo.angularVelocity = AngularVelocity;

            return entitySnapInfo;
        }
        #endregion

        #region IEntity
        public int EntityID { get; protected set; } = -1;

        public Vector3 Position
        {
            get => ModelTransform.position;
            set
            {
                GamePubSubService.Publish(GameMessageKey.EntityMove, new object[] { EntityID });

                ModelTransform.position = value;
            }
        }

        public Vector3 Rotation
        {
            get => ModelTransform.rotation.eulerAngles;
            set
            {
                ModelTransform.rotation = Quaternion.Euler(value);
            }
        }

        public Vector3 Velocity
        {
            get => ModelRigidbody.velocity;
            set
            {
                ModelRigidbody.velocity = value;
            }
        }

        public Vector3 AngularVelocity
        {
            get => ModelRigidbody.angularVelocity;
            set
            {
                ModelRigidbody.angularVelocity = value;
            }
        }

        public T AttachEntityComponent<T>(T entityComponent) where T : IEntityComponent
        {
            entityComponents.Add(entityComponent);

            entityComponent.OnAttached(this);

            return entityComponent;
        }

        public T DetachEntityComponent<T>(T entityComponent) where T : IEntityComponent
        {
            entityComponents.Remove(entityComponent);

            entityComponent.OnDetached();

            return entityComponent;
        }

        public void DetachAllComponents()
        {
            var iteration = new List<IEntityComponent>(entityComponents);

            iteration.ForEach(component => DetachEntityComponent(component));
        }

        public T GetEntityComponent<T>() where T : IEntityComponent
        {
            var found = entityComponents.Find(x => x is T);

            if (found == null)
                return default;

            return (T)found;
        }

        public List<T> GetEntityComponents<T>() where T : IEntityComponent
        {
            var found = entityComponents.FindAll(x => x is T);

            if (found == null)
                return null;

            return found.Cast<T>().ToList();
        }

        public void SendCommandToAll(ICommand command)
        {
            List<IEntityComponent> temp = new List<IEntityComponent>(entityComponents);

            foreach (var entityComponent in temp)
            {
                if (!entityComponents.Contains(entityComponent))
                    continue;

                entityComponent.OnCommand(command);
            }
        }

        public void SendCommand(ICommand command, List<Type> cullings)
        {
            List<IEntityComponent> temp = new List<IEntityComponent>(entityComponents);

            foreach (var entityComponent in temp)
            {
                if (!entityComponents.Contains(entityComponent))
                    continue;

                if (cullings.Exists(x => x.IsAssignableFrom(entityComponent.GetType())))
                {
                    entityComponent.OnCommand(command);
                }
            }
        }

        public void SendCommandToViews(ICommand command)
        {
            SendCommand(command, new List<Type> { typeof(ViewComponentBase), typeof(MonoViewComponentBase) });
        }
        #endregion
    }
}
