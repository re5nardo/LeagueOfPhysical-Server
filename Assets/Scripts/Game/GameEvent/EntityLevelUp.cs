using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityLevelUp : IGameEvent
    {
        public int seq { get; }
        public int tick { get; set; }
        public int entityID;
        public int level;

        public EntityLevelUp(int entityID, int level)
        {
            seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            tick = Game.Current.CurrentTick;
            this.entityID = entityID;
            this.level = level;
        }
    }
}
