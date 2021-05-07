using UnityEngine;
using GameFramework;
using GameEvent;
using EntityCommand;

namespace Behavior
{
    public class Move : BehaviorBase
    {
        private Vector3 m_vec3Destination;
        private int remainCount = 3;

        #region ISynchronizable
        protected override ISnap LastSendSnap { get; set; } = new MoveSnap();
        protected override ISnap CurrentSnap { get; set; } = new MoveSnap();
        #endregion

        #region BehaviorBase
        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, MasterData.ID, (SerializableVector3)m_vec3Destination), Entity.Position);

            Entity.SendCommandToViews(new AnimatorSetBool("Move", true));
        }

        protected override bool OnBehaviorUpdate()
        {
			Vector3 toMove = m_vec3Destination.XZ() - Entity.Position.XZ();

            if (Entity.ModelRigidbody.isKinematic)
            {
                Vector3 moved = toMove.normalized * Entity.MovementSpeed * DeltaTime;

                if (Util.Approximately(toMove.sqrMagnitude, moved.sqrMagnitude) || toMove.sqrMagnitude <= moved.sqrMagnitude)
                {
                    Entity.Position = m_vec3Destination;
                    return false;
                }
                else
                {
                    Entity.Position += moved;
                    return true;
                }
            }
            else
            {
                var force = toMove.XZ().normalized * Entity.MovementSpeed / 0.2f * DeltaTime;

                Entity.ModelRigidbody.velocity += force;

                if (Entity.ModelRigidbody.velocity.XZ().magnitude >= Entity.MovementSpeed)
                {
                    var xz = Entity.ModelRigidbody.velocity.XZ().normalized * Entity.MovementSpeed;

                    Entity.ModelRigidbody.velocity = new Vector3(xz.x, Entity.ModelRigidbody.velocity.y, xz.z);
                }

                return --remainCount > 0;
            }
        }

        protected override void OnBehaviorEnd()
        {
            base.OnBehaviorEnd();

            Entity.SendCommandToViews(new AnimatorSetBool("Move", false));
        }

        public override void SetData(int nBehaviorMasterID, params object[] param)
        {
            base.SetData(nBehaviorMasterID);

            m_vec3Destination = (Vector3)param[0];
            remainCount = 3;
        }
        #endregion

        public void SetDestination(Vector3 vec3Destination)
        {
            m_vec3Destination = vec3Destination;
            remainCount = 3;
        }

        public Vector3 GetDestination()
        {
            return m_vec3Destination;
        }
    }
}
