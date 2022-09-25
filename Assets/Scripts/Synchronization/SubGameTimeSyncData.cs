using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class SubGameTimeSyncData : ISyncData
{
    public double time;

    public SubGameTimeSyncData(double time)
    {
        this.time = time;
    }
}
