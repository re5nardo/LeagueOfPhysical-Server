using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityDie : IGameEvent
    {
        public int Seq { get; }
        public int Tick { get; set; }
        public int attackerID;
        public int deadID;
    }
}
