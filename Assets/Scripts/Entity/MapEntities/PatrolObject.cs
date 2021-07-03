using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior;

namespace Entity
{
    public class PatrolObject : MapObjectBase
    {
        [SerializeField] private Vector3 startPoint;
        [SerializeField] private Vector3 halfwayPoint;
        [SerializeField] float speed = 1;
        [SerializeField] float timeOffset;

        public override float MovementSpeed => speed;
        public override float FactoredMovementSpeed => speed * SubGameBase.Current.SubGameEnvironment.MoveSpeedFactor;

        public override void Initialize(params object[] param)
        {
            base.Initialize(param);
            
            ContinuousPatrol continuousPatrol = BehaviorFactory.Instance.CreateBehavior(gameObject, Define.MasterData.BehaviorID.CONTINUOUS_PATROL) as ContinuousPatrol;
            AttachComponent(continuousPatrol);
            continuousPatrol.SetData(Define.MasterData.BehaviorID.CONTINUOUS_PATROL, startPoint, halfwayPoint, timeOffset);
            continuousPatrol.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

            continuousPatrol.StartBehavior();
        }
    }
}
