using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityHeal : IGameEvent
    {
        public int Seq { get; }
        public int Tick { get; set; }
        public int healingEntityID;
        public int healedEntityID;
        public int heal;
        public int afterHP;

        public EntityHeal(int healingEntityID, int healedEntityID, int heal, int afterHP)
        {
            Seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            Tick = Game.Current.CurrentTick;
            this.healingEntityID = healingEntityID;
            this.healedEntityID = healedEntityID;
            this.heal = heal;
            this.afterHP = afterHP;
        }
    }
}
