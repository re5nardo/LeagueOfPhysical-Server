using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityGetMoney : IGameEvent
    {
        public int seq { get; }
        public int tick { get; set; }
        public int entityID;
        public SerializableVector3 position;
        public int money;
        public int afterMoney;

        public EntityGetMoney(int entityID, SerializableVector3 position, int money, int afterMoney)
        {
            seq = LOP.Game.Current.GameEventManager.GenerateSeq();
            tick = Game.Current.CurrentTick;
            this.entityID = entityID;
            this.position = position;
            this.money = money;
            this.afterMoney = afterMoney;
        }
    }
}
