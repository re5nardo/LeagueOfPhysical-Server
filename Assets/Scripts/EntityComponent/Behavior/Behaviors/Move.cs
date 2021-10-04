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
        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.ID, Destination), Entity.Position);

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
                    Vector3 moved = (Destination - startPosition).normalized * Entity.FactoredMovementSpeed * DeltaTime;
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

        public override void Initialize(BehaviorParam behaviorParam)
        {
            base.Initialize(behaviorParam);

            var moveBehaviorParam = behaviorParam as MoveBehaviorParam;

            Destination = moveBehaviorParam.destination;
            startPosition = Entity.Position;
        }
        #endregion

        public void SetDestination(Vector3 destination)
        {
            Destination = destination;
            startPosition = Entity.Position;
        }
    }
}
