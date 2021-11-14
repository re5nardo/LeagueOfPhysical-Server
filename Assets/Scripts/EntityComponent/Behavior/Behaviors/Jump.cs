using UnityEngine;
using GameEvent;

namespace Behavior
{
    public class Jump : BehaviorBase
    {
        public enum JumpType
        {
            AddForce = 0,
            FlapJump = 1,
        }

        private JumpBehaviorParam param;

        #region BehaviorBase
        protected override void OnInitialize(BehaviorParam behaviorParam)
        {
            param = behaviorParam as JumpBehaviorParam;
        }

        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.id), Entity.Position);

            //Entity.SendCommandToViews(new AnimatorSetBool("Jump", true));
        }

        protected override bool OnBehaviorUpdate()
        {
            if (param.jumpType == JumpType.AddForce)
            {
                Entity.Rigidbody.AddForce(param.normalizedPower * param.direction.normalized * LOP.Game.Current.GameManager.MapData.mapEnvironment.JumpPowerFactor, ForceMode.VelocityChange);
            }
            else if (param.jumpType == JumpType.FlapJump)
            {
                Entity.Velocity = new Vector3(Entity.Velocity.x, LOP.Game.Current.GameManager.MapData.mapEnvironment.JumpPowerFactor, Entity.Velocity.z);
            }

            return false;
        }
        #endregion
    }
}
