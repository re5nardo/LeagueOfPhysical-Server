﻿using UnityEngine;
using GameFramework;
using GameEvent;

namespace Behavior
{
    public class Move : BehaviorBase
    {
        private Vector3 m_vec3Destination;

        #region BehaviorBase
        protected override void OnBehaviorStart()
        {
            base.OnBehaviorStart();

            LOP.Game.Current.GameEventManager.SendToNear(new EntityBehaviorStart(Entity.EntityID, m_MasterData.ID, (SerializableVector3)m_vec3Destination), Entity.Position);
        }

        protected override bool OnBehaviorUpdate()
        {
			Vector3 toMove = m_vec3Destination - Entity.Position;

			Entity.Velocity = toMove.normalized * Entity.MovementSpeed;

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

        protected override void OnBehaviorEnd()
        {
            base.OnBehaviorEnd();

			Entity.Velocity = Vector3.zero;
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
