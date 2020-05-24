using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityBehaviorEnd : IGameEvent
    {
        public int seq { get; }
        public int tick { get; set; }
        public int entityID;
        public int behaviorMasterID;

        public EntityBehaviorEnd(int entityID, int behaviorMasterID)
        {
            seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            tick = Game.Current.CurrentTick;
            this.entityID = entityID;
            this.behaviorMasterID = behaviorMasterID;
        }
    }
}
