using UnityEngine;
using GameEvent;
using EntityCommand;

namespace Behavior
{
    public class Jump : BehaviorBase
    {
        #region ISynchronizable
        public override bool Enable => false;
        #endregion

        #region BehaviorBase
        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.ID), Entity.Position);

            //Entity.SendCommandToViews(new AnimatorSetBool("Jump", true));
        }

        protected override bool OnBehaviorUpdate()
        {
            Entity.ModelRigidbody.AddForce(Vector3.up * 1000, ForceMode.Impulse);

            return false;
        }
        #endregion
    }
}