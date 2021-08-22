using UnityEngine;
using GameFramework;

namespace Behavior
{
    public class ContinuousRotation : BehaviorBase
    {
        private Vector3 startRotation;
        private float timeOffset;

        #region BehaviorBase
        protected override bool OnBehaviorUpdate()
        {
            return true;
        }

        public override void SetData(int nBehaviorMasterID, params object[] param)
        {
            base.SetData(nBehaviorMasterID);

            startRotation = (Vector3)param[0];
            timeOffset = (float)param[1];
        }
        #endregion

        private void Update()
        {
            Entity.Rotation = GetRotationByTime();
        }

        private Vector3 GetRotationByTime()
        {
            var value = startRotation + Entity.AngularVelocity * (Game.Current.GameTime + timeOffset);

            return new Vector3(value.x % 360, value.y % 360, value.z % 360);
        }
    }
}
