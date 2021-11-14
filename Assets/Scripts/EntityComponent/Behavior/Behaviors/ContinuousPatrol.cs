using UnityEngine;
using GameFramework;

namespace Behavior
{
    public class ContinuousPatrol : BehaviorBase
    {
        private Vector3 startPoint;
        private Vector3 halfwayPoint;
        private float timeOffset;

        #region BehaviorBase
        protected override void OnInitialize(BehaviorParam behaviorParam)
        {
            var continuousPatrolBehaviorParam = behaviorParam as ContinuousPatrolBehaviorParam;

            startPoint = continuousPatrolBehaviorParam.startPoint;
            halfwayPoint = continuousPatrolBehaviorParam.halfwayPoint;
            timeOffset = continuousPatrolBehaviorParam.timeOffset;
        }

        protected override bool OnBehaviorUpdate()
        {
            return true;
        }
        #endregion

        private void Update()
        {
            Entity.Position = GetPositionByTime();
        }

        private Vector3 GetPositionByTime()
        {
            var halfMagnitude = (halfwayPoint - startPoint).magnitude;
            var distance = Entity.FactoredMovementSpeed * (Game.Current.GameTime + timeOffset);

            if ((int)(distance / halfMagnitude) % 2 == 0)
            {
                return Vector3.Lerp(startPoint, halfwayPoint, (distance % halfMagnitude) / halfMagnitude);
            }
            else
            {
                return Vector3.Lerp(halfwayPoint, startPoint, (distance % halfMagnitude) / halfMagnitude);
            }
        }
    }
}
