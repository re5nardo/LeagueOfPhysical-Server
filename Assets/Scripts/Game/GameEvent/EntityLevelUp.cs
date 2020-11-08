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
        public int Seq { get; }
        public int Tick { get; set; }
        public int entityID;
        public int level;

        public EntityLevelUp(int entityID, int level)
        {
            Seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            Tick = Game.Current.CurrentTick;
            this.entityID = entityID;
            this.level = level;
        }
    }
}
