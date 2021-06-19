using UnityEngine;
using GameFramework;

namespace Behavior
{
    public class ContinuousRotation : BehaviorBase
    {
        private Vector3 startRotation;

        #region ISynchronizable
        public override bool Enable => false;
        #endregion

        #region BehaviorBase
        protected override bool OnBehaviorUpdate()
        {
            Entity.Rotation = GetRotationByTick();

            return true;
        }

        public override void SetData(int nBehaviorMasterID, params object[] param)
        {
            base.SetData(nBehaviorMasterID);

            startRotation = (Vector3)param[0];
        }
        #endregion

        public Vector3 GetRotationByTick()
        {
            var value = startRotation + Entity.AngularVelocity * Game.Current.GameTime;

            return new Vector3(value.x % 360, value.y % 360, value.z % 360);
        }
    }
}
