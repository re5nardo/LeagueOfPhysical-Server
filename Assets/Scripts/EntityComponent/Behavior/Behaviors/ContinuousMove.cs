using System.Collections;
using UnityEngine;

namespace Behavior
{
    public class ContinuousMove : BehaviorBase
    {
        private Vector3 direction;

        #region BehaviorBase
        public override void Initialize(BehaviorParam behaviorParam)
        {
            base.Initialize(behaviorParam);

            var continuousMoveBehaviorParam = behaviorParam as ContinuousMoveBehaviorParam;

            direction = continuousMoveBehaviorParam.direction;
        }

        protected override bool OnBehaviorUpdate()
        {
            var velocity = direction.normalized * Entity.FactoredMovementSpeed;

            var x = velocity.x == 0 ? Entity.Velocity.x : velocity.x;
            var y = velocity.y == 0 ? Entity.Velocity.y : velocity.y;
            var z = velocity.z == 0 ? Entity.Velocity.z : velocity.z;

            Entity.Velocity = new Vector3(x, y, z);

            return true;
        }
        #endregion
    }
}
