using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameFramework;
using EntityCommand;

namespace Entity
{
	public abstract class MonoEntityBase : MonoBehaviour, IEntity
	{
        public EntityType EntityType { get; protected set; } = EntityType.None;
        public EntityRole EntityRole { get; protected set; } = EntityRole.None;

        public bool IsValid => EntityManager.Instance.IsRegistered(EntityID);
        public bool IsLocalEntity => EntityID < 0;

        private List<IComponent> components = new List<IComponent>();

        protected EntityBasicView entityBasicView = null;

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

        #region Interface For Convenience
        public abstract float MovementSpeed { get; }
        public Transform ModelTransform => entityBasicView.ModelTransform;
        public Rigidbody ModelRigidbody => entityBasicView.ModelRigidbody;

        public Vector3 Forward { get { return (Quaternion.Euler(Rotation) * Vector3.forward).normalized; } }

        public virtual EntitySnapInfo GetEntitySnapInfo()
        {
            EntitySnapInfo entitySnapInfo = new EntitySnapInfo();

            entitySnapInfo.m_nEntityID = EntityID;
            entitySnapInfo.m_EntityType = EntityType;
            entitySnapInfo.m_EntityRole = EntityRole;
            entitySnapInfo.m_Position = Position;
            entitySnapInfo.m_Rotation = Rotation;
            entitySnapInfo.m_Velocity = Velocity;
            entitySnapInfo.m_AngularVelocity = AngularVelocity;

            return entitySnapInfo;
        }
        #endregion

        #region IEntity
        public int EntityID { get; protected set; } = -1;

        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;

                GamePubSubService.Publish(GameMessageKey.EntityMove, new object[] { EntityID });

                SendCommandToViews(new PositionChanged());
            }
        }

        private Vector3 rotation;
        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;

                SendCommandToViews(new RotationChanged());
            }
        }

        private Vector3 velocity;
        public Vector3 Velocity
        {
            get => velocity;
            set
            {
                velocity = value;

                SendCommandToViews(new VelocityChanged());
            }
        }

        private Vector3 angularVelocity;
        public Vector3 AngularVelocity
        {
            get => angularVelocity;
            set
            {
                angularVelocity = value;

                SendCommandToViews(new AngularVelocityChanged());
            }
        }

        public T AttachComponent<T>(T component) where T : IComponent
        {
            components.Add(component);

            component.OnAttached(this);

            return component;
        }

        public T DetachComponent<T>(T component) where T : IComponent
        {
            components.Remove(component);

            component.OnDetached();

            return component;
        }

        public void DetachAllComponents()
        {
            var iteration = new List<IComponent>(components);

            iteration.ForEach(component => DetachComponent(component));
        }

        public T GetEntityComponent<T>() where T : IComponent
        {
            var found = components.Find(x => x is T);

            if (found == null)
                return default;

            return (T)found;
        }

        public List<T> GetEntityComponents<T>() where T : IComponent
        {
            var found = components.FindAll(x => x is T);

            if (found == null)
                return null;

            return found.Cast<T>().ToList();
        }

        public void SendCommandToAll(ICommand command)
        {
            List<IComponent> temp = new List<IComponent>(components);

            foreach (var component in temp)
            {
                if (!components.Contains(component))
                    continue;

                component.OnCommand(command);
            }
        }

        public void SendCommand(ICommand command, List<Type> cullings)
        {
            List<IComponent> temp = new List<IComponent>(components);

            foreach (var component in temp)
            {
                if (!components.Contains(component))
                    continue;

                if (cullings.Exists(x => x.IsAssignableFrom(component.GetType())))
                {
                    component.OnCommand(command);
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
