using UnityEngine;
using GameEvent;

namespace Behavior
{
    public class Jump : BehaviorBase
    {
        private float normalizedPower;
        private Vector3 direction;

        #region BehaviorBase
        public override void Initialize(BehaviorParam behaviorParam)
        {
            base.Initialize(behaviorParam);

            var jumpBehaviorParam = behaviorParam as JumpBehaviorParam;

            normalizedPower = jumpBehaviorParam.normalizedPower;
            direction = jumpBehaviorParam.direction;
        }


        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.ID), Entity.Position);

            //Entity.SendCommandToViews(new AnimatorSetBool("Jump", true));
        }

        protected override bool OnBehaviorUpdate()
        {
            Entity.Rigidbody.AddForce(normalizedPower * direction.normalized * LOP.Game.Current.GameManager.MapData.mapEnvironment.JumpPowerFactor, ForceMode.VelocityChange);

            return false;
        }
        #endregion
    }
}
