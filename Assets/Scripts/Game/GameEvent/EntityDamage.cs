using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityDamage : IGameEvent
    {
        public int seq { get; }
        public int tick { get; set; }
        public int attackerID;
        public int damagedID;
        public int damage;
        public int afterHP;

        public EntityDamage(int attackerID, int damagedID, int damage, int afterHP)
        {
            seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            tick = Game.Current.CurrentTick;
            this.attackerID = attackerID;
            this.damagedID = damagedID;
            this.damage = damage;
            this.afterHP = afterHP;
        }
    }
}
