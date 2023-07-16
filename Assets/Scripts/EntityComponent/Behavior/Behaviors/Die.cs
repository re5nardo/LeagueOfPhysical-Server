using System.Collections.Generic;
using UnityEngine;
using GameEvent;

namespace Behavior
{
    public class Die : BehaviorBase
    {
        #region BehaviorBase
        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityId, MasterData.id), Entity.Position);

            Entity.Rigidbody.detectCollisions = false;
        }

        protected override bool OnBehaviorUpdate()
        {
            return CurrentUpdateTime < MasterData.lifespan;
        }

        protected override void OnBehaviorEnd()
        {
            base.OnBehaviorEnd();

            Entity.Rigidbody.detectCollisions = true;
        }
        #endregion
    }
}
