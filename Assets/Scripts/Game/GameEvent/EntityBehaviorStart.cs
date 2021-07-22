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
        public int Seq { get; }
        public int Tick { get; set; }
        public int entityID;
        public int behaviorMasterID;
        public object[] param;

        public EntityBehaviorStart(int entityID, int behaviorMasterID, params object[] param)
        {
            Seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            Tick = Game.Current.CurrentTick;
            this.entityID = entityID;
            this.behaviorMasterID = behaviorMasterID;
            this.param = param;
        }
    }
}
