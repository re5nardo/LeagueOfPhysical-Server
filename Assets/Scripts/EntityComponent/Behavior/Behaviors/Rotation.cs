using UnityEngine;
using GameEvent;

namespace Behavior
{
    public class Rotation : BehaviorBase
    {
        public Vector3 Direction { get; set; }
        public Vector3 Destination => Quaternion.LookRotation(Direction).eulerAngles;

        // Angular speed in degrees per sec.
		private float m_fAngularSpeed = 360 * 2;

        #region BehaviorBase
        protected override void OnInitialize(BehaviorParam behaviorParam)
        {
            var rotationBehaviorParam = behaviorParam as RotationBehaviorParam;

            Direction = rotationBehaviorParam.direction;
        }

        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.id, Direction), Entity.Position);
        }

        protected override bool OnBehaviorUpdate()
        {
            float toRotate = Vector3.SignedAngle(Entity.Forward, Direction, Vector3.up);
			if (toRotate == 0)
            {
                return false;
            }

            var angularVelocity = new Vector3(0, (toRotate > 0 ? 1 : -1) * m_fAngularSpeed, 0);

            float rotated = angularVelocity.y * (float)DeltaTime;

            if (Util.Approximately(toRotate, rotated) || Mathf.Abs(toRotate) <= Mathf.Abs(rotated))
            {
                Entity.Rotation = Quaternion.LookRotation(Direction).eulerAngles;
                return false;
            }
            else
            {
                Entity.Rotation = new Vector3(Entity.Rotation.x, (Entity.Rotation.y + rotated) % 360, Entity.Rotation.z);
                return true;
            }
		}
        #endregion
    }
}
