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

        public bool IsValid
        {
            get { return EntityManager.Instance.IsRegistered(EntityID); }
        }

        //	Controller
        protected PhysicsController m_PhysicsController = null;
        protected TransformAgent m_TransformAgent = null;

        private List<IComponent> m_listComponent = new List<IComponent>();

        protected virtual void Awake()
		{
			InitComponents();
		}

		protected virtual void InitComponents()
		{
            //	Controller
            m_PhysicsController = AttachComponent(gameObject.AddComponent<PhysicsController>());
            m_TransformAgent = AttachComponent(gameObject.AddComponent<TransformAgent>());
        }

		public virtual void Initialize(params object[] param)
		{
		}

        #region Interface For Convenience
        public abstract float MovementSpeed { get; }
        public abstract void Move(Vector3 vec3Destination);

        public Vector3 Forward { get { return (Quaternion.Euler(Rotation) * Vector3.forward).normalized; } }

        public virtual EntitySnapInfo GetEntitySnapInfo()
        {
            EntitySnapInfo entitySnapInfo = new EntitySnapInfo();

            entitySnapInfo.m_nEntityID = EntityID;
            entitySnapInfo.m_EntityType = EntityType;
            entitySnapInfo.m_Position = Position;
            entitySnapInfo.m_Rotation = Rotation;
            entitySnapInfo.m_Velocity = Velocity;
            entitySnapInfo.m_AngularVelocity = AngularVelocity;

            return entitySnapInfo;
        }
        #endregion

        #region IEntity
        public virtual void Tick(int tick)
        {
            var allComponents = GetComponents<IComponent>();
            var tickables = allComponents.FindAll(x => x is ITickable).Cast<ITickable>().ToList();
            tickables.Sort((x, y) =>
            {
                int value_x = x is State.StateBase ? 300 : x is Behavior.BehaviorBase ? 200 : x is Skill.SkillBase ? 100 : 0;
                int value_y = y is State.StateBase ? 300 : y is Behavior.BehaviorBase ? 200 : y is Skill.SkillBase ? 100 : 0;

                return value_y.CompareTo(value_x);
            });

            tickables.ForEach(tickable =>
            {
                //  Iterating중에 Entity가 Destroy 안되었는지 체크
                if (IsValid)
                {
                    tickable.Tick(tick);
                }
            });
        }

        public int EntityID { get; protected set; } = -1;

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;

                RoomPubSubService.Instance.Publish(MessageKey.EntityMove, EntityID);

                SendCommandToViews(new PositionChanged());
            }
        }

        private Vector3 rotation;
        public Vector3 Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                SendCommandToViews(new RotationChanged());
            }
        }

        public Vector3 Velocity { get; set; }
        public Vector3 AngularVelocity { get; set; }

        public T AttachComponent<T>(T component) where T : IComponent
        {
            m_listComponent.Add(component);

            component.OnAttached(this);

            return component;
        }

        public T DetachComponent<T>(T component) where T : IComponent
        {
            m_listComponent.Remove(component);

            component.OnDetached();

            return component;
        }

        public T GetComponent<T>() where T : IComponent
        {
            var found = m_listComponent.Find(x => x is T);

            if (found == null)
                return default;

            return (T)found;
        }

        public List<T> GetComponents<T>() where T : IComponent
        {
            var found = m_listComponent.FindAll(x => x is T);

            if (found == null)
                return null;

            return found.Cast<T>().ToList();
        }

        public void SendCommandToAll(ICommand command)
        {
            List<IComponent> components = new List<IComponent>(m_listComponent);

            foreach (var component in components)
            {
                if (!m_listComponent.Contains(component))
                    continue;

                component.OnCommand(command);
            }
        }

        public void SendCommandToViews(ICommand command)
        {
            SendCommand(command, new List<Type> { typeof(IViewComponent) });
        }

        public void SendCommandToModels(ICommand command)
        {
            SendCommand(command, new List<Type> { typeof(IModelComponent) });
        }

        public void SendCommandToControllers(ICommand command)
        {
            SendCommand(command, new List<Type> { typeof(IControllerComponent) });
        }

        public void SendCommand(ICommand command, List<Type> cullings)
        {
            List<IComponent> components = new List<IComponent>(m_listComponent);

            foreach (var component in components)
            {
                if (!m_listComponent.Contains(component))
                    continue;

                if (cullings.Exists(x => x.IsAssignableFrom(component.GetType())))
                {
                    component.OnCommand(command);
                }
            }
        }
        #endregion

        #region PhysicsSimulation
        public virtual void OnBeforePhysicsSimulation(int tick)
        {
            BasicView basicView = GetComponent<BasicView>();

            basicView.ModelTransform.hasChanged = false;
            basicView.ModelTransform.GetComponent<Rigidbody>().isKinematic = false;
        }

        public virtual void OnAfterPhysicsSimulation(int tick)
        {
            BasicView basicView = GetComponent<BasicView>();

            if (basicView.ModelTransform.hasChanged)
            {
                if (Position != basicView.Position)
                {
                    Position = basicView.Position;
                }

                if (Rotation != basicView.Rotation)
                {
                    Rotation = basicView.Rotation;
                }
            }

            basicView.ModelTransform.GetComponent<Rigidbody>().isKinematic = true;
        }
        #endregion
    }
}
