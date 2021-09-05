using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public abstract class MapObjectBase : MonoEntityBase
    {
        [SerializeField] private GameObject model = null;

        protected override void Awake()
        {
            base.Awake();

            Initialize();
            Position = model.transform.position;
            Rotation = model.transform.rotation.eulerAngles;
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;

            EntityManager.Instance.RegisterEntity(this);
        }

        protected override void InitEntity()
        {
            base.InitEntity();

            Rigidbody.isKinematic = true;
            Rigidbody.useGravity = false;
        }

        public override void Initialize(params object[] param)
        {
            base.Initialize(param);

            EntityID = EntityManager.Instance.GenerateLocalEntityID();
            EntityType = EntityType.MapObject;
            EntityRole = EntityRole.NPC;
            
            entityBasicView.SetModel(model);
        }

        protected override void InitEntityComponents()
        {
            base.InitEntityComponents();

            entityBasicView = AttachEntityComponent(gameObject.AddComponent<EntityBasicView>());
        }
    }
}
