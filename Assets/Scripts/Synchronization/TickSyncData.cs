using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class TickSyncData : ISyncData
{
    public int tick;

    public TickSyncData() { }

    public TickSyncData(int tick)
    {
        this.tick = tick;
    }
}
