using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public class PatrolObject : MapObjectBase
    {
        [SerializeField] private Vector3 startPoint;
        [SerializeField] private Vector3 halfwayPoint;
        [SerializeField] private float speed = 1;
        [SerializeField] private float timeOffset;

        public override float MovementSpeed => speed;
        public override float FactoredMovementSpeed => speed * LOP.Game.Current.GameManager.MapData.mapEnvironment.MoveSpeedFactor;

        protected override void OnInitialize(EntityCreationData entityCreationData)
        {
            base.OnInitialize(entityCreationData);
            
            BehaviorController.StartBehavior(new ContinuousPatrolBehaviorParam(Define.MasterData.BehaviorId.ContinuousPatrol, startPoint, halfwayPoint, timeOffset));
        }
    }
}
