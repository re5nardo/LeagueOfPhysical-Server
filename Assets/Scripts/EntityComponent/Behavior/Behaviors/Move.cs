using UnityEngine;
using GameFramework;
using GameEvent;
using EntityMessage;

namespace Behavior
{
    public class Move : BehaviorBase
    {
        public Vector3 Destination { get; private set; }
        private Vector3 startPosition;

        #region BehaviorBase
        protected override void OnInitialize(BehaviorParam behaviorParam)
        {
            var moveBehaviorParam = behaviorParam as MoveBehaviorParam;

            Destination = moveBehaviorParam.destination;
            startPosition = Entity.Position;
        }

        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityId, MasterData.id, Destination), Entity.Position);

            Entity.MessageBroker.Publish(new AnimatorSetBool("Move", true));
        }

        protected override bool OnBehaviorUpdate()
        {
            if (Entity.Rigidbody.isKinematic)
            {
                if ((Destination - startPosition).sqrMagnitude <= (Entity.Position - startPosition).sqrMagnitude)
                {
                    Entity.Position = Destination;
                    return false;
                }
                else
                {
                    Vector3 moved = (Destination - startPosition).normalized * Entity.FactoredMovementSpeed * (float)DeltaTime;
                    Entity.Position += moved;
                    return true;
                }
            }
            else
            {
                if ((Destination - startPosition).sqrMagnitude <= (Entity.Position - startPosition).sqrMagnitude)
                {
                    Entity.Position = Destination;
                    return false;
                }
                else
                {
                    var xz = (Destination.XZ() - startPosition.XZ()).normalized * Entity.FactoredMovementSpeed;
                    Entity.Velocity = new Vector3(xz.x, Entity.Velocity.y, xz.z);

                    if (Entity.Velocity.XZ().magnitude >= Entity.FactoredMovementSpeed)
                    {
                        xz = Entity.Velocity.XZ().normalized * Entity.FactoredMovementSpeed;

                        Entity.Velocity = new Vector3(xz.x, Entity.Velocity.y, xz.z);
                    }

                    return true;
                }
            }
        }

        protected override void OnBehaviorEnd()
        {
            base.OnBehaviorEnd();

            Entity.MessageBroker.Publish(new AnimatorSetBool("Move", false));
        }
        #endregion

        public void SetDestination(Vector3 destination)
        {
            Destination = destination;
            startPosition = Entity.Position;
        }
    }
}
