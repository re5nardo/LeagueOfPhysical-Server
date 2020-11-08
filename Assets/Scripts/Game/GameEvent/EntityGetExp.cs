using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityGetExp : IGameEvent
    {
        public int Seq { get; }
        public int Tick { get; set; }
        public int entityID;
        public int exp;
        public int afterExp;

        public EntityGetExp(int entityID, int exp, int afterExp)
        {
            Seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            Tick = Game.Current.CurrentTick;
            this.entityID = entityID;
            this.exp = exp;
            this.afterExp = afterExp;
        }
    }
}
