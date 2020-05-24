using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityBehaviorStart : IGameEvent
    {
        public int seq { get; }
        public int tick { get; set; }
        public int entityID;
        public int behaviorMasterID;
        //public SerializableVector3 position;
        //public SerializableVector3 rotation;
        public object[] param;

        public EntityBehaviorStart(int entityID, int behaviorMasterID, params object[] param)
        {
            seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            tick = Game.Current.CurrentTick;
            this.entityID = entityID;
            this.behaviorMasterID = behaviorMasterID;
            this.param = param;
        }
    }
}
