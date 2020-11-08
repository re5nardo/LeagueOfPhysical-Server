using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

namespace GameEvent
{
    [Serializable]
    public class EntityStateEnd : IGameEvent
    {
        public int Seq { get; }
        public int Tick { get; set; }
        public int entityID;
        public int stateMasterID;
    }
}
