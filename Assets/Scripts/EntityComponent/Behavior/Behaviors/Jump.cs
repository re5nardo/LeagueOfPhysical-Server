using UnityEngine;
using GameEvent;

namespace Behavior
{
    public class Jump : BehaviorBase
    {
        #region BehaviorBase
        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.ID), Entity.Position);

            //Entity.SendCommandToViews(new AnimatorSetBool("Jump", true));
        }

        protected override bool OnBehaviorUpdate()
        {
            Entity.ModelRigidbody.AddForce(Vector3.up * SubGameBase.Current.SubGameEnvironment.JumpPowerFactor, ForceMode.VelocityChange);

            return false;
        }
        #endregion
    }
}
