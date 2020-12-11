using UnityEngine;
using GameFramework;
using GameEvent;
using EntityCommand;

namespace Behavior
{
    public class Move : BehaviorBase
    {
        private Vector3 m_vec3Destination;

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
			Vector3 toMove = m_vec3Destination - Entity.Position;

            //  회전량에 따라 감속하는 정도를 다르게?
            //var angle = Vector3.Angle(Entity.Forward, toMove);
            //Entity.Velocity *=  Mathf.Lerp(0.8f, 0.5f, angle / 180);

            Entity.Velocity *= 0.9f;
            Entity.Velocity += (toMove.normalized * Entity.MovementSpeed * 0.2f);

            if (Entity.Velocity.magnitude > Entity.MovementSpeed)
            {
                Entity.Velocity = Entity.Velocity.normalized * Entity.MovementSpeed;
            }

            Vector3 moved = Entity.Velocity * DeltaTime;

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

        protected override void OnBehaviorEnd()
        {
            base.OnBehaviorEnd();

			Entity.Velocity = Vector3.zero;

            Entity.SendCommandToViews(new AnimatorSetBool("Move", false));
        }

        public override void SetData(int nBehaviorMasterID, params object[] param)
        {
            base.SetData(nBehaviorMasterID);

            m_vec3Destination = (Vector3)param[0];
        }
        #endregion

        public void SetDestination(Vector3 vec3Destination)
        {
            m_vec3Destination = vec3Destination;
        }

        public Vector3 GetDestination()
        {
            return m_vec3Destination;
        }
    }
}
